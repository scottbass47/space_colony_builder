using Shared;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Client
{
    public class TaskQueueTest : MonoBehaviour
    {
        public TextMeshProUGUI taskQueueUI;

        // Start is called before the first frame update
        void Awake()
        {
            taskQueueUI = GameObject.Find("TaskQueue").GetComponent<TextMeshProUGUI>();
            var eo = GetComponent<EntityObject>();
            eo.AddUpdateListener<TaskQueueUpdate>((queue) =>
            {
                taskQueueUI.SetText(queue.QueueText.ToUpper());
            });
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}