using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using UnityEngine;

namespace Shared
{
    [ProtoContract]
    [ProtoInclude(10, typeof(HealthUpdate))]
    [ProtoInclude(11, typeof(PositionUpdate))]
    [ProtoInclude(12, typeof(PathUpdate))]
    [ProtoInclude(13, typeof(StateUpdate))]
    [ProtoInclude(14, typeof(OreUpdate))]
    [ProtoInclude(15, typeof(HouseUpdate))]
    [ProtoInclude(16, typeof(TaskQueueUpdate))]
    public class SCUpdate
    {
    }

    [ProtoContract]
    public class HealthUpdate : SCUpdate
    {
        [ProtoMember(1)]
        public int Health { get; set; }
    }

    [ProtoContract]
    public class PositionUpdate : SCUpdate
    {
        [ProtoMember(1)]
        public Vector3 Pos { get; set; }
    }

    [ProtoContract]
    public class PathUpdate : SCUpdate
    {
        [ProtoMember(1)]
        public Vector2[] Path { get; set; }
    }

    [ProtoContract]
    public class StateUpdate : SCUpdate
    {
        [ProtoMember(1)]
        public int State { get; set; }
    }

    [ProtoContract]
    public class OreUpdate : SCUpdate
    {
        [ProtoMember(1)]
        public int Amount { get; set; }
    }

    [ProtoContract]
    public class HouseUpdate : SCUpdate
    {
        [ProtoMember(1)]
        public int[] Residents { get; set; }
    }

    [ProtoContract]
    public class TaskQueueUpdate : SCUpdate
    {
        [ProtoMember(1)]
        public string QueueText { get; set; } 
    }
}
