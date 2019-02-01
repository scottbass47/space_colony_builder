﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECS;
using Server.NetObjects;
using Server.Tasks;
using Shared.SCData;
using UnityEngine;
using Utils;

namespace Server
{
    public sealed class EntityFactory
    {
        public static WorldStateManager World;
        public static Engine Engine;
        public static Level Level;

        public static Entity CreatePlayer(int clientID)
        {
            var player = new EntityBuilder(World, Engine, Level, EntityType.PLAYER)
                .Net()
                .build();

            player.AddComponent<ClientComponent>().ID = clientID;
            player.AddComponent<ResourceComponent>().OreAmount = 0;
            player.AddComponent<TaskFactoryComponent>().Set(new TaskFactory(Engine));
            player.AddComponent<TaskQueueComponent>();
            player.AddComponent<OwnedWorkersComponent>().Set(maxWorkers: Constants.MAX_WORKERS_PER_PLAYER);
            return player;
        }

        public static Entity CreateRock(Vector3Int pos)
        {
            var rock = new EntityBuilder(World, Engine, Level, EntityType.ROCK)
                .Net()
                .build();

            rock.AddComponent<MapObjectComponent>().Pos = pos;
            rock.AddComponent<HealthComponent>();
            rock.AddComponent<OreComponent>().Amount = Constants.ROCK_ORE;
            rock.AddComponent<SlotComponent>()
                .AddSlot(new Vector2(-0.1f, 0.3f))
                .AddSlot(new Vector2(-0.25f, 0))
                .AddSlot(new Vector2(0, -0.25f))
                .AddSlot(new Vector2(0.3f, -0.1f));
            return rock;
        }

        public static Entity CreateHouse(Vector3Int pos)
        {
            var house = new EntityBuilder(World, Engine, Level, EntityType.HOUSE)
                .Net()
                .build();

            house.AddComponent<MapObjectComponent>().Pos = pos;
            house.AddComponent<HealthComponent>().Health = 100;
            house.AddComponent<HouseComponent>().Set(Constants.HOUSE_CAPACITY);
            return house;
        }

        public static Entity CreateColonist()
        {
            var colonist = new EntityBuilder(World, Engine, Level, EntityType.COLONIST)
                .Net()
                .build();

            colonist.AddComponent<PositionComponent>().Pos = Vector3.zero;
            colonist.AddComponent<WorkerComponent>();
            colonist.AddComponent<NotOwnedComponent>();
            colonist.AddComponent<StateComponent>().State = (int)EntityState.IDLE;
            colonist.AddComponent<StatsComponent>().Set(walkSpeed: Constants.COLONIST_SPEED, mineSpeed: Constants.COLONIST_MINE_RATE);
            colonist.AddComponent<ResidentComponent>();
            return colonist;
        }

        public static Entity CreateLandingPad(Vector3Int pos)
        {
            var house = new EntityBuilder(World, Engine, Level, EntityType.LANDING_PAD)
                .Net()
                .build();

            house.AddComponent<MapObjectComponent>().Pos = pos;
            return house;
        }

        public class EntityBuilder
        {
            private Entity entity;

            public EntityBuilder(WorldStateManager world, Engine engine, Level level, EntityType type)
            {
                DebugUtils.Assert(engine is NetEngine, "Engine must be of type NetEngine.");
                entity = engine.CreateEntity();
                entity.AddComponent<EntityTypeComponent>().Type = type;
                entity.AddComponent<GlobalComponent>().Set(world, engine as NetEngine);
                entity.AddComponent<LevelComponent>().Level = level;
            }

            // Networked entities
            public EntityBuilder Net()
            {
                //entity.AddComponent<NetSpawnComponent>();
                entity.AddComponent<StateUpdateComponent>();

                var netEntity = entity as NetEntity;
                netEntity.NetObject.EntityType = netEntity.GetComponent<EntityTypeComponent>().Type;
                netEntity.AddToNetObjectManager();
                return this;
            }

            public Entity build()
            {
                return entity;
            }
        }

    }
    
}
