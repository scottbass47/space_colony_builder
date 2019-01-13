using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Utils
{
    public sealed class MathUtils
    {
        public static Vector3 IsoToWorld(Vector3 world)
        {
            Vector3 iso = new Vector3();
            iso.x = (world.x - world.y) * 0.5f;
            iso.y = (world.x + world.y) * 0.25f + world.z * 0.5f;
            iso.z = world.z;
            return iso;
        }
    }
}
