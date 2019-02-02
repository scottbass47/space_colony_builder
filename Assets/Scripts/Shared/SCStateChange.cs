using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using Server;
using Shared.SCData;
using Shared.SCPacket;
using ProtoBuf;

namespace Shared
{
    namespace StateChanges
    {
        //[ProtoContract]
        //[ProtoInclude(10, typeof(SCTileData))]
        //[ProtoInclude(11, typeof(EntitySpawn))]
        //[ProtoInclude(12, typeof(EntityRemove))]
        //[ProtoInclude(13, typeof(EntityUpdate))]
        //[ProtoInclude(14, typeof(NoChange))]
        //public class IStateChange 
        //{
        //    [ProtoMember(1)]
        //    public int Version { get; set; }
        //}

        [ProtoContract]
        public class SCTileData 
        {
            [ProtoMember(1)]
            public TileChangeType Type { get; set; }
            [ProtoMember(2)]
            public TileID TileID { get; set; } // Used for tile CREATE and CHANGE
            [ProtoMember(3)]
            public int X { get; set; }
            [ProtoMember(4)]
            public int Y { get; set; }
            [ProtoMember(5)]
            public int Z { get; set; }
        }

        public enum TileChangeType
        {
            CREATE,
            DELETE,
            CHANGE
        }

        //// @Hack We should be able to serialize Vector3 and other nested types automatically
        //[ProtoContract]
        //public class EntitySpawn : IStateChange
        //{
        //    [ProtoMember(1)]
        //    public int ID { get; set; }
        //    [ProtoMember(2)]
        //    public EntityType EntityType { get; set; }
        //    [ProtoMember(3)]
        //    public Vector3 Pos { get; set; }
        //}

        //[ProtoContract]
        //public class EntityRemove : IStateChange
        //{
        //    [ProtoMember(1)]
        //    public int ID { get; set; }
        //}

        //[ProtoContract]
        //public class EntityUpdate : IStateChange
        //{
        //    [ProtoMember(1)]
        //    public int ID { get; set; }
        //    [ProtoMember(2)]
        //    public SCUpdate Update { get; set; }
        //} 


        //// @Hack We need to signal to the client that no changes occurred if no changes
        //// occurred in a version of the world. This probably won't be used often, so it's 
        //// not a big deal, but there's also probably a better way to do this. 
        //[ProtoContract]
        //public class NoChange : IStateChange
        //{
        //}
    }
}