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
    public class ClientRequest
    {
    }

    [ProtoContract]
    public class PlaceBuildingRequest : ClientRequest
    {
        [ProtoMember(1)]
        public Vector3 Pos { get; set; }
    }

}
