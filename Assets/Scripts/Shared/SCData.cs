using UnityEngine;
using System.Collections;
using LiteNetLib.Utils;
using ECS;

namespace Shared
{
    namespace SCData
    {
        public sealed class Constants
        {
            public static readonly int HOUSE_CAPACITY = 50;
            public static readonly int ORE_AMOUNT = 100;
            public static readonly float COLONIST_SPEED = 2.5f;
            public static readonly int COLONIST_MINE_RATE = 5;
            public static readonly int MAX_WORKERS_PER_PLAYER = 16;
            public static readonly int WORLD_SIZE = 15;
            public static readonly int ORE_SPAWN_CHANCE = 10;
            public static readonly int NUM_INITIAL_COLONISTS = 3;
        }

        // @Cleanup TileAssets and their corresponding IDs need a stronger connection
        public enum TileID
        {
            GROUND,
            ROCK,
            TOWER
        }

        // @Note Reordering this enum will fuck up the EntityPrefabTable in the inspector.
        // Add new entity types to the end of the enum to avoid this.
        public enum EntityType
        {
            NOTHING,
            ORE,
            HOUSE,
            PLAYER,
            COLONIST,
            LANDING_PAD
        }

        public enum NetObjectType
        {
            NOTHING,
            COMPONENT
        }

        public enum EntityState
        {
            IDLE = 0,
            MINING = 1,
            WALKING = 2
        }
    }
}