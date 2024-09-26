using UnityEngine;

public class FireflySpawner : MonoBehaviour
{
    [SerializeField] GameObject fireflyPrefab; 
    private Camera mainCamera;        
    private float minSpawnDistance = 15f;
    private float maxSpawnDistance = 35f; 

    private float lastSpawnPosition; 
    private float nextSpawnPosition; 

    void Start()
    {
        mainCamera = Camera.main;
        lastSpawnPosition = mainCamera.transform.position.x;
        nextSpawnPosition = Random.Range(minSpawnDistance, maxSpawnDistance);
    }

    void Update()
    {
        float distanceMoved = mainCamera.transform.position.x - lastSpawnPosition;
    
        if (distanceMoved >= nextSpawnPosition)
        {
            SpawnFirefly();
            nextSpawnPosition = Random.Range(minSpawnDistance, maxSpawnDistance);
            lastSpawnPosition = mainCamera.transform.position.x;
        }
    }

    void SpawnFirefly()
    {
        Vector3 spawnPosition = new Vector3(
            mainCamera.transform.position.x + Random.Range(13f, 20f),
            mainCamera.transform.position.y + Random.Range(-2f, 2f),
            1f
        );

        Instantiate(fireflyPrefab, spawnPosition, Quaternion.identity);
    }
}
