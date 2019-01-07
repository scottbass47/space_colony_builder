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
    [ProtoInclude(10, typeof(PlaceBuildingRequest))]
    [ProtoInclude(11, typeof(AddWorkerRequest))]
    public class ClientRequest
    {
    }

    [ProtoContract]
    public class PlaceBuildingRequest : ClientRequest
    {
        [ProtoMember(1)]
        public Vector3 Pos { get; set; }
    }

    [ProtoContract]
    public class AddWorkerRequest : ClientRequest
    {
        [ProtoMember(1)]
        public int EntityID { get; set; } // What is the worker being added to
        [ProtoMember(2)]
        public int Slot { get; set; }
    }

}
