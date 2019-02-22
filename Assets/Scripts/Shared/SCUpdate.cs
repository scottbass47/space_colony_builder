using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using Shared.SCData;
using UnityEngine;

namespace Shared
{

    // Base class for sending data in the NetCreatePacket
    [ProtoContract]
    [ProtoInclude(10, typeof(TaskCreateData))]
    [ProtoInclude(11, typeof(EntityCreateData))]
    public class CreateData
    {
    }
    
    [ProtoContract]
    public class EntityCreateData : CreateData
    {
        [ProtoMember(1)] 
        public EData[] Data { get; set; }

        // Looks in the list of all data objects and finds the one with
        // the matching type, and returns it. Returns null if no data of
        // that type can be found.
        // 
        // Ex: var positionData = GetData<EPositionData>(); 
        public T GetData<T>() where T : EData
        {
            foreach (var data in Data)
            {
                if(data.GetType() == typeof(T))
                {
                    return (T) data;
                }
            }
            return null;
        }
    }

    [ProtoContract]
    [ProtoInclude(10, typeof(EPositionData))]
    public class EData
    {
    }

    [ProtoContract]
    public class EPositionData : EData
    {
        [ProtoMember(1)] 
        public Vector3 Pos { get; set; }
    }

    [ProtoContract]
    public class TaskCreateData : CreateData
    {
        [ProtoMember(1)] 
        public int Order { get; set; }
    }

    [ProtoContract]
    [ProtoInclude(10, typeof(TaskDestroyData))]
    public class DestroyData
    {
    }

    [ProtoContract]
    public class TaskDestroyData : DestroyData
    {
        [ProtoMember(1)] 
        public string DummyText { get; set; }
    }

    [ProtoContract]
    [ProtoInclude(10, typeof(HealthUpdate))]
    [ProtoInclude(11, typeof(PositionUpdate))]
    [ProtoInclude(12, typeof(PathUpdate))]
    [ProtoInclude(13, typeof(StateUpdate))]
    [ProtoInclude(14, typeof(OreUpdate))]
    [ProtoInclude(15, typeof(HouseUpdate))]
    [ProtoInclude(16, typeof(TaskQueueUpdate))]
    [ProtoInclude(17, typeof(TaskUpdate))]
    public class NetUpdate
    {
    }

    [ProtoContract]
    public class HealthUpdate : NetUpdate
    {
        [ProtoMember(1)]
        public int Health { get; set; }
    }

    [ProtoContract]
    public class PositionUpdate : NetUpdate
    {
        [ProtoMember(1)]
        public Vector3 Pos { get; set; }
        [ProtoMember(2)]
        public Vector3 Vel { get; set; }
    }

    [ProtoContract]
    public class PathUpdate : NetUpdate
    {
        [ProtoMember(1)]
        public Vector2[] Path { get; set; }
        [ProtoMember(2)]
        public Vector2 Dest { get; set; }
    }

    [ProtoContract]
    public class StateUpdate : NetUpdate
    {
        [ProtoMember(1)]
        public int State { get; set; }
    }

    [ProtoContract]
    public class OreUpdate : NetUpdate
    {
        [ProtoMember(1)]
        public int Amount { get; set; }
    }

    [ProtoContract]
    public class HouseUpdate : NetUpdate
    {
        [ProtoMember(1)]
        public int[] Residents { get; set; }
    }

    [ProtoContract]
    public class TaskQueueUpdate : NetUpdate
    {
        [ProtoMember(1)]
        public string QueueText { get; set; } 
    }

    [ProtoContract]
    public class TaskUpdate : NetUpdate
    {
        [ProtoMember(1)]
        public string Text { get; set; }
        [ProtoMember(2)]
        public int Order { get; set; }
    }
}
