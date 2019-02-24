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
        public GameObject TaskPrefab;
        public GameObject content;

        private Dictionary<int, TaskObject> taskDict;

        public delegate void StatusListener();
        public delegate void TaskAddListener(int id);
        public delegate void TaskRemoveListener(int id);

        //update listener subscribed to by prefabs
        //create/destroy/reorder listener subscribed to by content pane

        public event TaskAddListener onTaskAdd;
        public event TaskRemoveListener onTaskRemove;

        public event StatusListener onStatusChange;
        public Dictionary<int, List<StatusListener>> statusChangeTable;
        //make a method called addListener(id, listener)

        private void Awake()
        {
            transform.parent = GameObject.Find("UI").transform;
            taskDict = new Dictionary<int, TaskObject>();
            GetComponent<NetObject>().RegisterChild(NetObjectType.TASK, OnTaskCreate, OnTaskUpdate, OnTaskDestroy);
        }

        private void OnTaskCreate(INetObject obj)
        {
            var data = obj.CreateData as TaskCreateData;

            var go = Instantiate(TaskPrefab);
            go.transform.parent = content.transform;
            var taskObj = go.GetComponent<TaskObject>();
            taskObj.SetTitle(data.Title);
            taskDict.Add(obj.NetID, taskObj);
        }

        private void OnTaskUpdate(INetObject obj, NetUpdate update)
        {
            var taskObj = taskDict[obj.NetID];
            var taskUpdate = update as TaskUpdate;
            var status = taskUpdate.Status;
            taskObj.SetStatus(status);
          //  taskDict[obj.NetID].Desc = (update as TaskUpdate).Text;
        }

        //private void UpdateQueueText()
        //{
        //    var taskObjs = taskDict.Values;
        //    var sorted = new List<TaskObj>(taskObjs);
        //    sorted.Sort((x, y) => x.Order - y.Order);

        //    var builder = new StringBuilder();
        //    builder.Append("Queue:\n======\n");
        //    foreach(var obj in taskObjs)
        //    {
        //        builder.Append(obj.Order + " " + obj.Desc);
        //        builder.Append('\n');
        //    }
        //    taskQueueUI.SetText(builder.ToString().ToUpper());
        //}

        private void OnTaskDestroy(INetObject obj)
        {
            //var data = obj.DestroyData as TaskDestroyData;
            //Debug.Log($"{taskDict[obj.NetID].Order} - {data.DummyText}");
            var taskObj = taskDict[obj.NetID];
            Destroy(taskObj.gameObject);
            taskDict.Remove(obj.NetID);
        }

    }
}