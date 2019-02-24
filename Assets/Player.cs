using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class Player : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Game.Instance.ClientPlayer = gameObject;
        }
    } 
}
