using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// class to keep track of crystal lights and their
// chunk positions
[System.Serializable]
public class CrystalLightInfo
{
    public GameObject lightObject;
    public int chunkPosition;

    public CrystalLightInfo(GameObject lightObject, int chunkPosition)
    {
        this.lightObject = lightObject;
        this.chunkPosition = chunkPosition;
    }
}
public class ProceduralGeneration : MonoBehaviour
{
    private int chunkSize = 16;
    private int height = 6;
    private int renderDistance = 1; 
    private int heightDiff = 6;

    [SerializeField] float smoothness = 10;
    [SerializeField] TileBase groundTile;
    [SerializeField] Tilemap groundTilemap;
    [SerializeField] TileBase roofTile;
    [SerializeField] Tilemap roofTilemap;

    [SerializeField] TileBase obstacleTile;
    [SerializeField] TileBase crystalTile;
    public GameObject CrystalLight;
    private List<CrystalLightInfo> crystalLights = new List<CrystalLightInfo>();
    private float roofOffset = 5000f;
    float seed;


    private float cameraWidth = 18f;
    private float cameraHeight = 10f;


    [SerializeField] Transform player;

    HashSet<int> activeChunks = new HashSet<int>();

    public delegate int HeightFunction(int x, int chunkX, float offset, float seed, float smoothness, int height);


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        groundTilemap.transform.position = new Vector3(-cameraWidth,-cameraHeight,0);
        roofTilemap.transform.position = new Vector3(-cameraWidth,cameraHeight,0);
        roofTilemap.transform.rotation = Quaternion.Euler(180, 0, 0); 
        seed = Random.Range(-10000,10000);
        UpdateChunks();

        // update now that a "safe" spawn has been generated
        renderDistance = 3; 
        height = 12;
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
                int[,] groundChunkMap = GenerateChunk(chunkPos, 0, false);
                RenderMap(groundChunkMap, chunkPos, groundTilemap, groundTile, obstacleTile, crystalTile, GroundHeightFunction, 0, false);
            
            // Generate and render roof chunk
                int[,] roofChunkMap = GenerateChunk(chunkPos, roofOffset, true);
                RenderMap(roofChunkMap, chunkPos, roofTilemap, roofTile, obstacleTile, crystalTile, RoofHeightFunction, roofOffset, true);
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
            for (int i = crystalLights.Count - 1; i >= 0; i--)
            {
                // delete crystal lights in unloaded chunks
                if (crystalLights[i].chunkPosition == chunkPos)
                {
                    Destroy(crystalLights[i].lightObject); 
                    crystalLights.RemoveAt(i);               
                }
            }
            activeChunks.Remove(chunkPos);
        }
    }

    private int[,] GenerateChunk(int chunkX, float perlinOffset, bool isRoof)
    {
        int[,] map = GenerateArray(chunkSize, height, true);

        if (isRoof)
        {
            return TerrainGeneration(map, chunkX, perlinOffset, RoofHeightFunction);
        }
        else
        {
            return TerrainGeneration(map, chunkX, perlinOffset, GroundHeightFunction);
        }
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

    public int GroundHeightFunction(int x, int chunkX, float offset, float seed, float smoothness, int height)
    {
        return Mathf.RoundToInt(Mathf.PerlinNoise((chunkX * chunkSize + x) / smoothness, seed) * height);
    }

    public int RoofHeightFunction(int x, int chunkX, float offset, float seed, float smoothness, int height)
    {
        // ensures a separation of at least `heightDiff` between the ground and roof
        int groundHeight = GroundHeightFunction(x, chunkX, offset, seed, smoothness, height);
        int roofHeight = Mathf.RoundToInt(Mathf.PerlinNoise((chunkX * chunkSize + x) / smoothness, seed + offset) * height);
        int dist = 2*(int)cameraHeight - roofHeight - groundHeight;
        if (dist < heightDiff)
        {
            roofHeight = 2*(int)cameraHeight - groundHeight - heightDiff;
        }
        return roofHeight;
    }

    public int[,] TerrainGeneration(int[,] map, int chunkX, float offset, HeightFunction heightFunction)
    {
        for (int x = 0; x < chunkSize; x++)
        {
            int terrainHeight = heightFunction(x, chunkX, offset, seed, smoothness, height);
            for (int y = 0; y < terrainHeight; y++)
            {
                if (y < height) 
                {
                    map[x, y] = 1; 
                }
            }
        }
        return map;
    }

    public void RenderMap(int[,] map, int chunkPosition, Tilemap tilemap, TileBase tile, TileBase obstacleTile, TileBase crystalTile, HeightFunction heightFunction, float offset, bool roof)
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == 1)
                {
                    tilemap.SetTile(new Vector3Int(chunkPosition * chunkSize + x, y, 0), tile);

                    // obstacles
                    if (y == heightFunction(x, chunkPosition, offset, seed, smoothness, height) - 1 && Random.value < 0.3f)
                    {
                        if (Random.value < 0.95f)
                        {
                            tilemap.SetTile(new Vector3Int(chunkPosition * chunkSize + x, y + 1, 0), obstacleTile);
                        }
                        else
                        {
                            Vector3Int crystalPosition = new Vector3Int(chunkPosition * chunkSize + x, y + 1, 0);
                            tilemap.SetTile(crystalPosition, crystalTile);

                            // Instantiate the light at the crystal position
                            float heightBump = roof ? -0.7f : 0.7f;
                            Vector3 lightPosition = tilemap.CellToWorld(crystalPosition) + new Vector3(0.6f, heightBump, 0);
                            GameObject light = Instantiate(CrystalLight, lightPosition, Quaternion.identity);
                            light.GetComponent<Light2D>().intensity = 4f; // Adjust as needed

                            crystalLights.Add(new CrystalLightInfo(light, chunkPosition));
                        }
                    }
                }
            }
        }
        
    }

}


