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
            EntityFactory.World = this;
            EntityFactory.Engine = engine;

            engine.AddSystem(new NetSpawnSystem());
            engine.AddSystem(new RequestProcessingSystem());    
            engine.AddSystem(new WorkerSystem());    
            engine.AddSystem(new DeathSystem());    
            engine.AddSystem(new StateChangeEmitterSystem());

            level = new Level(this);

            List<Vector3Int> rockSpawns;
            tiles = WorldGeneration.GenerateWorld(size, 10, out rockSpawns);

            foreach (Vector3Int spawn in rockSpawns)
            {
                var rock = EntityFactory.CreateRock(spawn);
                engine.AddEntity(rock);
            }

            for (int i = 0; i < 10; i++)
            {
                //var dest = rockSpawns[Random.Range(0, rockSpawns.Count)];

                var colonist = EntityFactory.CreateColonist(level);
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