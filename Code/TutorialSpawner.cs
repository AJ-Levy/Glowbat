using UnityEngine;
using System.Collections.Generic; 

/// <summary>
/// A simple firefly spawner used in the tutorial.
/// </summary>
public class TutorialSpawner : MonoBehaviour
{
    [SerializeField] GameObject fireflyPrefab; 
    private Camera mainCamera;    
    private GameObject firefly;

    private LogicScript logic;

    private int spawnCount = 0;    
    private int spawnLimit = 1;
    private bool eaten = false;

    /// <summary>
    /// Some intialisation.
    /// </summary>
    void Start()
    {
        mainCamera = Camera.main;
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
    }

    /// <summary>
    /// Ensures a single firefly is repatedly spawned until the player consumes one.
    /// </summary>
    void Update()
    {
        if (logic.getIsTutorial())
        {
            if (spawnCount < spawnLimit)
            {
                SpawnFirefly();
                spawnCount++;
            }

            checkFirefly();
        }
        
    }

    // <summary>
    /// Instantiates a firefly offscreen (to the right) at a slightly varied height.
    /// </summary>
    void SpawnFirefly()
    {
        Vector3 spawnPosition = new Vector3(
            mainCamera.transform.position.x + Random.Range(13f, 20f),
            mainCamera.transform.position.y + Random.Range(-2f, 2f),
            1f
        );

        firefly = Instantiate(fireflyPrefab, spawnPosition, Quaternion.identity);
    }

    /// <summary>
    /// Allows another firefly to spawn if the current one despawns (i.e. is missed by the player).
    /// </summary>
    void checkFirefly()
    {
   
        if(firefly == null && !eaten)
        {
            spawnLimit++;
        }
        
    }

    /// <summary>
    ///  Set method for eaten variable.
    /// </summary>
    /// <param name="hasEaten">Whether the firefly has been consumed.</param>
    public void setEaten(bool hasEaten)
    {
        eaten = hasEaten;
    }
}

