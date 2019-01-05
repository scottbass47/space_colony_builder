using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Shared.SCPacket;
using Shared.StateChanges;
using Shared.SCData;
using ECS;
using Utils;

namespace Server
{
    public class WorldStateManager
    {
        private TileID[][] tiles;
        //private List<Entity> entities;
        private Dictionary<int, List<IStateChange>> worldStateChanges;
        private Engine engine;

        public int Version { get; private set; }
        public int Size { get; private set; }
        public Engine Engine => engine;

        // Creates a new world state with the specified tile map size
        public WorldStateManager(int size)
        {
            engine = new Engine();
            EntityFactory.Engine = engine;

            engine.AddSystem(new RandomDeleteSystem(this));
            engine.AddSystem(new RandomHealthSystem(this));
            engine.AddSystem(new StateChangeEmitterSystem(this));

            Version = 1;
            Size = size;

            worldStateChanges = new Dictionary<int, List<IStateChange>>();

            List<Vector3Int> rockSpawns;
            tiles = WorldGeneration.GenerateWorld(size, 10, out rockSpawns);

            foreach (Vector3Int spawn in rockSpawns)
            {
                var rock = EntityFactory.CreateRock();
                engine.AddEntity(rock);
                ApplyChange(new EntitySpawn { ID = rock.ID, EntityType = EntityType.ROCK, Pos = spawn });
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

        public TileID[][] GetTiles()
        {
            return tiles;
        }
    }
}