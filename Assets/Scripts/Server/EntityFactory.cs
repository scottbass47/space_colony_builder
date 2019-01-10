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
                .Net()
                .build();

            player.AddComponent<ClientComponent>().ID = clientID;
            player.AddComponent<ResourceComponent>().OreAmount.Value = 0;
            return player;
        }

        public static Entity CreateRock(Vector3Int pos)
        {
            var rock = new EntityBuilder(World, Engine, EntityType.ROCK)
                .Net()
                .build();

            rock.AddComponent<MapObjectComponent>().Pos = pos;
            rock.AddComponent<HealthComponent>();
            rock.AddComponent<OreComponent>().Amount.Value = 100;
            rock.AddComponent<SlotComponent>()
                .AddSlot(new Vector2(-0.1f, 0.3f))
                .AddSlot(new Vector2(-0.25f, 0))
                .AddSlot(new Vector2(0, -0.25f))
                .AddSlot(new Vector2(0.3f, -0.1f));
            return rock;
        }

        public static Entity CreateHouse(Vector3Int pos)
        {
            var house = new EntityBuilder(World, Engine, EntityType.HOUSE)
                .Net()
                .build();

            house.AddComponent<MapObjectComponent>().Pos = pos;
            house.AddComponent<HealthComponent>().Health.Value = 100;
            house.AddComponent<HouseComponent>().Set(5);
            return house;
        }

        public static Entity CreateColonist(Level level)
        {
            var colonist = new EntityBuilder(World, Engine, EntityType.COLONIST)
                .Net()
                .build();

            colonist.AddComponent<PositionComponent>().Pos.Value = Vector3.zero;
            colonist.AddComponent<WorkerComponent>();
            colonist.AddComponent<LevelComponent>().Level = level;
            colonist.AddComponent<StateComponent>().State.Value = (int)EntityState.IDLE;
            colonist.AddComponent<StatsComponent>().Set(4, 2);
            colonist.AddComponent<ResidentComponent>();
            return colonist;
        }

        public class EntityBuilder
        {
            private Entity entity;

            public EntityBuilder(WorldStateManager world, Engine engine, EntityType type)
            {
                entity = engine.CreateEntity();
                entity.AddComponent<EntityTypeComponent>().Type = type;
                entity.AddComponent<GlobalComponent>().Set(world, engine);
            }

            // Networked entities
            public EntityBuilder Net()
            {
                entity.AddComponent<NetSpawnComponent>();
                entity.AddComponent<StateUpdateComponent>();
                return this;
            }

            public Entity build()
            {
                return entity;
            }
        }
    }
    
}
