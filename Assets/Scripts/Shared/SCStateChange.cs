using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using Server;
using Shared.SCData;
using Shared.SCPacket;

namespace Shared
{
    namespace StateChanges
    {
        public abstract class IStateChange : INetSerializable
        {
            public int Version { get; set; }

            public abstract void Deserialize(NetDataReader reader);
            public abstract void Serialize(NetDataWriter writer);
        }

        public class SCTileData : IStateChange
        {
            public TileChangeType Type { get; set; }
            public TileID TileID { get; set; } // Used for tile CREATE and CHANGE
            public int X { get; set; }
            public int Y { get; set; }
            public int Z { get; set; }

            public override void Deserialize(NetDataReader reader)
            {
                Type = (TileChangeType)reader.GetInt();
                TileID = (TileID) reader.GetInt();
                X = reader.GetInt();
                Y = reader.GetInt();
                Z = reader.GetInt();
            }

            public override void Serialize(NetDataWriter writer)
            {
                writer.Put((int)Type);
                writer.Put((int)TileID);
                writer.Put(X);
                writer.Put(Y);
                writer.Put(Z);
            }
        }

        public enum TileChangeType
        {
            CREATE,
            DELETE,
            CHANGE
        }

        // @Hack We should be able to serialize Vector3 and other nested types automatically
        public class EntitySpawn : IStateChange
        {
            public int ID { get; set; }
            public EntityType EntityType { get; set; }
            public Vector3 Pos { get; set; }

            public override void Deserialize(NetDataReader reader)
            {
                ID = reader.GetInt();
                EntityType = (EntityType)reader.GetInt();
                Pos = new Vector3
                {
                    x = reader.GetFloat(),
                    y = reader.GetFloat(),
                    z = reader.GetFloat()
                };
            }

            public override void Serialize(NetDataWriter writer)
            {
                writer.Put(ID);
                writer.Put((int)EntityType);
                writer.Put(Pos.x);
                writer.Put(Pos.y);
                writer.Put(Pos.z);
            }
        }

        public class EntityRemove : IStateChange
        {
            public int ID { get; set; }

            public override void Deserialize(NetDataReader reader)
            {
                ID = reader.GetInt();
            }

            public override void Serialize(NetDataWriter writer)
            {
                writer.Put(ID);
            }
        }

        public class EntityUpdate : IStateChange
        {
            public int ID { get; set; }

            public override void Deserialize(NetDataReader reader)
            {
                ID = reader.GetInt();
            }

            public override void Serialize(NetDataWriter writer)
            {
                writer.Put(ID);
            }
        } 

        public class HealthUpdate : EntityUpdate
        {
            public int Health { get; set; }
        }

        // @Hack We need to signal to the client that no changes occurred if no changes
        // occurred in a version of the world. This probably won't be used often, so it's 
        // not a big deal, but there's also probably a better way to do this. 
        public class NoChange : IStateChange
        {
            public override void Deserialize(NetDataReader reader)
            {
            }

            public override void Serialize(NetDataWriter writer)
            {
            }
        }
    }
}