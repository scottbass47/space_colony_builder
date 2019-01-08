using ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Priority_Queue;
using Shared.SCData;

namespace Server
{
    public class Level
    {
        private WorldStateManager worldManager;
        private Dictionary<int, MapObjectComponent> mapObjects;
        private Dictionary<Vector3Int, Entity> objectPositions;
        private int size;
        private PathFinder pathFinder;

        public int Size => size;
        public PathFinder PathFinder => pathFinder;

        public Level(WorldStateManager worldManager)
        {
            this.worldManager = worldManager;
            size = worldManager.Size;

            mapObjects = new Dictionary<int, MapObjectComponent>();
            objectPositions = new Dictionary<Vector3Int, Entity>();

            worldManager.Engine.AddGroupListener(Group.createGroup().All(typeof(MapObjectComponent)), 
                (entity) => 
                {
                    mapObjects.Add(entity.ID, entity.GetComponent<MapObjectComponent>());
                    objectPositions.Add(entity.GetComponent<MapObjectComponent>().Pos, entity);
                },
                (entity) => 
                {
                    mapObjects.Remove(entity.ID);
                    objectPositions.Remove(entity.GetComponent<MapObjectComponent>().Pos);
                });
            pathFinder = new PathFinder(this);
        }

        public bool IsMapObject(Vector3Int pos)
        {
            return objectPositions.ContainsKey(pos);
        }

        public Entity ObjectAt(Vector3Int pos)
        {
            Entity entity;
            objectPositions.TryGetValue(pos, out entity);
            return entity;
        }

        public bool IsOutOfBounds(Vector3Int pos)
        {
            return pos.x < 0 || pos.y < 0 || pos.x >= size || pos.y >= size;
        }

        public Dictionary<Vector3Int, Entity> GetObjects()
        {
            return objectPositions;
        }
    }

    // @Todo Test corner cases to make sure PathFinder works in all situations
    // @Performance Reduce amount of object creation / garbage collecting
    public class PathFinder
    {
        private Level level;

        public PathFinder(Level level)
        {
            this.level = level;
        }

        public List<PathNode> GetPath(Vector3Int from, Vector3Int to)
        {
            // We will allow paths that start and end at map objects but NOT go through them.

            // If the from and to are the same, then the path is just the from
            if(from == to) return new List<PathNode> { from };

            // Visited will store black nodes 
            HashSet<PathNode> visited = new HashSet<PathNode>();
            SimplePriorityQueue<PathNode> queue = new SimplePriorityQueue<PathNode>();
            queue.Enqueue(from, 0);

            while(queue.Count > 0)
            {
                // Getting a node from the queue means that there is no better path to this
                // node. This means that it is done being visited, so we can turn it black and
                // add it to the visited set.
                PathNode current = queue.Dequeue();
                visited.Add(current);

                if(current.x == to.x && current.y == to.y)
                {
                    List<PathNode> path = new List<PathNode>();
                    while(current != null)
                    {
                        path.Insert(0, current);
                        current = current.prev;
                    }
                    return path;
                }

                for(int x = current.x - 1; x <= current.x + 1; x++)
                {
                    for(int y = current.y - 1; y <= current.y + 1; y++)
                    {
                        if (x == current.x && y == current.y) continue; // Skip center
                        if (x != current.x && y != current.y) continue; // Skip diagonals

                        PathNode next = new PathNode { x = x, y = y, prev = current, cost = current.cost + 1 };

                        if (level.IsMapObject(next) && !(next.x == to.x && next.y == to.y)) continue; // Skip map objects unless it's the goal
                        if (level.IsOutOfBounds(next)) continue; // Skip out of bounds locations

                        // Gray node - in the middle of processing
                        if(queue.Contains(next))
                        {
                            if(queue.GetPriority(next) > next.cost)
                            {
                                queue.Remove(next);
                                queue.Enqueue(next, next.cost);
                            }
                        }
                        // White node - never seen before
                        else if(!visited.Contains(next))
                        {
                            queue.Enqueue(next, next.cost);
                        }
                    }

                }
            }

            return null;
        }
    }

    public class PathNode : IEquatable<PathNode>
    {
        static readonly PathNode Right = new PathNode { x = 1, y = 0 };
        static readonly PathNode Left = new PathNode { x = -1, y = 0 };
        static readonly PathNode Up = new PathNode { x = 0, y = 1 };
        static readonly PathNode Down = new PathNode { x = 0, y = -1 };

        public int x { get; set; }
        public int y { get; set; }
        public PathNode prev { get; set; }
        public float cost;

        public static implicit operator PathNode(Vector3Int pos)
        {
            return new PathNode { x = pos.x, y = pos.y };
        }

        public static implicit operator Vector3Int(PathNode node)
        {
            return new Vector3Int { x = node.x, y = node.y, z = 0 };
        }

        // ORDER OF OPERATIONS MATTERS (a + b != b + a)
        // Basically, there's no way to add the two nodes with regards to their
        // previous node. So for convenience, the previous of the new node is the same
        // as the preevious of the first node.
        public static PathNode operator +(PathNode one, PathNode two)
        {
            return new PathNode
            {
                x = one.x + two.x,
                y = one.y + two.y,
                prev = one.prev,
                cost = one.cost + two.cost,
            };
        }

        public override string ToString()
        {
            return $"({x}, {y})";
        }

        public static bool operator ==(PathNode node1, PathNode node2)
        {
            return EqualityComparer<PathNode>.Default.Equals(node1, node2);
        }

        public static bool operator !=(PathNode node1, PathNode node2)
        {
            return !(node1 == node2);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as PathNode);
        }

        public bool Equals(PathNode other)
        {
            return other != null &&
                   x == other.x &&
                   y == other.y;
        }

        public override int GetHashCode()
        {
            var hashCode = 1502939027;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            return hashCode;
        }
    }
}
