using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECS;
using Server.NetObjects;
using Shared;
using UnityEngine;
using Utils;

namespace Server
{
    public class HouseComponent : NetComponent
    {
        public int ResidentCount => Residents.Count;
        public int Capacity { get; set; }
        private List<Entity> residentList;
        public List<Entity> Residents => residentList;

        public HouseComponent()
        {
            residentList = new List<Entity>();
        }

        public HouseComponent Set(int capacity)
        {
            Capacity = capacity;
            return this;
        }

        protected override void OnInit(NetObject netObj)
        {
            residentList.Clear();
            netObj.NetMode = NetMode.IMPORTANT;
            netObj.OnUpdate = () =>
            {
                int[] ids = new int[residentList.Count];
                for (int i = 0; i < residentList.Count; i++)
                {
                    var res = residentList[i] as NetEntity;
                    ids[i] = res.NetObject.ID;
                }
                return new HouseUpdate { Residents = ids };
            };
        }

        public void AddResident(Entity resident)
        {
            if (ResidentCount == Capacity)
            {
                Debug.Log("Failed to add resident: house is already at max capacity!");
                return;
            }
            residentList.Add(resident);
            net.Sync();
        }

        public void RemoveResident(Entity resident)
        {
            if (ResidentCount == 0)
            {
                Debug.Log("Failed to remove resident: house is empty!");
                return;
            }
            DebugUtils.Assert(residentList.Remove(resident));
            net.Sync();
        }
    }
}
