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
        private Dictionary<int, List<IStateChange>> worldStateChanges;
        private Engine engine;
        private Level level;
        private NetObjectManager nom;

        public int Version { get; private set; }
        public int Size { get; private set; }
        public Engine Engine => engine;
        public Level Level => level;

        private Dictionary<int, Entity> players;

        private NetObject testObj;
        private NetObject testChildObj;
        private NetObject testChildObj2;

        // Creates a new world state with the specified tile map size
        public WorldStateManager(SCServer server, int size)
        {
            this.server = server;
            Version = 1;
            Size = size;

            worldStateChanges = new Dictionary<int, List<IStateChange>>();
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

            List<Vector3Int> rockSpawns;
            Vector3Int landingPadSpawn;
            Vector3Int houseSpawn;
            tiles = WorldGeneration.GenerateWorld(size, Constants.ROCK_SPAWN_CHANCE, out rockSpawns, out landingPadSpawn, out houseSpawn);

            //foreach (Vector3Int spawn in rockSpawns)
            //{
            //    var rock = EntityFactory.CreateRock(spawn);
            //    engine.AddEntity(rock);
            //}

            //var house = EntityFactory.CreateHouse(houseSpawn);
            //engine.AddEntity(house);

            //var landingPad = EntityFactory.CreateLandingPad(landingPadSpawn);
            //engine.AddEntity(landingPad);

            //for (int i = 0; i < Constants.NUM_INITIAL_COLONISTS; i++)
            //{
            //    var colonist = EntityFactory.CreateColonist();
            //    var spawnPoint = new Vector3(Random.Range(0, Size), Random.Range(0, Size));
            //    colonist.GetComponent<PositionComponent>().Pos.Value = spawnPoint;
            //    engine.AddEntity(colonist);
            //}

        }

        // TEMP
        private float elapsed;

        // Call this every SERVER update so the version numbers get
        // incremented properly.
        public void Update(float delta)
        {
            Version++;
            if (testObj != null && testObj.Alive)
            {
                elapsed += delta;
                //testObj.Sync();
                if(testChildObj.Alive)  testChildObj.Sync();
                if(testChildObj2.Alive) testChildObj2.Sync();

                // After 3 seconds remove the object
                if(elapsed >= 3 && testChildObj.Alive)
                {
                    nom.RemoveNetObject(testChildObj.ID);
                }
            }
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
            // Create test with synced object
            testObj = nom.CreateNetObject();
            testObj.Type = NetObjectType.TEST;
            testObj.OnUpdate = () => new TestUpdate { Value = Random.Range(0, 100) };

            testChildObj = nom.CreateNetObject();
            testChildObj.Type = NetObjectType.TEST_CHILD;
            testChildObj.NetMode = NetMode.LATEST;
            testChildObj.OnUpdate = () => new TestUpdate { Value = 69 };

            testChildObj2 = nom.CreateNetObject();
            testChildObj2.Type = NetObjectType.TEST_CHILD;
            testChildObj2.NetMode = NetMode.LATEST;
            testChildObj2.OnUpdate = () => new TestUpdate { Value = 71 };

            NetObject.BindParentChild(testObj, testChildObj);
            NetObject.BindParentChild(testChildObj, testChildObj2);

            // Can we add out of order? 
            nom.AddNetObject(testObj);
            nom.AddNetObject(testChildObj);
            nom.AddNetObject(testChildObj2);

            var player = EntityFactory.CreatePlayer(clientID);
            players.Add(clientID, player);
            Engine.AddEntity(player);

            //var taskFac = player.GetComponent<TaskFactoryComponent>().Factory;

            //var level = player.GetComponent<LevelComponent>().Level;
            //var allRocks = level.GetObjects((entity) => 
            //{
            //    return entity.GetComponent<EntityTypeComponent>().Type == EntityType.ROCK;
            //});
            //var ids = new List<int>();
            //allRocks.ForEach((rock) => ids.Add(rock.ID));

            //var task = taskFac.CreateMiningTask(ids, player);
            //var taskQueue = player.GetComponent<TaskQueueComponent>().Tasks;
            //taskQueue.AddTask(task);
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