using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECS;
using Shared;
using UnityEngine;
using Utils;

namespace Server
{
    public class HouseComponent : NetComponent
    {
        public NetValue<int> ResidentCount { get; set; }
        public int Capacity { get; set; }
        private List<Entity> residentList;
        public List<Entity> Residents => residentList;

        public HouseComponent()
        {
            residentList = new List<Entity>();

            ResidentCount = new NetValue<int>();

            AddNetValue(ResidentCount);
        }

        public HouseComponent Set(int capacity)
        {
            Capacity = capacity;
            return this;
        }

        public void AddResident(Entity resident)
        {
            if(ResidentCount == Capacity)
            {
                Debug.Log("Failed to add resident: house is already at max capacity!");
                return;
            }
            residentList.Add(resident);
            ResidentCount.Value += 1;
        }            

        public void RemoveResident(Entity resident)
        {
            if(ResidentCount == 0)
            {
                Debug.Log("Failed to remove resident: house is empty!");
                return;
            }
            DebugUtils.Assert(residentList.Remove(resident));
            ResidentCount.Value -= 1;
        }            

        public override SCUpdate CreateChange()
        {
            int[] ids = new int[residentList.Count];
            for (int i = 0; i < residentList.Count; i++)
            {
                ids[i] = residentList[i].ID;
            }
            return new HouseUpdate { Residents = ids };
        }

        public override void OnResetTemp()
        {
            Capacity = 0;
            residentList.Clear();
            ResidentCount = new NetValue<int>();

            AddNetValue(ResidentCount);
        }
    }
}
