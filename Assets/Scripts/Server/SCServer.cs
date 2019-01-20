using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Net;
using System.Net.Sockets;
using Shared.SCPacket;
using Shared.SCData;
using Shared.StateChanges;
using Shared;
using Utils;

namespace Server
{
    public class SCServer : MonoBehaviour, INetEventListener
    {
        private float elapsed;
        private float startTime;
        private int nextClientID = 0;

        private NetManager server;
        private NetDataWriter writer;
        private Dictionary<int, NetPeer> connectedClients;
        private Dictionary<int, int> clientVersions;
        private NetPacketProcessor processor;
        private WorldStateManager stateManager;

        //private ProtoSerializer serializer;

        void Start()
        {
            writer = new NetDataWriter();

            // Start the server on port 5000
            server = new NetManager(this);
            server.Start(5000);
            server.DiscoveryEnabled = true;
            server.UpdateTime = 15;

            connectedClients = new Dictionary<int, NetPeer>();
            clientVersions = new Dictionary<int, int>();

            stateManager = new WorldStateManager(this, Constants.WORLD_SIZE);

            processor = PacketUtils.CreateProcessor();
            processor.Subscribe<UpdatePacket, NetPeer>(OnUpdatePacket, () => new UpdatePacket());
            processor.Subscribe<ClientRequestPacket, NetPeer>(OnClientRequest, () => new ClientRequestPacket());
        }

        // Update is called once per frame
        void Update()
        {
            server.PollEvents();

            elapsed += Time.deltaTime;

            if (elapsed > SCNetworkManager.UPS_INV)
            {
                // @Todo Fix this
                stateManager.Update(elapsed);
                elapsed -= SCNetworkManager.UPS_INV;
            }
        }

        private void OnDestroy()
        {
            server.Stop();
        }

        public void SendToAllClients<T>(T packet, DeliveryMethod method = DeliveryMethod.ReliableOrdered) where T : class, new()
        {
            foreach(var clientID in connectedClients.Keys)
            {
                processor.Send(connectedClients[clientID], packet, method);
            }
        }

        public void SendToSingleClient<T>(T packet, int clientID, DeliveryMethod method = DeliveryMethod.ReliableOrdered) where T : class, new()
        {
            processor.Send(connectedClients[clientID], packet, method);
        }


        void OnUpdatePacket(UpdatePacket packet, NetPeer peer)
        {
            //Debug.Log($"[Server] received update packet with version {packet.Version}");
            StateChangePacket[] changes = stateManager.GetDiff(clientVersions[packet.ClientID]);
            clientVersions[packet.ClientID] = stateManager.Version - 1;

            if (changes == null) return;


            foreach(StateChangePacket change in changes)
            {
                if (change.Change is EntityRemove)
                {
                    Debug.Log($"[Server] - deleting entity with ID {(change.Change as EntityRemove).ID}");
                }
                //serializer.Send(peer, change, DeliveryMethod.ReliableUnordered);
                processor.Send(peer, change, DeliveryMethod.ReliableUnordered);
                //peer.Send(data, 0, data.Length, DeliveryMethod.ReliableUnordered);
            }
        }

        void OnClientRequest(ClientRequestPacket packet, NetPeer peer) 
        {
            stateManager.AddRequest(packet.ClientID, packet.Request);
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            processor.ReadAllPackets(reader, peer);
        }

        public void OnPeerConnected(NetPeer peer)
        {
            Debug.Log($"[Server] peer connected: {peer.EndPoint}");

            int clientID = nextClientID++;
            connectedClients.Add(clientID, peer);
            clientVersions.Add(clientID, 0);

            // Create player entity
            stateManager.AddPlayer(clientID);

            SendClientID(clientID);
            SendWorldData(peer);
        }

        private void SendClientID(int clientID)
        {
            processor.Send(connectedClients[clientID], new ClientID { ID = clientID }, DeliveryMethod.ReliableOrdered);
        }

        private void SendWorldData(NetPeer peer)
        {
            // SEND WORLD DATA
            TileID[][] tiles = stateManager.GetTiles();
            int worldSize = stateManager.Size;

            int tilesPerChunk = 64;
            int numPackets = (worldSize * worldSize) / tilesPerChunk + 1;
            int lastPacketSize = (worldSize * worldSize) % tilesPerChunk;

            // Send the header packet containing world meta data
            processor.Send(peer,
                new WorldInitPacket
                {
                    Chunks = numPackets,
                    Size = worldSize,
                    WorldX = 0.5f,
                    WorldY = 0.5f
                },
                DeliveryMethod.ReliableOrdered
            );

            int packetIndex = 1;
            int tileCount = 0;
            SCTileData[] tileArr = new SCTileData[packetIndex == numPackets ? lastPacketSize : tilesPerChunk];
            for (int x = 0; x < worldSize; x++)
            {
                for (int y = 0; y < worldSize; y++)
                {
                    tileArr[tileCount++] = new SCTileData { X = x, Y = y, Z = 0, TileID = tiles[x][y] };

                    // Once we've hit the max tile count, send a new packet to the client with the chunk data
                    if (tileCount == tilesPerChunk)
                    {
                        processor.Send(peer,
                            new WorldChunkPacket
                            {
                                ChunkNumber = packetIndex,
                                TileData = tileArr,
                                DataCount = tileCount
                            }, DeliveryMethod.ReliableUnordered);

                        // @Performance Allocating new memory here is probably not the best.
                        // Alternatively, we can just overwrite the values existing for the next
                        // packet. However, if the packet doesn't use all tilesPerPacket slots, 
                        // then the data at the end of the array is not going to be overwritten.
                        packetIndex++;
                        tileCount = 0;
                        tileArr = new SCTileData[packetIndex == numPackets ? lastPacketSize : tilesPerChunk];
                    }
                }
            }
            processor.Send(peer,
            new WorldChunkPacket
            {
                ChunkNumber = packetIndex,
                TileData = tileArr,
                DataCount = tileCount
            }, DeliveryMethod.ReliableUnordered);
        }

        // @Todo We need to figure out what the client ID is when a client disconnects
        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Debug.Log($"[Server] peer disconnected: {peer.EndPoint}");
            //connectedClients.Remove(peer);
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            if (messageType == UnconnectedMessageType.DiscoveryRequest)
            {
                Debug.Log($"[Server] received discovery request from {remoteEndPoint}");
                server.SendDiscoveryResponse(new byte[] { 0 }, remoteEndPoint);
            }
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            request.AcceptIfKey("space_colony");
        }
    }

}