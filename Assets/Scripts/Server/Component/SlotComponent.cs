using ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utils;
using Component = ECS.Component;

namespace Server
{
    public class SlotComponent : Component
    {
        private List<Vector2> slotLocations;
        private Dictionary<int, Entity> slots;
        //public List<Vector2> Slots => slotLocations;

        public SlotComponent()
        {
            slotLocations = new List<Vector2>();
            slots = new Dictionary<int, Entity>();
        }

        public SlotComponent AddSlot(Vector2 pos)
        {
            slotLocations.Add(pos);
            slots.Add(slots.Count, null);
            return this;
        } 

        public bool HasOpenSlot()
        {
            for(int i = 0; i < slots.Count; i++)
            {
                DebugUtils.Assert(slots.ContainsKey(i));
                if (slots[i] == null) return true;
            }
            return false;
        }

        public int NextSlot()
        {
            DebugUtils.Assert(HasOpenSlot(), "No open slots");
            for(int i = 0; i < slots.Count; i++)
            {
                if(slots[i] == null)
                {
                    return i;
                }
            }
            DebugUtils.Assert(false, "No open slots");
            return -1; // Should never get hit
        }

        public Vector2 GetSlotLocation(int slotNum)
        {
            return slotLocations[slotNum];
        }

        public Entity GetEntity(int slotNum)
        {
            return slots[slotNum];
        }

        public void AddEntityToSlot(int slotNum, Entity entity)
        {
            DebugUtils.Assert(slots[slotNum] == null, $"There is already an entity at slot {slotNum}");
            slots[slotNum] = entity; 
        }

        // Adds the entity to the next open slot and then returns
        // the slot index.
        public int AddEntityToNextSlot(Entity entity)
        {
            int slotNum = NextSlot();
            AddEntityToSlot(slotNum, entity);
            return slotNum;
        }

        public void RemoveFromSlot(int slotNum)
        {
            DebugUtils.Assert(slots[slotNum] != null, $"There is no entity at slot {slotNum}");
            slots[slotNum] = null;
        }

        public void Reset()
        {
            slotLocations.Clear();
            slots.Clear();
        }
    }
}
