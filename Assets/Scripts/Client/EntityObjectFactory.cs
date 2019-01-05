﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Shared.SCData;
using Shared.StateChanges;

namespace Client
{
    public class EntityObjectFactory : MonoBehaviour
    {
        public GameObject CreateEntityObject(EntityType type, EntitySpawn spawn)
        {
            GameObject go = null;
            switch (type)
            {
                case EntityType.ROCK:
                    go = CreateRock(spawn);
                    break;
            }
            var eo = go.GetComponent<EntityObject>();
            eo.ID = spawn.ID;
            eo.AddToEntityManager();
            return go;
        }

        private GameObject CreateRock(EntitySpawn spawn)
        {
            // Create the rock from prefab    
            var prefabTable = Game.Instance.PrefabTable;
            var go = Instantiate(prefabTable.GetPrefab(EntityType.ROCK));

            Vector3 pos = spawn.Pos;
            go.GetComponent<TilemapObject>().Pos = new Vector3Int((int)pos.x, (int)pos.y, (int)pos.z);

            return go;
        }
    }
}