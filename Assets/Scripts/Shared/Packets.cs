﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using LiteNetLib;
using LiteNetLib.Utils;
using Shared.StateChanges;
using ProtoBuf;
using Shared.SCData;

namespace Shared
{
    namespace SCPacket
    {
        public class PacketUtils
        {
            public static NetPacketProcessor CreateProcessor()
            {
                return new NetPacketProcessor();
            }
        }

        [ProtoContract]
        public class ClientID
        {
            [ProtoMember(1)]
            public int ID { get; set; }
        }

        [ProtoContract]
        public class WorldInitPacket
        {
            [ProtoMember(1)]
            public int Size { get; set; }
            [ProtoMember(2)]
            public float WorldX { get; set; }
            [ProtoMember(3)]
            public float WorldY { get; set; }
            [ProtoMember(4)]
            public int Chunks { get; set; }
        }

        [ProtoContract]
        public class WorldChunkPacket
        {
            [ProtoMember(1)]
            public int ChunkNumber { get; set; }
            [ProtoMember(2)]
            public int DataCount { get; set; }
            [ProtoMember(3)]
            public SCTileData[] TileData { get; set; }
        }

        [ProtoContract]
        public class NetUpdatePacket
        {
            [ProtoMember(1)]
            public int NetID { get; set; }

            [ProtoMember(2)]
            public NetUpdate Update { get; set; }
        }

        [ProtoContract]
        public class NetCreatePacket
        {
            [ProtoMember(1)]
            public int NetID{ get; set; }

            [ProtoMember(2)]
            public NetObjectType NetObjectType{ get; set; }

            [ProtoMember(3)]
            public EntityType EntityType { get; set; }

            [ProtoMember(4)]
            public int ParentID { get; set; }

            [ProtoMember(5)]
            public CreateData CreateData { get; set; }

            public string TypeName => EntityType == EntityType.NOTHING ? $"{NetObjectType}" : $"{EntityType}";
        }

        [ProtoContract]
        public class NetDestroyPacket
        {
            [ProtoMember(1)]
            public int NetID{ get; set; }
            [ProtoMember(2)]
            public DestroyData DestroyData { get; set; }
        }

        [ProtoContract]
        public class ClientRequestPacket
        {
            [ProtoMember(1)]
            public int ClientID { get; set; }
            [ProtoMember(2)]
            public ClientRequest Request { get; set; }
        }

        //[ProtoContract]
        //public class StateChangePacket
        //{
        //    [ProtoMember(1)]
        //    public int Version { get; set; }
        //    [ProtoMember(2)]
        //    public byte TotalChanges { get; set; }
        //    [ProtoMember(3)]
        //    public byte ChangeNumber { get; set; }
        //    [ProtoMember(4)]
        //    public IStateChange Change { get; set; }
        //}

    }
}