using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Class neccesary to keep track of crystal's and their lights for removal when out of view.
/// </summary>
[System.Serializable]
public class CrystalLightInfo
{
    public GameObject lightObject;
    public int chunkPosition;

    /// <summary>
    /// Simple constructor that instantiates a CrystalLightInfo object.
    /// </summary>
    /// <param name="lightObject">The light associated with a crystal.</param>
    /// <param name="chunkPosition">The chunk it was spawned in.</param>
    public CrystalLightInfo(GameObject lightObject, int chunkPosition)
    {
        this.lightObject = lightObject;
        this.chunkPosition = chunkPosition;
    }
}

/// <summary>
/// A class that procedurally generates cave terrain with obstacles and crystals.
/// Chunking is used, with out of view chunks unloaded to save memory.
/// </summary>
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


    /// <summary>
    /// Some intialisation and sets (forgiving) spawn chunk parameters.
    /// </summary>
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

    /// <summary>
    /// Update the chunks rendered/loaded.
    /// </summary>
    void Update()
    {
        UpdateChunks();
    }

    /// <summary>
    /// Determine the player's current chunk and load next `renderDistance` chunks,
    /// and unload those that have been passed (i.e. aren't in view).
    /// </summary>
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

    /// <summary>
    /// Generates a single chunk under the desired conditions.
    /// </summary>
    /// <param name="chunkX">The chunk postion.</param>
    /// <param name="perlinOffset">The offset on the perlin noise terrain generation.</param>
    /// <param name="isRoof">Whether the roof or floor is being generated.</param>
    /// <returns>A map containing the generated chunk (either floor or roof components).</returns>
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
    
    /// <summary>
    /// Creates and intialises a 2D array.
    /// </summary>
    /// <param name="width">The width (x dimension) of the array.</param>
    /// <param name="height">The height (y dimension) of the array.</param>
    /// <param name="empty">Whether the array element is filled or not</param>
    /// <returns>A map containing filled or empty elements.</returns>
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

    /// <summary>
    /// Function that generates a random ground terrain constrained by a certain height.
    /// </summary>
    /// <param name="x">The horizontal postion of the tile.</param>
    /// <param name="chunkX">The chunk the tile is located in.</param>
    /// <param name="offset">The offset of the perlin noise function.</param>
    /// <param name="seed"The seed of the perlin noise function.</param>
    /// <param name="smoothness">The smoothness of the terrain.</param>
    /// <param name="height">The maximum height of the terrain.</param>
    /// <returns>An integer height.</returns>
    public int GroundHeightFunction(int x, int chunkX, float offset, float seed, float smoothness, int height)
    {
        return Mathf.RoundToInt(Mathf.PerlinNoise((chunkX * chunkSize + x) / smoothness, seed) * height);
    }

    /// <summary>
    /// Function that generates a random roof terrain constrained by a certain height
    /// and that adheres to a minimum ground height separation condition.
    /// </summary>
    /// <param name="x">The horizontal postion of the tile.</param>
    /// <param name="chunkX">The chunk the tile is located in.</param>
    /// <param name="offset">The offset of the perlin noise function.</param>
    /// <param name="seed"The seed of the perlin noise function.</param>
    /// <param name="smoothness">The smoothness of the terrain.</param>
    /// <param name="height">The maximum height of the terrain.</param>
    /// <returns>An integer height.</returns>
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

    /// <summary>
    /// Generates terrain using a specified height function.
    /// </summary>
    /// <param name="map">The map containing the chunk</param>
    /// <param name="chunkX">The chunk postion.</param>
    /// <param name="offset">The perlin noise offset.</param>
    /// <param name="heightFunction">The function that determines the height of the terrain.</param>
    /// <returns>A map containing the generated terrain.</returns>
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

    /// <summary>
    /// Sets the tiles of a map appropriately and randomly adds obstacles (including crystals with ambient light).
    /// </summary>
    /// <param name="map">The populated chunk map.</param>
    /// <param name="chunkPosition">The chunk postion.</param>
    /// <param name="tilemap">The appropriate tile map</param>
    /// <param name="tile">The terrain rule tile.</param>
    /// <param name="obstacleTile">The stagmite/stalactite rule tile.</param>
    /// <param name="crystalTile"><The crystal tile./param>
    /// <param name="heightFunction">The function that determines the height of the terrain.</param>
    /// <param name="offset">The perlin noise offset.</param>
    /// <param name="roof">Whether the roof or floor is being rendered.</param>
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


