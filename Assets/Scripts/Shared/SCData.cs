using UnityEngine;
using System.Collections;
using LiteNetLib.Utils;
using ECS;

namespace Shared
{
    namespace SCData
    {
        // @Cleanup TileAssets and their corresponding IDs need a stronger connection
        public enum TileID
        {
            GROUND,
            ROCK,
            TOWER
        }

        public enum EntityType
        {
            ROCK
        }



        public class EntityData : INetSerializable
        {
            private Bits changedBits;

            public void Deserialize(NetDataReader reader)
            {
                Bits bits = new Bits();
                bits.Deserialize(reader);
                changedBits = bits;

                foreach(bool bit in changedBits)
                {
                }
            }

            public void Serialize(NetDataWriter writer)
            {
            }
        }


    }
}