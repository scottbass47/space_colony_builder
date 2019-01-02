using System;
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
using Utils;

namespace Client
{
    public class SCClient : MonoBehaviour, INetEventListener
    {
        private NetManager client;
        private NetPeer peer; // this should be the server
        private float elapsed;
        private NetPacketProcessor processor;
        private int clientID;
        private bool clientIDSet = false;
        private int MyVersion = 0;

        private bool loadingWorld = true;
        private int chunksReceived = 0;
        private int totalChunks = 0;
        private SCTileData[] tileDataBuffer;
        private int tileBufferIdx;
        public GameObject groundPrefab;
        public GameObject ground;
        public TileBase tileBase;

        private Dictionary<int, List<StateChangePacket>> stateChangeBuffer;

        // @ Hack Once again, C# generics let us down but this time it's even worse.
        // Instead of being able to use Action<T> where T : IStateChange to have generic
        // listeners for different types of events, we have to use object and cast. Lets
        // hope this doesn't cause problems.
        private Dictionary<Type, List<Action<IStateChange>>> eventTable;

        void Start()
        {
            client = new NetManager(this);
            client.Start();

            processor = PacketUtils.CreateProcessor();

            processor.SubscribeReusable<ClientID>(
                (packet) => 
                {
                    clientID = packet.ID;
                    clientIDSet = true;
                }
            );
            processor.SubscribeReusable<WorldInitPacket>(WorldInit);
            processor.SubscribeReusable<WorldChunkPacket>(WorldChunk);
            processor.Subscribe<StateChangePacket>(StateChanges, () => new StateChangePacket());

            stateChangeBuffer = new Dictionary<int, List<StateChangePacket>>();
            eventTable = new Dictionary<Type, List<Action<IStateChange>>>();

            AddStateChangeListener<EntitySpawn>((spawn) =>
            {
                var go = Game.Instance.EntityObjectFactory.CreateEntityObject(spawn.EntityType, spawn);
                //Instantiate(go);
            });
            
            AddStateChangeListener<EntityRemove>((remove) =>
            {
                var go = Game.Instance.EntityManager.GetEntity(remove.ID);
                Destroy(go); 
            });
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
            else if (!loadingWorld && clientIDSet)
            {
                // Request updates
                SendPacket(new UpdatePacket { ClientID = clientID });
            }
        }

        public void AddStateChangeListener<T>(Action<T> listener) where T : IStateChange
        {
            Type t = typeof(T);
            if(!eventTable.ContainsKey(t))
            {
                eventTable.Add(t, new List<Action<IStateChange>>());
            }

            // Why C#? 
            Action<IStateChange> action = (change) =>
            {
                var cast = (T)Convert.ChangeType(change, t);
                listener(cast);
            };
            eventTable[t].Add(action);
        }

        // @Test this needs to be tested, I doubt it works.
        public void RemoveStateChangeListener<T>(Action<T> listener) where T : IStateChange
        {
            Type t = typeof(T);
            DebugUtils.Assert(eventTable.ContainsKey(t));

            Action<IStateChange> action = (change) =>
            {
                var cast = (T)Convert.ChangeType(change, t);
                listener(cast);
            };
            eventTable[t].Remove(action);
        }

        private void NotifyStateChange<T>(T change) where T : IStateChange
        {
            Type t = change.GetType();
            if (!eventTable.ContainsKey(t)) return;

            foreach(var listener in eventTable[t])
            {
                listener(change);
            }
        }

        void StateChanges(StateChangePacket packet)
        {
            Debug.Log($"[Client] received state changes for version {packet.Version}. Change {packet.ChangeNumber}/{packet.TotalChanges}");

            if (!stateChangeBuffer.ContainsKey(packet.Version))
            {
                stateChangeBuffer[packet.Version] = new List<StateChangePacket>();
            }

            stateChangeBuffer[packet.Version].Add(packet);

            // Try and apply changes to the oldest version we don't have
            TryApplyChanges(MyVersion + 1); 
        }

        // Checks if all the changes for a given version number have arrived,
        // and if so, applies the changes and increments the version.
        private void TryApplyChanges(int version)
        {
            if (!stateChangeBuffer.ContainsKey(version)) return;

            if (stateChangeBuffer[version].Count == stateChangeBuffer[version][0].TotalChanges)
            {
                Debug.Log($"[Client] - applying changes for version {version}");
                foreach (var packet in stateChangeBuffer[version])
                {
                    IStateChange change = packet.Change;
                    NotifyStateChange(change);
                }

                // After applying all the changes, we can safely increment our version of
                // the world and try to apply the next version's changes.
                MyVersion = version;
                stateChangeBuffer.Remove(version);
                TryApplyChanges(version + 1);
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

            //GameObject.Find("Loading Screen").SetActive(false);
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