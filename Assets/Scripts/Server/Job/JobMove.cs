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

        public void Init()
        {
            var pos = entity.GetComponent<PositionComponent>();
            Vector3 myPos = pos.Pos;

            path = level.PathFinder.GetPath(worldPosToGrid(myPos), worldPosToGrid(dest));

            if(entity.HasComponent<StateComponent>())
            {
                var state = entity.GetComponent<StateComponent>();
                state.State.Value = (int)EntityState.WALKING;
            }
        }

        public void Done()
        {
            if(entity.HasComponent<StateComponent>())
            {
                var state = entity.GetComponent<StateComponent>();
                state.State.Value = (int)EntityState.IDLE;
            }
        }

        public bool IsFinished()
        {
            if (path == null) return true;

            if(target != null)
            {
                var engine = entity.GetComponent<GlobalComponent>().Engine;
                if (!engine.IsValid(target)) return true;
            }

            var pos = entity.GetComponent<PositionComponent>();
            Vector3 myPos = pos.Pos;

            return (dest - myPos).sqrMagnitude < 0.01f;
        }

        public void OnUpdate(float delta)
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

        private void moveAlongDir(Vector3 dir, float delta)
        {
            var speed = entity.GetComponent<StatsComponent>().WalkSpeed;
            var pos = entity.GetComponent<PositionComponent>();
            pos.Pos.Value += dir * speed * delta;
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
            DebugUtils.Assert(false, $"Position {pos} not on path {path}.");
            return -1;
        }

        public void SetEntity(Entity entity)
        {
            this.entity = entity;
        }

        private Vector3Int worldPosToGrid(Vector3 pos)
        {
            return new Vector3Int { x = (int)pos.x, y = (int)pos.y, z = (int)pos.z };
        }
    }
}
