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
using Shared;

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

        //private Dictionary<Type, List<Action<object>>> eventTable;
        private EventTable<object> eventTable;

        void Awake()
        {
            client = new NetManager(this);
            client.Start();

            processor = PacketUtils.CreateProcessor();

            processor.Subscribe<ClientID>(
                (packet) =>
                {
                    clientID = packet.ID;
                    clientIDSet = true;
                },
                () => new ClientID()
            );
            processor.Subscribe<WorldInitPacket>(NotifyPacketListeners<WorldInitPacket>, () => new WorldInitPacket());
            processor.Subscribe<WorldChunkPacket>(NotifyPacketListeners<WorldChunkPacket>, () => new WorldChunkPacket());
            processor.Subscribe<StateChangePacket>(NotifyPacketListeners<StateChangePacket>, () => new StateChangePacket());
            processor.Subscribe<NetCreatePacket>(NotifyPacketListeners<NetCreatePacket>, () => new NetCreatePacket());
            processor.Subscribe<NetUpdatePacket>(NotifyPacketListeners<NetUpdatePacket>, () => new NetUpdatePacket());
            processor.Subscribe<NetDestroyPacket>(NotifyPacketListeners<NetDestroyPacket>, () => new NetDestroyPacket());

            //eventTable = new Dictionary<Type, List<Action<object>>>();
            eventTable = new EventTable<object>();

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
            else if (!Game.Instance.World.loadingWorld && clientIDSet)
            {
                // Request updates
                SendPacket(new UpdatePacket { ClientID = clientID });
            }
        }

        public void AddPacketListener<T>(Action<T> listener) 
        {
            eventTable.AddListener(listener);
        }

        public void RemoveStateChangeListener<T>(Action<T> listener) 
        {
            eventTable.RemoveListener(listener);
        }

        private void NotifyPacketListeners<T>(T packet) 
        {
            eventTable.NotifyListeners(packet);
        }

        public void SendPacket<T>(T packet) where T : class, new()
        {
            processor.Send(peer, packet, DeliveryMethod.ReliableOrdered);
        }

        public void SendRequestPacket(ClientRequest request)
        {
            SendPacket(new ClientRequestPacket { ClientID = clientID, Request = request });
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