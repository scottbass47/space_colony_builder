using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteNetLib;
using LiteNetLib.Utils;
using ProtoBuf;
using ProtoBuf.Meta;
using Shared.SCPacket;
using UnityEngine;

namespace Utils
{
    // @Performance Reusue memory stream
    public sealed class ProtoSerializer
    {
        private Dictionary<Type, Action<NetPeer>> callbacks;

        static ProtoSerializer()
        {
            RuntimeTypeModel.Default[typeof(Vector3)].SetSurrogate(typeof(Vector3Surrogate));
        }

        public ProtoSerializer()
        {
            callbacks = new Dictionary<Type, Action<NetPeer>>();
        }

        public byte[] Serialize<T>(T obj) where T : class
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    Serializer.Serialize(stream, obj);
                    return stream.ToArray();
                }
            }
            catch 
            {
                throw;
            }
        }

        public T Deserialize<T>(byte[] bytes, int offset, long length, out long bytesRead) where T : class
        {
            try
            {
                using (var stream = new MemoryStream(bytes))
                {
                    stream.Position = offset;
                    stream.SetLength(length);
                    T obj = Serializer.Deserialize<T>(stream);
                    bytesRead = stream.Position - offset;
                    return obj;
                }
            }
            catch 
            {
                throw;
            }
        }
    }

    [ProtoContract]
    public class Vector3Surrogate
    {
        [ProtoMember(1)]
        public float x { get; set; }
        [ProtoMember(2)]
        public float y { get; set; }
        [ProtoMember(3)]
        public float z { get; set; }

        public static implicit operator Vector3(Vector3Surrogate v)
        {
            return new Vector3 { x = v.x, y = v.y, z = v.z };
        }
        //  User-defined conversion from double to Digit
        public static implicit operator Vector3Surrogate(Vector3 v)
        {
            return new Vector3Surrogate { x = v.x, y = v.y, z = v.z };
        }
    }
}
