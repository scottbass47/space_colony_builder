using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    [RequireComponent(typeof(TilemapObject))]
    public class RandomMove : MonoBehaviour
    {
        private TilemapObject to;

        void Start()
        {
            to = GetComponent<TilemapObject>();
        }

        // Update is called once per frame
        void Update()
        {
            to.Pos = new Vector3Int(Random.Range(0, 20), Random.Range(0, 20), 0);
        }
    }

}