using ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Component = ECS.Component;

namespace Server
{
    public class SlotComponent : Component
    {
        private List<Vector2> slots;
        public List<Vector2> Slots => slots;

        public SlotComponent()
        {
            slots = new List<Vector2>();
        }

        public SlotComponent AddSlot(Vector2 pos)
        {
            slots.Add(pos);
            return this;
        } 
        
        public void Reset()
        {
            slots.Clear();
        }
    }
}
