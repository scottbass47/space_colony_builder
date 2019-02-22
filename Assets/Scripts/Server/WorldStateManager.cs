using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Shared.SCPacket;
using Shared.StateChanges;
using Shared.SCData;
using ECS;
using Utils;
using Shared;
using Server.Job;
using Random = UnityEngine.Random;
using Server.NetObjects;

namespace Server
{
    public class WorldStateManager
    {
        private SCServer server;
        private TileID[][] tiles;
        private Engine engine;
        private Level level;
        private NetObjectManager nom;

        //public int Version { get; private set; }
        public int Size { get; private set; }
        public Engine Engine => engine;
        public Level Level => level;

        private Dictionary<int, Entity> players;

        private float waitTime = 0f;
        private float elapsed;

        // Creates a new world state with the specified tile map size
        public WorldStateManager(SCServer server, int size)
        {
            this.server = server;
            Size = size;

            players = new Dictionary<int, Entity>();
            nom = new NetObjectManager(server);

            engine = new NetEngine(nom);
            level = new Level(this);

            EntityFactory.World = this;
            EntityFactory.Engine = engine;
            EntityFactory.Level = Level;

            engine.AddSystem(new RequestProcessingSystem());    
            engine.AddSystem(new NetSpawnSystem());
            engine.AddSystem(new TaskProcessingSystem());
            engine.AddSystem(new HiringSystem());    
            engine.AddSystem(new WorkerSystem());
            engine.AddSystem(new HousingSystem());
            engine.AddSystem(new DeathSystem());    
            engine.AddSystem(new StateChangeEmitterSystem());
        }

        // Called once after both clients are connected
        public void Init()
        {
            List<Vector3Int> oreSpawns;
            Vector3Int landingPadSpawn;
            Vector3Int houseSpawn;
            tiles = WorldGeneration.GenerateWorld(Size, Constants.ORE_SPAWN_CHANCE, out oreSpawns, out landingPadSpawn, out houseSpawn);

            server.SendWorldData();

            foreach (Vector3Int spawn in oreSpawns)
            {
                var ore = EntityFactory.CreateOre(spawn);
                engine.AddEntity(ore);
            }

            var house = EntityFactory.CreateHouse(houseSpawn);
            engine.AddEntity(house);

            var landingPad = EntityFactory.CreateLandingPad(landingPadSpawn);
            engine.AddEntity(landingPad);

            for (int i = 0; i < Constants.NUM_INITIAL_COLONISTS; i++)
            {
                var spawnPoint = new Vector3(Random.Range(0, Size), Random.Range(0, Size));
                var colonist = EntityFactory.CreateColonist(spawnPoint);
                engine.AddEntity(colonist);
            }

            var player = players[0];
            var taskFac = player.GetComponent<TaskFactoryComponent>().Factory;

            var level = player.GetComponent<LevelComponent>().Level;
            var allOre = level.GetObjects((entity) =>
            {
                return entity.GetComponent<EntityTypeComponent>().Type == EntityType.ORE;
            });
            var ids = new List<int>();
            allOre.ForEach((ore) => ids.Add(ore.ID));

            var taskQueue = player.GetComponent<TaskQueueComponent>().Tasks;
            foreach(var id in ids)
            {
                var task = taskFac.CreateMiningTask(new List<int>{ id }, player);
                taskQueue.AddTask(task);
            }
        }

        // Call this every SERVER update so the version numbers get
        // incremented properly.
        public void Update(float delta)
        {
            elapsed += delta;
            if (elapsed < waitTime) return;
            engine.Update(delta);
        }

        public void AddRequest(int clientID, ClientRequest request)
        {
            var player = GetPlayer(clientID);
            var client = player.GetComponent<ClientComponent>();
            client.Requests.Enqueue(request);
        }

        public void AddPlayer(int clientID)
        {
            var player = EntityFactory.CreatePlayer(clientID);
            players.Add(clientID, player);
            Engine.AddEntity(player);

        }

        public Entity GetPlayer(int clientID)
        {
            return players[clientID];
        }

        public TileID[][] GetTiles()
        {
            return tiles;
        }
    }
}