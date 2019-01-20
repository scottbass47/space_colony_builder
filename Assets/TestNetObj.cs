using Shared;
using Shared.SCData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class TestNetObj : MonoBehaviour
    {
        private NetObject netObjComp;

        private void Awake()
        {
            Debug.Log("TestNetObj.Awake");
            netObjComp = GetComponent<NetObject>();
            netObjComp.RegisterChild(NetObjectType.TEST_CHILD, CreateTestChild, UpdateTestChild, DestroyTestChild);
        }

        private void Start()
        {
            netObjComp.NetObj.AddUpdateListner<TestUpdate>((update) =>
            {
                //Debug.Log($"TEST update {update}");
            });
        }

        private void CreateTestChild(INetObject obj)
        {
            Debug.Log($"TEST_CHILD created with id {obj.NetID}");
        }

        private void UpdateTestChild(INetObject obj, NetUpdate update)
        {
            Debug.Log($"TEST_CHILD update with id {obj.NetID} and update {update}.");
        }

        private void DestroyTestChild(INetObject obj)
        {
            Debug.Log($"TEST_CHILD destroyed with id {obj.NetID}");
        }
    }
}