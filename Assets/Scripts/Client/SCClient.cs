using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine.Tilemaps;
using System.Net;
using System.Net.Sockets;
using Shared.SCPacket;
using Shared.SCData;
using Shared.StateChanges;

namespace Client
{
    public class SCClient : MonoBehaviour, INetEventListener
    {
        private NetManager client;
        private NetPeer peer; // this should be the server
        private float elapsed;
        private NetPacketProcessor processor;
        private int MyVersion;

        private bool loadingWorld = true;
        private int chunksReceived = 0;
        private int totalChunks = 0;
        private SCTileData[] tileDataBuffer;
        private int tileBufferIdx;
        public GameObject groundPrefab;
        public GameObject ground;
        public TileBase tileBase;

        void Start()
        {
            client = new NetManager(this);
            client.Start();

            processor = PacketUtils.CreateProcessor();

            processor.SubscribeReusable<DeleteTilePacket>(DeleteTile);
            processor.SubscribeReusable<StateChangesPacket>(StateChanges);
            processor.SubscribeReusable<WorldInitPacket>(WorldInit);
            processor.SubscribeReusable<WorldChunkPacket>(WorldChunk);
        }

        // Update is called once per frame
        void Update()
        {
            client.PollEvents();

            // While the server is null, try to discover it
            if (peer == null)
            {
                client.SendDiscoveryRequest(new byte[] { 1 }, 5000);
            }
            else if (!loadingWorld)
            {
                // Request updates
                SendPacket(new UpdatePacket { Version = MyVersion });
            }

        }

        void DeleteTile(DeleteTilePacket packet)
        {
            Debug.Log($"[Client] recieved from server request to delete tile at {packet.Pos}");
            Tilemap tileMap = GameObject.Find("Ground").GetComponentInChildren<Tilemap>();
            tileMap.SetTile(packet.Pos, null);
        }

        void StateChanges(StateChangesPacket packet)
        {
            if (MyVersion == packet.Version) return;

            Debug.Log($"[Client] received state changes");
            MyVersion = packet.Version;

            foreach(IStateChange change in packet.Changes)
            {
                SCTileData data = (SCTileData)change;

                Tilemap tilemap = ground.GetComponentInChildren<Tilemap>();
                TileStore tileStore = GameObject.Find("Tile Registry").GetComponent<TileStore>();

                tilemap.SetTile(new Vector3Int { x = data.X, y = data.Y, z = data.Z }, tileStore.Get(data.TileID));
            }
        }

        void WorldInit(WorldInitPacket packet)
        {
            Debug.Log($"SCClient.WorldChunk - Received world init packet. Expecting {packet.Chunks} chunks of data.");
            tileDataBuffer = new SCTileData[packet.Size * packet.Size];
            tileBufferIdx = 0;
            totalChunks = packet.Chunks;
        }

        // @Bug NullPointer if the Init packet is dropped (TileChange arr doesn't get created)
        void WorldChunk(WorldChunkPacket packet)
        {
            chunksReceived++;
            Debug.Log($"SCClient.WorldChunk - Received chunk {packet.ChunkNumber}.");

            for (int i = 0; i < packet.DataCount; i++)
            {
                tileDataBuffer[tileBufferIdx++] = packet.TileData[i];
            }

            // We have all the data, create the world
            if (chunksReceived == totalChunks)
            {
                CreateWorld();
            }
        }

        void CreateWorld()
        {
            Debug.Log("SCClient.CreateWorld - All data received! Creating world.");

            ground = Instantiate(groundPrefab);

            Tilemap tilemap = ground.GetComponentInChildren<Tilemap>();
            tilemap.ClearAllTiles();

            TileStore tileStore = GameObject.Find("Tile Registry").GetComponent<TileStore>();

            foreach (SCTileData data in tileDataBuffer)
            {
                tilemap.SetTile(new Vector3Int { x = data.X, y = data.Y, z = data.Z }, tileStore.Get(data.TileID));
            }

            tilemap.CompressBounds();
            Vector3 center = tilemap.cellBounds.center;
            Vector3 camPos = tilemap.CellToWorld(new Vector3Int((int)center.x, (int)center.y, 0));
            camPos.z = -10;

            Camera.main.transform.position = camPos;

            loadingWorld = false;

            GameObject.Find("Loading Screen").SetActive(false);
        }

        public void SendPacket<T>(T packet) where T : class, new()
        {
            //Debug.Log($"[Client] sending packet of type {packet.GetType()} to server");

            processor.Send(peer, packet, DeliveryMethod.ReliableOrdered);
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
        }

        public void OnPeerConnected(NetPeer peer)
        {
            Debug.Log($"[Client] peer connected to {peer.EndPoint}");
            this.peer = peer;
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            processor.ReadAllPackets(reader);
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            if (messageType == UnconnectedMessageType.DiscoveryResponse)
            {
                Debug.Log($"[Client] discovery response received, connecting to server.");
                client.Connect(remoteEndPoint, "space_colony");
            }
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
        }
    }
}