using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class CameraController : MonoBehaviour
    {
        public float ScrollSpeed = 2;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.Minus))
            {
                // zoom in
                Camera.main.orthographicSize -= Time.deltaTime * ScrollSpeed;
            }
            else if (Input.GetKey(KeyCode.Equals))
            {
                // zoom out
                Camera.main.orthographicSize += Time.deltaTime * ScrollSpeed;
            }
        }
    }
}