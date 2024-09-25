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
    [SerializeField] TileBase roofTile;
    [SerializeField] Tilemap roofTilemap;
    private float roofOffset = 5000f;
    float seed;
    int[,] map;
    Vector3 groundTilemapPos = new Vector3(-10,-5,0);

    [SerializeField] Transform player;

    HashSet<int> activeChunks = new HashSet<int>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        groundTilemap.transform.position = groundTilemapPos;
        roofTilemap.transform.position = new Vector3(-10,5,0);
        roofTilemap.transform.rotation = Quaternion.Euler(180, 0, 0); 
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

                 // Generate and render ground chunk
                int[,] groundChunkMap = GenerateChunk(chunkPos, 0);
                RenderMap(groundChunkMap, chunkPos, groundTilemap, groundTile);
            
            // Generate and render roof chunk
                int[,] roofChunkMap = GenerateChunk(chunkPos, roofOffset);
                RenderMap(roofChunkMap, chunkPos, roofTilemap, roofTile);
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

    private int[,] GenerateChunk(int chunkX, float perlinOffset)
    {
        int[,] map = GenerateArray(chunkSize, height, true);
        return TerrainGeneration(map, chunkX, perlinOffset);
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

    public int[,] TerrainGeneration(int[,] map, int chunkX, float offset)
    {
        int perlinHeight;
        for (int x = 0; x < chunkSize; x++)
        {
            perlinHeight = Mathf.RoundToInt(Mathf.PerlinNoise((chunkX * chunkSize + x) / smoothness, seed + offset) * (height - 1)) + 1;
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
    public void RenderMap(int[,] map, int chunkPosition, Tilemap tilemap, TileBase tile)
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == 1)
                {
                    tilemap.SetTile(new Vector3Int(chunkPosition * chunkSize + x, y, 0), tile);
                }
            }
        }
    }

}


