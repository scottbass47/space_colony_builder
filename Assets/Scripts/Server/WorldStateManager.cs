using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Shared.SCPacket;
using Shared.StateChanges;
using Shared.SCData;

namespace Server
{
    public class WorldStateManager
    {
        private TileID[][] tiles;
        //private List<Entity> entities;
        private List<IStateChange> worldStateChanges;

        public int Version { get; private set; }
        public int Size { get; private set; }

        // Creates a new world state with the specified tile map size
        public WorldStateManager(int size)
        {
            Version = 1;
            Size = size;
            //entities = new List<Entity>();
            worldStateChanges = new List<IStateChange>();

            tiles = WorldGeneration.GenerateWorld(size, 10);
        }

        // Call this every SERVER update so the version numbers get
        // incremented properly.
        public void Update()
        {
            Version++;
        }

        public void ApplyChange(IStateChange change)
        {
            Debug.Log($"[Server] applying change with version number {Version}.");
            change.Version = Version;
            worldStateChanges.Add(change);
            change.Apply(this);
        }


        // Returns null if the client is up to date with updates
        public StateChangesPacket GetDiff(int oldVersion)
        {
            // @Performance
            // We loop through the entire history of state changes until
            // we find one with a version that we don't have yet.

            int i = 0; // Index of first state change with version > oldVersion
            while (i < worldStateChanges.Count && worldStateChanges[i].Version <= oldVersion) i++;

            // No new changes, no need to send a packet back
            if (i == worldStateChanges.Count) return null;

            IStateChange[] changes = new IStateChange[worldStateChanges.Count - i];

            for (int j = i; j < worldStateChanges.Count; j++)
            {
                changes[j - i] = worldStateChanges[i];
            }

            return new StateChangesPacket { Version = Version, Changes = changes };
        }

        public TileID[][] GetTiles()
        {
            return tiles;
        }
    }

}