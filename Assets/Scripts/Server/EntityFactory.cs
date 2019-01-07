using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECS;
using Shared.SCData;
using UnityEngine;

namespace Server
{
    public sealed class EntityFactory
    {
        public static Engine Engine;

        public static Entity CreatePlayer(int clientID)
        {
            Entity player = Engine.CreateEntity();
            player.AddComponent<EntityTypeComponent>().Type = EntityType.PLAYER;
            player.AddComponent<ClientComponent>().ID = clientID;
            return player; 
        }

        public static Entity CreateRock(Vector3Int pos)
        {
            Entity rock = Engine.CreateEntity();
            rock.AddComponent<EntityTypeComponent>().Type = EntityType.ROCK;
            rock.AddComponent<MapObjectComponent>().Pos = pos;
            rock.AddComponent<HealthComponent>();
            rock.AddComponent<StateUpdateComponent>();
            return rock;
        }

        public static Entity CreateHouse(Vector3Int pos)
        {
            Entity house = Engine.CreateEntity();
            house.AddComponent<EntityTypeComponent>().Type = EntityType.HOUSE;
            house.AddComponent<MapObjectComponent>().Pos = pos;
            house.AddComponent<HealthComponent>();
            return house;
        }

        public static Entity CreateColonist()
        {
            Entity colonist = Engine.CreateEntity();
            colonist.AddComponent<EntityTypeComponent>().Type = EntityType.COLONIST;
            return colonist;
        }
    }
}
