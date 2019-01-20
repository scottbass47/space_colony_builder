using System.Collections;
using System.Collections.Generic;
using ECS;
using UnityEngine;
using Component = ECS.Component;

namespace Server
{
    public class MapObjectComponent : Component
    {
        public Vector3Int Pos { get; set; } = Vector3Int.zero;

        public override void OnReset()
        {
            Pos = Vector3Int.zero;
        }
    }

}