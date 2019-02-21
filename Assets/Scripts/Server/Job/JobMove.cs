using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECS;
using Shared.SCData;
using UnityEngine;
using Utils;

namespace Server.Job
{
    public class JobMove : IJob
    {
        private Level level;
        private Entity entity;
        private Vector3 dest;

        private List<PathNode> path;
        private Entity target;

        public JobMove(Level level, Vector3 dest)
        {
            this.level = level;
            this.dest = dest;
        }

        public JobMove(Level level, Vector3 dest, Entity target) : this(level, dest)
        {
            this.target = target; 
        }

        public override void Init()
        {
            var pos = entity.GetComponent<PositionComponent>();
            Vector3 myPos = pos.Pos;

            path = level.PathFinder.GetPath(worldPosToGrid(myPos), worldPosToGrid(dest));

            if(entity.HasComponent<StateComponent>())
            {
                var state = entity.GetComponent<StateComponent>();
                state.State = (int)EntityState.WALKING;
            }

            if(entity.HasComponent<PathComponent>())
            {
                entity.GetComponent<PathComponent>().SetPath(path, dest);
            }


            AddTerminationCondition(() =>
            {
                if (path == null) return true;

                if (target != null)
                {
                    var engine = entity.GetComponent<GlobalComponent>().Engine;
                    if (!engine.IsValid(target)) return true;
                }

                var p = entity.GetComponent<PositionComponent>();
                return AtDest(p.Pos, dest);
            });
        }

        public override void Done()
        {
            if(entity.HasComponent<StateComponent>())
            {
                var state = entity.GetComponent<StateComponent>();
                state.State = (int)EntityState.IDLE;
            }
            entity.GetComponent<PositionComponent>().Vel = Vector3.zero;
        }

        public override void OnUpdate(float delta)
        {
            if (path == null) return;

            var pos = entity.GetComponent<PositionComponent>().Pos;
            var gridPos = worldPosToGrid(pos);
            int nodeIdx = getCurrentNodeIndex(gridPos);

            Vector3 dir = Vector3.zero;

            // This means we're at the goal node. The last thing we need to do is
            // move to the actual position inside the node.
            if(nodeIdx == path.Count - 1)
            {
                dir = dest - pos;
            }
            else
            {
                var curNode = path[nodeIdx];
                var nextNode = path[nodeIdx + 1];

                dir = new Vector3(nextNode.x - curNode.x, nextNode.y - curNode.y, 0);
            }

            moveAlongDir(dir, delta);
        }

        public static bool AtDest(Vector3 pos, Vector3 dest)
        {
            return (dest - pos).sqrMagnitude < 0.01f;
        }

        private void moveAlongDir(Vector3 dir, float delta)
        {
            var speed = entity.GetComponent<StatsComponent>().WalkSpeed;
            var pos = entity.GetComponent<PositionComponent>();

            dir.Normalize();

            var vel = dir * speed;
            //  Debug.Log($"Vel: {vel}");
            pos.Vel = vel;
            pos.Pos += vel * delta;
            //Debug.Log($"Moving colonist. New pos: {pos.Pos}");
        }

        private int getCurrentNodeIndex(Vector3Int pos)
        {
            for(int i = 0; i < path.Count; i++)
            {
                var node = path[i];
                if (node.x == pos.x && node.y == pos.y)
                {
                    return i;
                }
            }

            // Unclear how to handle this
            path = level.PathFinder.GetPath(pos, worldPosToGrid(dest));
            return 0;

            //DebugUtils.Assert(false, $"Position {pos} not on path {path}.");
            //return -1;
        }

        public override void SetEntity(Entity entity)
        {
            this.entity = entity;
        }

        private Vector3Int worldPosToGrid(Vector3 pos)
        {
            return new Vector3Int { x = (int)(pos.x + 0.5f), y = (int)(pos.y + 0.5f), z = (int)pos.z };
        }
    }
}
