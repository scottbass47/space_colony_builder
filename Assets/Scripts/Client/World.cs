using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shared.SCPacket;
using Shared.SCData;
using UnityEngine.Tilemaps;
using Shared.StateChanges;

namespace Client
{
    public class World : MonoBehaviour
    {
        public bool loadingWorld = true;
        private int chunksReceived = 0;
        private int totalChunks = 0;
        private SCTileData[] tileDataBuffer;
        private int tileBufferIdx;
        public GameObject GroundPrefab;
        public GameObject Ground;
        public TileBase tileBase;

        private Dictionary<Vector3Int, GameObject> map;
        
        void Start()
        {
            var client = Game.Instance.Client;
            client.AddPacketListener<WorldInitPacket>(WorldInit);
            client.AddPacketListener<WorldChunkPacket>(WorldChunk);
            map = new Dictionary<Vector3Int, GameObject>();
        }

        void WorldInit(WorldInitPacket packet)
        {
            Debug.Log($"SCClient.WorldChunk - Received world init packet. Expecting {packet.Chunks} chunks of data.");
            tileDataBuffer = new SCTileData[packet.Size * packet.Size];
            tileBufferIdx = 0;
            totalChunks = packet.Chunks;
        }

        // @Bug NullPointer if the Init packet is dropped (TileChange arr doesn't get created)
        void WorldChunk(WorldChunkPacket packet)
        {
            chunksReceived++;
            Debug.Log($"SCClient.WorldChunk - Received chunk {packet.ChunkNumber}.");

            for (int i = 0; i < packet.DataCount; i++)
            {
                tileDataBuffer[tileBufferIdx++] = packet.TileData[i];
            }

            // We have all the data, create the world
            if (chunksReceived == totalChunks)
            {
                CreateWorld();
            }
        }

        void CreateWorld()
        {
            Debug.Log("SCClient.CreateWorld - All data received! Creating world.");

            Ground = Instantiate(GroundPrefab);

            Tilemap tilemap = Ground.GetComponentInChildren<Tilemap>();
            tilemap.ClearAllTiles();

            TileStore tileStore = GameObject.Find("Tile Registry").GetComponent<TileStore>();

            foreach (SCTileData data in tileDataBuffer)
            {
                tilemap.SetTile(new Vector3Int { x = data.X, y = data.Y, z = data.Z }, tileStore.Get(data.TileID));
            }

            tilemap.CompressBounds();
            Vector3 center = tilemap.cellBounds.center;
            Vector3 camPos = tilemap.CellToWorld(new Vector3Int((int)center.x, (int)center.y, 0));
            camPos.z = -10;

            Camera.main.transform.position = camPos;

            loadingWorld = false;

            //GameObject.Find("Loading Screen").SetActive(false);
        }

        public void AddMapObject(GameObject obj)
        {
            map.Add(obj.GetComponent<TilemapObject>().Pos, obj);
            Ground.GetComponent<MapObjectRenderer>().AddMapObject(obj);          
        }

        public void RemoveMapObject(GameObject obj)
        {
            map.Remove(obj.GetComponent<TilemapObject>().Pos);
            Ground.GetComponent<MapObjectRenderer>().RemoveMapObject(obj);     
        }

        public GameObject GetMapObject(Vector3Int pos)
        {
            map.TryGetValue(pos, out GameObject obj);
            return obj;
        }
    }

}