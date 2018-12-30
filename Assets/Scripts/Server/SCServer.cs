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

namespace Server
{
    public class SCServer : MonoBehaviour, INetEventListener
    {
        public int WorldSize = 20;
        private float elapsed;

        private NetManager server;
        private NetDataWriter writer;
        private List<NetPeer> connectedClients;
        private NetPacketProcessor processor;
        private WorldStateManager stateManager;

        void Start()
        {
            writer = new NetDataWriter();

            // Start the server on port 5000
            server = new NetManager(this);
            server.Start(5000);
            server.DiscoveryEnabled = true;
            server.UpdateTime = 15;

            connectedClients = new List<NetPeer>();

            stateManager = new WorldStateManager(WorldSize);

            processor = PacketUtils.CreateProcessor();
            processor.SubscribeReusable<UpdatePacket, NetPeer>(OnUpdatePacket);

            processor.SubscribeReusable<DeleteTilePacket>(
                packet =>
                {
                    Debug.Log($"[Server] received delete tile packet at position {packet.Pos}");

                    foreach (NetPeer peer in connectedClients)
                    {
                        Debug.Log($"[Server] sending packet to peer {peer.EndPoint}");
                        processor.Send(peer, packet, DeliveryMethod.ReliableOrdered);
                    }
                }
            );
        }

        // Update is called once per frame
        void Update()
        {
            server.PollEvents();

            elapsed += Time.deltaTime;

            if (elapsed > SCNetworkManager.UPS_INV)
            {
                // Lets modify tiles ever 1/2 updates 
                var num = Random.Range(0, 2);
                if (num == 0)
                {
                    int x = Random.Range(0, WorldSize);
                    int y = Random.Range(0, WorldSize);
                    int z = 0;

                    Debug.Log($"[Server] changing tile at ({x}, {y}) to GROUND tile.");
                    stateManager.ApplyChange(new SCTileData { X = x, Y = y, Z = z, TileID = TileID.GROUND, Type = TileChangeType.CHANGE });
                }
                stateManager.Update();
                elapsed -= SCNetworkManager.UPS_INV;
            }
        }

        private void OnDestroy()
        {
            server.Stop();
        }

        void OnUpdatePacket(UpdatePacket packet, NetPeer peer)
        {
            //Debug.Log($"[Server] received update packet with version {packet.Version}");
            StateChangesPacket changes = stateManager.GetDiff(packet.Version);
            if (changes == null) return;

            processor.Send(peer, changes, DeliveryMethod.Unreliable);
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
            connectedClients.Add(peer);
            SendWorldData(peer);
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

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Debug.Log($"[Server] peer disconnected: {peer.EndPoint}");
            connectedClients.Remove(peer);
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            if (messageType == UnconnectedMessageType.DiscoveryRequest)
            {
                Debug.Log($"[Server] received discovery request from {remoteEndPoint}");
                server.SendDiscoveryResponse(new byte[] { 1 }, remoteEndPoint);
            }
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            request.AcceptIfKey("space_colony");
        }
    }

}