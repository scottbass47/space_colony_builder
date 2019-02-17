using Shared;
using Shared.SCData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace Client
{
    public class TaskQueueController : MonoBehaviour
    {
        public TextMeshProUGUI taskQueueUI;

        private Dictionary<int, TaskObj> taskDict;

        private void Awake()
        {
            taskDict = new Dictionary<int, TaskObj>();
            GetComponent<NetObject>().RegisterChild(NetObjectType.TASK, OnTaskCreate, OnTaskUpdate, OnTaskDestroy);
            taskQueueUI = GameObject.Find("TaskQueueDisplay").GetComponent<TextMeshProUGUI>();
        }

        private void OnTaskCreate(INetObject obj)
        {
            taskDict.Add(obj.NetID, new TaskObj { Order = (obj.CreateData as TaskCreateData).Order });
            UpdateQueueText();
        }

        private void OnTaskUpdate(INetObject obj, NetUpdate update)
        {
            taskDict[obj.NetID].Desc = (update as TaskUpdate).Text;
            UpdateQueueText();
        }

        private void UpdateQueueText()
        {
            var taskObjs = taskDict.Values;
            var sorted = new List<TaskObj>(taskObjs);
            sorted.Sort((x, y) => x.Order - y.Order);

            var builder = new StringBuilder();
            builder.Append("Queue:\n======\n");
            foreach(var obj in taskObjs)
            {
                builder.Append(obj.Order + " " + obj.Desc);
                builder.Append('\n');
            }
            taskQueueUI.SetText(builder.ToString().ToUpper());
        }

        private void OnTaskDestroy(INetObject obj)
        {
            //var data = obj.DestroyData as TaskDestroyData;
            //Debug.Log($"{taskDict[obj.NetID].Order} - {data.DummyText}");
            taskDict.Remove(obj.NetID);
            UpdateQueueText();
        }

        private class TaskObj
        {
            public String Desc { get; set; }
            public int Order { get; set; }
        }
    }
}