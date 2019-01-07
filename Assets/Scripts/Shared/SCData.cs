﻿using UnityEngine;
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
            ROCK,
            HOUSE,
            PLAYER,
            COLONIST
        }

        public enum EntityState
        {
            IDLE,
            MINING,
            WALKING
        }
    }
}