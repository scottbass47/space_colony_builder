using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using LiteNetLib;
using LiteNetLib.Utils;
using Shared.StateChanges;

namespace Shared
{
    namespace SCPacket
    {
        public class PacketUtils
        {

            // We use singleton here so we don't have to keep creating new instances of
            // the serializer every time we want to send and receive packets
            public static NetPacketProcessor CreateProcessor()
            {
                NetPacketProcessor processor = new NetPacketProcessor();
                processor.RegisterNestedType<Vector3Int>(
                    (writer, vec) =>
                    {
                        writer.Put(vec.x);
                        writer.Put(vec.y);
                        writer.Put(vec.z);
                    },
                    reader =>
                    {
                        return new Vector3Int
                        {
                            x = reader.GetInt(),
                            y = reader.GetInt(),
                            z = reader.GetInt()
                        };
                    }
                );
                processor.RegisterNestedType<Vector3>(
                    (writer, vec) =>
                    {
                        writer.Put(vec.x);
                        writer.Put(vec.y);
                        writer.Put(vec.z);
                    },
                    reader =>
                    {
                        return new Vector3
                        {
                            x = reader.GetFloat(),
                            y = reader.GetFloat(),
                            z = reader.GetFloat()
                        };
                    }
                );
                processor.RegisterNestedType<IStateChange>(
                    (writer, change) =>
                    {
                        writer.Put(change.Version);
                        writer.Put(change.GetType().ToString());
                        change.Serialize(writer);
                    },
                    reader =>
                    {
                        // @Performance Using reflection here might be slow considering how often this
                        // code is going to be run and how fast using reflection is to instantiate objects
                        // based on runtime types.

                        int version = reader.GetInt();
                        Type type = Type.GetType($"{reader.GetString()}, Assembly-CSharp");
                        var obj = Activator.CreateInstance(type);
                        IStateChange change = (IStateChange)obj;
                        change.Version = version;
                        change.Deserialize(reader);
                        return change;
                    }
                );
                processor.RegisterNestedType<SCTileData>(() => new SCTileData());
                return processor;
            }
        }

        public enum PacketType
        {
            WORLD_INIT,
            DELETE_TILE
        }

        public class ClientID
        {
            public int ID { get; set; }
        }

        public class WorldInitPacket
        {
            public int Size { get; set; }
            public float WorldX { get; set; }
            public float WorldY { get; set; }
            public int Chunks { get; set; }
        }

        public class WorldChunkPacket
        {
            public int ChunkNumber { get; set; }
            public int DataCount { get; set; }
            public SCTileData[] TileData { get; set; }
        }

        public class UpdatePacket
        {
            public int ClientID { get; set; }
        }

        public class StateChangePacket
        {
            public int Version { get; set; }
            public byte TotalChanges { get; set; }
            public byte ChangeNumber { get; set; }
            public IStateChange Change { get; set; }
        }

    }
}