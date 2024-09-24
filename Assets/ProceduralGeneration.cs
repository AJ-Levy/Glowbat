using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class ProceduralGeneration : MonoBehaviour
{
    [SerializeField] int chunkSize = 16;
    [SerializeField] int height = 4;
    [SerializeField] int renderDistance = 3; 

    [SerializeField] float smoothness;
    [SerializeField] TileBase groundTile;
    [SerializeField] Tilemap groundTilemap;
    float seed;
    int[,] map;
    Vector3 groundTilemapPos = new Vector3(-10,-5,0);

    [SerializeField] Transform player;

    HashSet<int> activeChunks = new HashSet<int>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        groundTilemap.transform.position = groundTilemapPos;
        seed = Random.Range(-10000,10000);
        UpdateChunks();
    }

    void Update()
    {
        UpdateChunks();
    }

    void UpdateChunks()
    {
        // Determine the player's current chunk position
        int playerChunkPosition = Mathf.FloorToInt(player.position.x / chunkSize);

        // Generate new chunks
        for (int x = 0; x <= renderDistance; x++)
        {
            int chunkPos = playerChunkPosition + x;
            if (!activeChunks.Contains(chunkPos))
            {
                // Create a new chunk if it doesn't exist
                activeChunks.Add(chunkPos);
                int[,] chunkMap = GenerateChunk(chunkPos);
                RenderMap(chunkMap, chunkPos);
            }
        }

        // Unload old chunks
        List<int> chunksToUnload = new List<int>();
        foreach (var chunk in activeChunks)
        {
            if (Mathf.Abs(chunk - playerChunkPosition) > renderDistance)
            {
                chunksToUnload.Add(chunk);
            }
        }

        foreach (var chunkPos in chunksToUnload)
        {
            activeChunks.Remove(chunkPos);
        }
    }

    private int[,] GenerateChunk(int chunkX)
    {
        int[,] map = GenerateArray(chunkSize, height, true);
        return TerrainGeneration(map, chunkX);
    }

    public int[,] GenerateArray(int width, int height, bool empty)
    {
        int[,] map = new int[width, height];
        for(int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                map[x,y]=empty ? 0 : 1;
            }
        }
        return map;
    }

    public int[,] TerrainGeneration(int[,] map, int chunkX)
    {
        int perlinHeight;
        for (int x = 0; x < chunkSize; x++)
        {
            perlinHeight = Mathf.RoundToInt(Mathf.PerlinNoise((chunkX * chunkSize + x) / smoothness, seed) * (height - 1)) + 1;
            for (int y = 0; y < perlinHeight; y++)
            {
                if (y < height) 
                {
                    map[x, y] = 1; 
                }
            }
        }
        return map;
    }

    public void RenderMap(int[,] map, int chunkPosition)
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == 1)
                {
                    groundTilemap.SetTile(new Vector3Int(chunkPosition * chunkSize + x, y, 0), groundTile);
                }
            }
        }
    }

}


