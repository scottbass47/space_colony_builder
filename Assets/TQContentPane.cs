using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class TQContentPane : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        public void Resize()
        {
            // Count children
            // Set transform height to be n * height
            // where n is # of children and height is height of child

            int children = transform.childCount - 1;
            if (children == 0 || true) return;

            var child = transform.GetChild(0);
            var trans = GetComponent<RectTransform>();
            var rect = trans.rect;
            var width = rect.width;
            var height = children * child.GetComponent<RectTransform>().rect.height;
            trans.sizeDelta = new Vector2(0, height);
        }
    } 
}
