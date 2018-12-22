using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using LiteNetLib;
using LiteNetLib.Utils;

namespace SCPacket
{
    public class PacketUtils
    {
        private static NetSerializer SerializerInstance = null;
            
        // We use singleton here so we don't have to keep creating new instances of
        // the serializer every time we want to send and receive packets
        public static NetSerializer GetSerializer()
        {
            if (SerializerInstance == null)
            {
                NetSerializer serializer = new NetSerializer();
                serializer.RegisterCustomType<Vector3Int>(
                    (writer, vec) =>
                    {
                        writer.Put(vec.x);
                        writer.Put(vec.y);
                        writer.Put(vec.z);
                    },
                    reader =>
                    {
                        return new Vector3Int {
                            x = reader.GetInt(),
                            y = reader.GetInt(),
                            z = reader.GetInt()
                        };
                    }
                );
                SerializerInstance = serializer;
            }
            return SerializerInstance;
        }
    }

    public enum PacketType
    {
        DELETE_TILE
    }

    public struct DeleteTilePacket
    {
        public Vector3Int Pos { get; set; }
    } 

}
