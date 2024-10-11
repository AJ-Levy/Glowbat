using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    [SerializeField] GameObject[] powerups;
    [SerializeField] PlayerManager playerManager;
    private float minSpawnInterval = 10f;//20f;
    private float maxSpawnInterval = 10f;//45f;

    private float spawnInterval;

    private float SpawnDistance = 20f;
    private float timeSinceLastSpawn;
    private float yVariation = 2f;

    private Camera mainCamera;


    void Start()
    {
        mainCamera = Camera.main;
        timeSinceLastSpawn = 0f;
        spawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= spawnInterval)
        {
            SpawnPowerUp();
            timeSinceLastSpawn = 0f;
            spawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
        }
    }

    private void SpawnPowerUp()
    {
        GameObject powerUpToSpawn = null;
        bool validPowerUpFound = false;

        // Try to find a valid power-up to spawn (limited to 10 attempts)
        for (int attempts = 0; attempts < 10; attempts++) 
        {
            int index = Random.Range(0, powerups.Length);
            powerUpToSpawn = powerups[index];

            // Check if the selected power-up is ShieldPowerUp and if the shield is active
            if (!(powerUpToSpawn.GetComponent<PowerUp>() is ShieldPowerUp && playerManager.isShieldActive()))
            {
                validPowerUpFound = true;
                break; 
            }
        }

        // If a valid power-up was found, instantiate it
        if (validPowerUpFound)
        {
            Vector3 spawnPosition = new Vector3(
                mainCamera.transform.position.x + SpawnDistance,
                mainCamera.transform.position.y + Random.Range(-yVariation, yVariation),
                1f
            );

            Instantiate(powerUpToSpawn, spawnPosition, Quaternion.identity);
        }
    }
}
