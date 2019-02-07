﻿using System;
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
    }

    [ProtoContract]
    public class PathUpdate : NetUpdate
    {
        [ProtoMember(1)]
        public Vector2[] Path { get; set; }
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
