using UnityEngine;

public class FireflySpawner : MonoBehaviour
{
    [SerializeField] GameObject fireflyPrefab; 
    private Camera mainCamera;        
    private float minSpawnDistance = 20f;
    private float maxSpawnDistance = 40f; 

    private float lastSpawnPosition; 
    private float nextSpawnPosition; 

    private float spawnPos = 20f;
    private float yVariation = 2f;

    [SerializeField] PlayerManager player;

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
            minSpawnDistance = 5 * player.getPlayerSpeed();
            maxSpawnDistance = 10 * player.getPlayerSpeed();
            nextSpawnPosition = Random.Range(minSpawnDistance, maxSpawnDistance);
            lastSpawnPosition = mainCamera.transform.position.x;
        }
    }

    void SpawnFirefly()
    {
        Vector3 spawnPosition = new Vector3(
            mainCamera.transform.position.x + spawnPos,
            mainCamera.transform.position.y + Random.Range(-yVariation, yVariation),
            1f
        );

        Instantiate(fireflyPrefab, spawnPosition, Quaternion.identity);
    }
}
