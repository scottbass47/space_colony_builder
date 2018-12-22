using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;
using LiteNetLib.Utils;
using SCPacket;
using UnityEngine.Tilemaps;

public class SCClient : MonoBehaviour, INetEventListener
{
    private NetManager client;
    private NetPeer peer; // this should be the server

    void Start()
    {
        client = new NetManager(this, "space_colony");
        client.Start();
    }

    // Update is called once per frame
    void Update()
    {
        client.PollEvents();

        // While the server is null, try to discover it
       if(peer == null)
       {
           client.SendDiscoveryRequest(new byte[] { 1 }, 5000);
       }

    }

    public void SendPacket<T>(PacketType type, T packet) where T : struct
    {
        Debug.Log($"[Client] sending packet of type {type} to server");
        NetDataWriter writer = new NetDataWriter();
        NetSerializer serializer = PacketUtils.GetSerializer();
        
        writer.Put((int)type);
        serializer.Serialize(writer, packet);
        peer.Send(writer, SendOptions.ReliableOrdered);
    }

    public void OnNetworkError(NetEndPoint endPoint, int socketErrorCode)
    {
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
    }

    public void OnNetworkReceive(NetPeer peer, NetDataReader reader)
    {
        Debug.Log($"[Client] received packet from {peer.EndPoint}");
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
        Debug.Log($"[Client] recieved from server request to delete tile at {packet.Pos}");
        Tilemap tileMap = GameObject.Find("Ground").GetComponentInChildren<Tilemap>();
        tileMap.SetTile(packet.Pos, null);
    }

    public void OnNetworkReceiveUnconnected(NetEndPoint remoteEndPoint, NetDataReader reader, UnconnectedMessageType messageType)
    {
        if(messageType == UnconnectedMessageType.DiscoveryResponse)
        {
            Debug.Log($"[Client] discovery response received, connecting to server.");
            client.Connect(remoteEndPoint.Host, remoteEndPoint.Port);
        }
    }

    public void OnPeerConnected(NetPeer peer)
    {
        Debug.Log($"[Client] peer connected to {peer.EndPoint}");
        this.peer = peer;
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
    }

}
