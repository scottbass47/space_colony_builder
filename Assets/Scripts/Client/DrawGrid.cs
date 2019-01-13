using Shared.SCData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Client
{
    public class DrawGrid : MonoBehaviour
    {
        public GameObject line;
        // Start is called before the first frame update
        void Start()
        {
            for(int i = 0; i < Constants.WORLD_SIZE + 1; i++)
            {
                Vector3[] points = new Vector3[2];
                points[0] = MathUtils.IsoToWorld(new Vector3(-0.5f, -0.5f + i, 1));
                points[0].z = -1;
                points[1] = MathUtils.IsoToWorld(new Vector3(Constants.WORLD_SIZE - 0.5f, -0.5f + i, 1));
                points[1].z = -1;

                var go = Instantiate(line);
                go.transform.parent = transform;
                var l = go.GetComponent<LineRenderer>();
                l.positionCount = 2;
                l.SetPositions(points);
                l.material.color = Color.red;

                points = new Vector3[2];
                points[0] = MathUtils.IsoToWorld(new Vector3(-0.5f + i, -0.5f, 1));
                points[0].z = -1;
                points[1] = MathUtils.IsoToWorld(new Vector3(-0.5f + i, Constants.WORLD_SIZE -0.5f, 1));
                points[1].z = -1;

                go = Instantiate(line);
                go.transform.parent = transform;
                l = go.GetComponent<LineRenderer>();
                l.positionCount = 2;
                l.SetPositions(points);
                l.material.color = Color.red;
            }

        }
    }

}