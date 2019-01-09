using ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Server
{
    public class HousingSystem : AbstractSystem
    {
        private List<Entity> houses;

        public HousingSystem() : base(Group.createGroup().All(typeof(NeedHousingComponent)))
        {
            houses = new List<Entity>();
        }

        public override void AddedToEngine()
        {
            Engine.AddGroupListener(Group.createGroup().All(typeof(HouseComponent)), AddHouse, RemoveHouse);
            Engine.AddGroupListener(Group.createGroup().All(typeof(ResidentComponent)), AddResident, RemoveResident);
        }

        private void AddHouse(Entity house)
        {
            houses.Add(house);
        }

        private void RemoveHouse(Entity house)
        {
            houses.Remove(house);
            var houseComp = house.GetComponent<HouseComponent>();

            foreach(var resident in houseComp.Residents)
            {
                DebugUtils.Assert(Engine.IsValid(resident), "Resident has already been removed, yet still belongs to this house.");
                removeHome(house, resident);
            }
        }
        private void AddResident(Entity resident)
        {
            resident.AddComponent<NeedHousingComponent>();
        }
        private void RemoveResident(Entity resident)
        {
            var resComp = resident.GetComponent<ResidentComponent>();
            resComp.House.GetComponent<HouseComponent>().RemoveResident(resident);
        }

        public override void Update(float delta)
        {
            foreach(var entity in Entities)
            {
                var openHouses = houses.FindAll((e) =>
                {
                    var house = e.GetComponent<HouseComponent>();
                    return house.ResidentCount < house.Capacity;
                });

                if(openHouses.Count == 0)
                {
                    //Debug.Log("No open houses!")
                    continue;
                }

                var randomHouse = openHouses[Random.Range(0, openHouses.Count)];

                giveHome(randomHouse, entity);
            }
        }
        private void giveHome(Entity house, Entity resident)
        {
            var houseComp = house.GetComponent<HouseComponent>();
            var resComp = resident.GetComponent<ResidentComponent>();

            houseComp.AddResident(resident);
            resComp.House = house;
            resident.RemoveComponent<NeedHousingComponent>();
        }

        private  void removeHome(Entity house, Entity resident)
        {
            var houseComp = house.GetComponent<HouseComponent>();
            var resComp = resident.GetComponent<ResidentComponent>();

            houseComp.RemoveResident(resident);
            resComp.House = null;
            resident.AddComponent<NeedHousingComponent>();
        }
    }

}
