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
        public static WorldStateManager World;
        public static Engine Engine;

        public static Entity CreatePlayer(int clientID)
        {
            var player = new EntityBuilder(World, Engine, EntityType.PLAYER)
                .build();

            player.AddComponent<ClientComponent>().ID = clientID;
            player.AddComponent<ResourceComponent>().OreAmount.Value = 0;
            return player;
        }

        public static Entity CreateRock(Vector3Int pos)
        {
            var rock = new EntityBuilder(World, Engine, EntityType.ROCK)
                .build();

            rock.AddComponent<MapObjectComponent>().Pos = pos;
            rock.AddComponent<HealthComponent>();
            rock.AddComponent<OreComponent>().Amount.Value = 200;
            return rock;
        }

        public static Entity CreateHouse(Vector3Int pos)
        {
            var house = new EntityBuilder(World, Engine, EntityType.HOUSE)
                .build();

            house.AddComponent<MapObjectComponent>().Pos = pos;
            house.AddComponent<HealthComponent>().Health.Value = 100;
            return house;
        }

        public static Entity CreateColonist(Level level)
        {
            var colonist = new EntityBuilder(World, Engine, EntityType.COLONIST)
                .build();

            colonist.AddComponent<PositionComponent>().Pos.Value = Vector3.zero;
            colonist.AddComponent<WorkerComponent>();
            colonist.AddComponent<LevelComponent>().Level = level;
            colonist.AddComponent<StateComponent>().State.Value = (int)EntityState.IDLE;
            colonist.AddComponent<StatsComponent>().Set(1000, 5);
            return colonist;
        }

        public class EntityBuilder
        {
            private Entity entity;

            public EntityBuilder(WorldStateManager world, Engine engine, EntityType type)
            {
                entity = engine.CreateEntity();
                entity.AddComponent<EntityTypeComponent>().Type = type;
                entity.AddComponent<StateUpdateComponent>();
                entity.AddComponent<GlobalComponent>().Set(world, engine);
            }

            public EntityBuilder NetSpawn()
            {
                return this;
            }

            public Entity build()
            {
                return entity;
            }
        }
    }
    
}
