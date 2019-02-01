using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shared.SCData;
using Utils;

namespace Client
{
    public class EntityPrefabTable : MonoBehaviour
    {
        private Dictionary<EntityType, GameObject> prefabTable;
        public TypePrefab[] list;

        private void Start()
        {
            prefabTable = new Dictionary<EntityType, GameObject>();
            foreach(var pair in list)
            {
                prefabTable.Add(pair.type, pair.prefab);
            }
        }

        public GameObject GetPrefab(EntityType type)
        {
            GameObject go;
            DebugUtils.Assert(prefabTable.TryGetValue(type, out go), $"Entity of type {type} not found in prefab table.");
            return go;
        }

    }

    [Serializable]
    public struct TypePrefab
    {
        public EntityType type;
        public GameObject prefab; 
    }

}