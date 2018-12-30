using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using Server;
using Shared.SCData;

namespace Shared
{
    namespace StateChanges
    {
        public abstract class IStateChange : INetSerializable
        {
            public int Version { get; set; }

            public abstract void Deserialize(NetDataReader reader);
            public abstract void Serialize(NetDataWriter writer);
            public abstract void Apply(WorldStateManager worldStateManager);
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

            public override void Apply(WorldStateManager worldStateManager)
            {
                TileID[][] tiles = worldStateManager.GetTiles();
                tiles[X][Y] = TileID;
            }
        }

        public enum TileChangeType
        {
            CREATE,
            DELETE,
            CHANGE
        }
    }

}