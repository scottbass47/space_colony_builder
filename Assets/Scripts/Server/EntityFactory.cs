using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECS;
using UnityEngine;

namespace Server
{
    public sealed class EntityFactory
    {
        public static Engine Engine;

        public static Entity CreateRock(Vector3Int pos)
        {
            Entity rock = Engine.CreateEntity();
            rock.AddComponent<MapObjectComponent>().Pos = pos;
            rock.AddComponent<HealthComponent>();
            rock.AddComponent<StateUpdateComponent>();
            return rock;
        }

        public static Entity CreateHouse()
        {
            Entity house = Engine.CreateEntity();
            house.AddComponent<MapObjectComponent>();
            house.AddComponent<HealthComponent>();
            return house;
        }
    }
}
