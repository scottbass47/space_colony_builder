using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Shared.SCData;
using Shared.StateChanges;
using Utils;

namespace Client
{
    public class EntityObjectFactory : MonoBehaviour
    {
        private EntityPrefabTable prefabTable;

        private void Start()
        {
            prefabTable = Game.Instance.PrefabTable;
        }

        public GameObject CreateEntityObject(EntityType type)
        {
            GameObject go = null;
            switch (type)
            {
                case EntityType.ROCK:
                    go = CreateRock();
                    break;
                case EntityType.HOUSE:
                    go = CreateHouse();
                    break;
                case EntityType.COLONIST:
                    go = CreateColonist();
                    break;
                case EntityType.PLAYER:
                    go = CreatePlayer();
                    break;
                case EntityType.LANDING_PAD:
                    go = CreateLandingPad();
                    break;
                case EntityType.NOTHING:
                    DebugUtils.Assert(false, "Can't create Entity of type NOTHING.");
                    break;
            }
            return go;
        }

        private GameObject CreateRock()
        {
            // Create the rock from prefab    
            var go = Instantiate(prefabTable.GetPrefab(EntityType.ROCK));

            //Vector3 pos = spawn.Pos;
            //go.GetComponent<TilemapObject>().Pos = new Vector3Int((int)pos.x, (int)pos.y, (int)pos.z);

            return go;
        }

        private GameObject CreateHouse()
        {  
            var go = Instantiate(prefabTable.GetPrefab(EntityType.HOUSE));

            //Vector3 pos = spawn.Pos;
            //go.GetComponent<TilemapObject>().Pos = new Vector3Int((int)pos.x, (int)pos.y, (int)pos.z);

            return go;
        }

        private GameObject CreateColonist()
        {
            var go = Instantiate(prefabTable.GetPrefab(EntityType.COLONIST));

            //var iso = go.GetComponent<IsometricPosition>();
            //iso.Position = spawn.Pos;
            //iso.Position.z = 1;

            return go;
        }

        private GameObject CreatePlayer()
        {
            var go = Instantiate(prefabTable.GetPrefab(EntityType.PLAYER));
            return go;
        }

        private GameObject CreateLandingPad()
        {
            var go = Instantiate(prefabTable.GetPrefab(EntityType.LANDING_PAD));
            //Vector3 pos = spawn.Pos;
            //go.GetComponent<TilemapObject>().Pos = new Vector3Int((int)pos.x, (int)pos.y, (int)pos.z);
            return go;
        }
    }
}
