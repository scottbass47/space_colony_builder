using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using SCPacket;

public class SCServer : MonoBehaviour, INetEventListener
{
    private NetManager server;
    private NetDataWriter writer;
    private List<NetPeer> connectedClients;

    void Start()
    {
        writer = new NetDataWriter();

        // Start the server on port 5000
        server = new NetManager(this, 2, "space_colony");
        server.Start(5000);
        server.DiscoveryEnabled = true;
        server.UpdateTime = 15;

        connectedClients = new List<NetPeer>();
    }

    // Update is called once per frame
    void Update()
    {
        server.PollEvents(); 
    }

    private void OnDestroy()
    {
        server.Stop(); 
    }

    public void OnNetworkError(NetEndPoint endPoint, int socketErrorCode)
    {
        Debug.Log("[Server] error: " + socketErrorCode);
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
    }

    public void OnNetworkReceive(NetPeer peer, NetDataReader reader)
    {
        Debug.Log($"[Server] received packet from {peer.EndPoint}");
        PacketType type = (PacketType)reader.GetInt();
        DeleteTilePacket packet = default(DeleteTilePacket);
        NetSerializer serializer = PacketUtils.GetSerializer();
        serializer.Subscribe<DeleteTilePacket>(
            p =>
            {
                packet = p;    
            }
        );
        serializer.ReadPacket(reader);
        Debug.Log($"[Server] request to delete tile at {packet.Pos}");

        writer.Reset();
        writer.Put((int)type);
        serializer.Serialize(writer, packet);
        foreach(var client in connectedClients) {
            client.Send(writer, SendOptions.ReliableOrdered); 
        }
    }

    public void OnNetworkReceiveUnconnected(NetEndPoint remoteEndPoint, NetDataReader reader, UnconnectedMessageType messageType)
    {
        if(messageType == UnconnectedMessageType.DiscoveryRequest)
        {
            Debug.Log($"[Server] received discovery request from {remoteEndPoint}");
            server.SendDiscoveryResponse(new byte[] { 1 }, remoteEndPoint);
        }
    }

    public void OnPeerConnected(NetPeer peer)
    {
        Debug.Log($"[Server] peer connected: {peer.EndPoint}");
        connectedClients.Add(peer);
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Debug.Log($"[Server] peer disconnected: {peer.EndPoint}");
        connectedClients.Remove(peer);
    }

}
