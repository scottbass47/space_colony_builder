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

namespace Server
{
    public class WorldStateManager
    {
        private TileID[][] tiles;
        private Dictionary<int, List<IStateChange>> worldStateChanges;
        private Engine engine;
        private Level level;

        public int Version { get; private set; }
        public int Size { get; private set; }
        public Engine Engine => engine;
        public Level Level => level;

        private Dictionary<int, Entity> players;

        // Creates a new world state with the specified tile map size
        public WorldStateManager(int size)
        {
            Version = 1;
            Size = size;

            worldStateChanges = new Dictionary<int, List<IStateChange>>();
            players = new Dictionary<int, Entity>();

            engine = new Engine();
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


            List<Vector3Int> rockSpawns;
            Vector3Int landingPadSpawn;
            Vector3Int houseSpawn;
            tiles = WorldGeneration.GenerateWorld(size, Constants.ROCK_SPAWN_CHANCE, out rockSpawns, out landingPadSpawn, out houseSpawn);

            foreach (Vector3Int spawn in rockSpawns)
            {
                var rock = EntityFactory.CreateRock(spawn);
                engine.AddEntity(rock);
            }

            var house = EntityFactory.CreateHouse(houseSpawn);
            engine.AddEntity(house);

            var landingPad = EntityFactory.CreateLandingPad(landingPadSpawn);
            engine.AddEntity(landingPad);

            for (int i = 0; i < Constants.NUM_INITIAL_COLONISTS; i++)
            {
                var colonist = EntityFactory.CreateColonist();
                var spawnPoint = new Vector3(Random.Range(0, Size), Random.Range(0, Size));
                colonist.GetComponent<PositionComponent>().Pos.Value = spawnPoint;
                engine.AddEntity(colonist);
            }
        }

        // Call this every SERVER update so the version numbers get
        // incremented properly.
        public void Update(float delta)
        {
            Version++;
            engine.Update(delta);
        }

        public void ApplyChange(IStateChange change)
        {
            if (!worldStateChanges.ContainsKey(Version))
            {
                worldStateChanges.Add(Version, new List<IStateChange>());
            }
            change.Version = Version;
            worldStateChanges[Version].Add(change);
        }

        // Returns null if the client is up to date with updates
        // OldVersion is the version that the client is up to date with.
        // This means that we have to return all versions in the range
        // (oldVersion, Version)
        public StateChangePacket[] GetDiff(int oldVersion)
        {
            // We don't include the latest Version because the server 
            // could be in the middle of updating with more changes being
            // added to the Version.
            int endVersion = Version - 1;
            int startVersion = oldVersion + 1;

            if (startVersion > endVersion) return null;

            int size = 0; // Total number of changes

            for (int i = startVersion; i <= endVersion; i++)
            {
                // If there are changes, then we just look at how many were recorded
                if (worldStateChanges.ContainsKey(i))
                {
                    size += worldStateChanges[i].Count;
                }
                // Otherwise, we just need to increase the size by one to account for a NoChange
                else
                {
                    size++;
                }
            }

            StateChangePacket[] changes = new StateChangePacket[size];
            int idx = 0;

            for (int i = startVersion; i <= endVersion; i++)
            {
                // If there are changes, then we add those changes 
                if (worldStateChanges.ContainsKey(i))
                {
                    var versionChanges = worldStateChanges[i];
                    int numChanges = versionChanges.Count;
                    DebugUtils.Assert(numChanges != 0);
                    byte changeNum = 1;

                    foreach (IStateChange change in versionChanges)
                    {
                        changes[idx++] = new StateChangePacket
                        {
                            Version = i,
                            ChangeNumber = changeNum++,
                            TotalChanges = (byte)numChanges,
                            Change = change
                        };
                    }
                }
                // Otherwise, we just need to increase the size by one to account for a NoChange
                else
                {
                    changes[idx++] = new StateChangePacket
                    {
                        Version = i,
                        ChangeNumber = 1,
                        TotalChanges = 1,
                        Change = new NoChange()
                    };
                }
            }

            return changes;
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

            var taskFac = player.GetComponent<TaskFactoryComponent>().Factory;

            var level = player.GetComponent<LevelComponent>().Level;
            var allRocks = level.GetObjects((entity) => 
            {
                return entity.GetComponent<EntityTypeComponent>().Type == EntityType.ROCK;
            });
            var ids = new List<int>();
            allRocks.ForEach((rock) => ids.Add(rock.ID));

            var task = taskFac.CreateMiningTask(ids, player);
            var taskQueue = player.GetComponent<TaskQueueComponent>().Tasks;
            taskQueue.AddTask(task);
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