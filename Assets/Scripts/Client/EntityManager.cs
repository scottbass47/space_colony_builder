using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shared;
using Shared.SCPacket;
using Shared.StateChanges;
using Shared.SCData;
using Utils;

namespace Client
{
    public class EntityManager : MonoBehaviour
    {
        // This class is mostly useless, but we can keep this here for legacy purposes.
        public GameObject GetEntity(int id)
        {
            var netObj = Game.Instance.NetObjectManager.GetNetObject(id);
            DebugUtils.Assert(netObj.IsGameObject, $"NetObject with ID {id} and type {netObj.NetObjectType} is not a GameObject!");
            return netObj.GameObject;
        }
    }
}