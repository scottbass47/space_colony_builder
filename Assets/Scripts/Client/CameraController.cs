using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class CameraController : MonoBehaviour
    {
        public float ScrollSpeed = 2;
        public float mouseSensitivity = .01f;

        private Vector3 lastPosition;
        // Update is called once per frame
        void Update()
        {
            float scrollVal = Input.GetAxisRaw("Mouse ScrollWheel");
            if (scrollVal > 0)
            {
                // zoom in
                Camera.main.orthographicSize -= Time.deltaTime * ScrollSpeed;
            }
            else if (scrollVal < 0)
            {
                // zoom out
                Camera.main.orthographicSize += Time.deltaTime * ScrollSpeed;
            }

            //panning controls
            if (Input.GetMouseButtonDown(1))
            {
                lastPosition = Input.mousePosition;
            }

            if (Input.GetMouseButton(1))
            {
                var delta = Input.mousePosition - lastPosition;
                transform.Translate(-delta.x * mouseSensitivity, -delta.y * mouseSensitivity, -delta.z * mouseSensitivity);
                lastPosition = Input.mousePosition;
            }
        }
    }
}