using UnityEngine;
using System.Collections.Generic; 

public class TutorialSpawner : MonoBehaviour
{
    [SerializeField] GameObject fireflyPrefab; 
    private Camera mainCamera;    
    private GameObject firefly;

    private LogicScript logic;

    private int spawnCount = 0;    
    private int spawnLimit = 1;
    private bool eaten = false;

    void Start()
    {
        mainCamera = Camera.main;
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
    }

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

    void SpawnFirefly()
    {
        Vector3 spawnPosition = new Vector3(
            mainCamera.transform.position.x + Random.Range(13f, 20f),
            mainCamera.transform.position.y + Random.Range(-2f, 2f),
            1f
        );

        firefly = Instantiate(fireflyPrefab, spawnPosition, Quaternion.identity);
    }

    void checkFirefly()
    {
   
        if(firefly == null && !eaten)
        {
            spawnLimit++;
        }
        
    }

    public void setEaten(bool hasEaten)
    {
        eaten = hasEaten;
    }
}

