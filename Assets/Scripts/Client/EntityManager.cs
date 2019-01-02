﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shared;
using Shared.SCPacket;
using Shared.StateChanges;
using Shared.SCData;


namespace Client
{
    public class EntityManager : MonoBehaviour
    {

        public GameObject RockPrefab;

        private Dictionary<int, GameObject> entities;

        // Start is called before the first frame update
        void Start()
        {
            entities = new Dictionary<int, GameObject>();
        }

        public void AddEntity(int id, GameObject obj)
        {
            entities.Add(id, obj);
        }

        public void RemoveEntity(int id)
        {
            entities.Remove(id);
        }

        public GameObject GetEntity(int id)
        {
            return entities[id];
        }

        //public void SpawnEntity(EntitySpawn spawn)
        //{
        //    GameObject go = null;
        //    switch (spawn.EntityType)
        //    {
        //        case EntityType.ROCK:
        //            go = Instantiate(RockPrefab);
        //            Vector3 pos = spawn.Pos;
        //            go.GetComponent<TilemapObject>().Pos = new Vector3Int((int)pos.x, (int)pos.y, (int)pos.z);
        //            Game.Instance.Client.ground.GetComponent<MapObjectRenderer>().AddMapObject(go);
        //            break;
        //    }
        //    AddEntity(spawn.ID, go);
        //}

        //public void RemoveEntity(EntityRemove remove)
        //{
        //    var id = remove.ID;
        //    var go = entities[id];

        //    var mapObj = go.GetComponent<TilemapObject>();
        //    if(mapObj != null)
        //    {
        //        Game.Instance.Client.ground.GetComponent<MapObjectRenderer>().RemoveMapObject(go);
        //        Destroy(go);
        //    }
        //    RemoveEntity(id);
        //}
    }
}