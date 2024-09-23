using UnityEngine;
using UnityEngine.Rendering;

public class ProceduralGeneration : MonoBehaviour
{
    public GameObject boundary;
    public float segmentWidth = 5f;
    public float heightRange = 3f;
    public float baseFloorHeight = -5f;
    public float baseRoofHeight = 5f;
    public float noiseScale = 0.5f;

    private float lastGeneratedX;
    private Transform playerTransform;
    private float seed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        // random seed that defines the layout of the playthrough
        seed = Random.Range(0f, 10000f);
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

         // Generate initial floor and roof segments
        GenerateSegment(playerTransform.position.x);
        // ensure that it generates from beginning
        lastGeneratedX = -2*segmentWidth;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform.position.x > lastGeneratedX - 20f)
        {
            GenerateSegment(lastGeneratedX + segmentWidth);
        }
    }

    // needs to be improved but "good-enough" for now
    void GenerateSegment(float xPosition)
    {
        // Generate floor with a different offset for the noise
        float floorHeight = baseFloorHeight + Mathf.PerlinNoise(xPosition * noiseScale + seed, 0) * heightRange; 
        Vector3 floorPosition = new Vector3(xPosition, floorHeight, 0);
        GameObject floor = Instantiate(boundary, floorPosition, Quaternion.identity);
        floor.transform.localScale = new Vector3(segmentWidth, 0.1f, 1f); // Thin floor

        // Generate roof with a different noise offset
        float roofHeight = baseRoofHeight - Mathf.PerlinNoise(xPosition * noiseScale + seed + 1000, 0) * heightRange; 
        Vector3 roofPosition = new Vector3(xPosition, roofHeight, 0);
        GameObject roof = Instantiate(boundary, roofPosition, Quaternion.identity);
        roof.transform.localScale = new Vector3(segmentWidth, 0.1f, 1f); // Thin roof

        // Update the last generated X position
        lastGeneratedX = xPosition;
    }
}
