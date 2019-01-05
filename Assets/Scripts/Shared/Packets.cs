using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using LiteNetLib;
using LiteNetLib.Utils;
using Shared.StateChanges;
using ProtoBuf;

namespace Shared
{
    namespace SCPacket
    {
        public class PacketUtils
        {
            private static Dictionary<string, Type> typeCache = new Dictionary<string, Type>();

            // We use singleton here so we don't have to keep creating new instances of
            // the serializer every time we want to send and receive packets
            public static NetPacketProcessor CreateProcessor()
            {
                NetPacketProcessor processor = new NetPacketProcessor();
                //processor.RegisterNestedType<Vector3Int>(
                //    (writer, vec) =>
                //    {
                //        writer.Put(vec.x);
                //        writer.Put(vec.y);
                //        writer.Put(vec.z);
                //    },
                //    reader =>
                //    {
                //        return new Vector3Int
                //        {
                //            x = reader.GetInt(),
                //            y = reader.GetInt(),
                //            z = reader.GetInt()
                //        };
                //    }
                //);
                //processor.RegisterNestedType<Vector3>(
                //    (writer, vec) =>
                //    {
                //        writer.Put(vec.x);
                //        writer.Put(vec.y);
                //        writer.Put(vec.z);
                //    },
                //    reader =>
                //    {
                //        return new Vector3
                //        {
                //            x = reader.GetFloat(),
                //            y = reader.GetFloat(),
                //            z = reader.GetFloat()
                //        };
                //    }
                //);
                //processor.RegisterNestedType<IStateChange>(
                //    (writer, change) =>
                //    {
                //        writer.Put(change.Version);
                //        writer.Put(change.GetType().ToString());
                //        change.Serialize(writer);
                //    },
                //    reader =>
                //    {
                //        int version = reader.GetInt();
                //        string str = reader.GetString();

                //        Type type = null;
                //        if(!typeCache.TryGetValue(str, out type))
                //        {
                //            type = Type.GetType($"{str}, Assembly-CSharp");
                //            typeCache.Add(str, type);
                //        }

                //        var obj = Activator.CreateInstance(type);
                //        IStateChange change = (IStateChange)obj;
                //        change.Version = version;
                //        change.Deserialize(reader);
                //        return change;
                //    }
                //);
                //processor.RegisterNestedType<SCTileData>(() => new SCTileData());
                return processor;
            }
        }

        public enum PacketType
        {
            WORLD_INIT,
            DELETE_TILE
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
        public class UpdatePacket
        {
            [ProtoMember(1)]
            public int ClientID { get; set; }
        }

        [ProtoContract]
        public class StateChangePacket
        {
            [ProtoMember(1)]
            public int Version { get; set; }
            [ProtoMember(2)]
            public byte TotalChanges { get; set; }
            [ProtoMember(3)]
            public byte ChangeNumber { get; set; }
            [ProtoMember(4)]
            public IStateChange Change { get; set; }
        }

    }
}