using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using Unity.VisualScripting;
using System;


public class PlayerManager : MonoBehaviour
{
    private Camera mainCamera;
    private float leftBound;
    private Vector3 newPosition;
    private float speed = 4f;
    private float maxSpeed = 12f;
    private float speedIncreaseRate = 0.005f;
    private float playerOffset = 0.5f;
    private LogicScript logic;
    private SpriteRenderer spriteRenderer;
    private bool isAlive; 

    private Light2D batGlow;  
    private float minRadius = 0f;
    private float maxOuterRadius = 6f;
    private float innerRadiusFraction = 0.8f;


    public float glowDecayRate = 0.1f;
    public float fireflyGlow = 0.2f;
    private float minGlow = 0f; 
    private float maxGlow = 1f; 
    public float startGlow = 0.5f;
    private float currentGlow;

    AudioManager audioManager;

    private TutorialSpawner tutSpawner;

    private bool shieldActive = false;


    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    void Start(){
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        leftBound = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + playerOffset;

        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
        tutSpawner = GameObject.FindGameObjectWithTag("Spawner")?.GetComponent<TutorialSpawner>();

        batGlow = GetComponent<Light2D>();
        currentGlow = startGlow;
        isAlive = true;
    }
    void FixedUpdate()
    {   
        leftBound = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + playerOffset;
        
        // disable movement once dead
        if (!isAlive){return;}

        // Get input from WASD keys
        float moveX = Input.GetAxis("Horizontal"); 
        float moveY = Input.GetAxis("Vertical");   

        if (moveX > 0)
        {
            spriteRenderer.flipX = false; // Facing right
        }
        else if (moveX < 0)
        {
            spriteRenderer.flipX = true; // Facing left
        }

        // Create a movement vector
        Vector3 movement = new Vector3(moveX, moveY, 0);
        newPosition = transform.position + movement * speed * Time.deltaTime;

        if (newPosition.x < leftBound)
        {
            newPosition.x = leftBound; // Clamp to left boundary
        }
        // Move the player
        transform.position = newPosition;
        logic.UpdateScore();


        Glow();
        
        
        if (speed < maxSpeed && !logic.getIsTutorial()){
            speed += Time.deltaTime * speedIncreaseRate;
            //Debug.Log("Speed " + speed.ToString());
        }
    }
    

     private void OnCollisionEnter2D(Collision2D collision)
    {
        
        // Check if the player collides with the ground or roof tilemaps
        if (!logic.getIsTutorial() && (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Roof")))
        {   
            // shield powerup check
            if (shieldActive)
            {
                DeactivateShield();
            }
            else
            {
                isAlive = false;
                audioManager.PlaySFX(audioManager.death);
                StartCoroutine(RestartAfterDelay());
            }
        }
        

        if(collision.gameObject.CompareTag("Firefly"))
        {
            EatFirefly();
            Destroy(collision.gameObject);
            audioManager.PlaySFX(audioManager.firefly);

            if(logic.getIsTutorial()){
                tutSpawner.setEaten(true);
                logic.setIsEaten(true);
            }
        }
    }

   private void Glow(){
        if (!logic.getIsTutorial())
        {
            currentGlow -= glowDecayRate * Time.deltaTime;
            currentGlow = Mathf.Clamp(currentGlow, minGlow, maxGlow); 
        } else if (logic.getIsTutorial() && logic.getDecreaseGlow())
        {
            currentGlow -= glowDecayRate * Time.deltaTime * 2.5f;
            currentGlow = Mathf.Clamp(currentGlow, 0.4f, maxGlow); 
        }
        

        batGlow.intensity = currentGlow;

        // adjust radii of spotlight based on currentGlow
        float normalisedGlow = (currentGlow - minGlow) / (maxGlow - minGlow);
        float radius = Mathf.Lerp(minRadius, maxOuterRadius, normalisedGlow);    
        
        batGlow.pointLightInnerRadius = radius * innerRadiusFraction;
        batGlow.pointLightOuterRadius = radius; 

        //Debug.Log("Light Level:" + currentGlow.ToString());
        if (currentGlow <= minGlow)
        {
            isAlive = false;
            logic.GameOver();
        }
    }

    public void EatFirefly()
    {
        currentGlow += fireflyGlow;
        currentGlow = Mathf.Clamp(currentGlow, minGlow, maxGlow);
    }

    private IEnumerator RestartAfterDelay()
    {
        // Wait for the duration of the audio clip before restarting the game
        yield return new WaitForSeconds(audioManager.death.length);
        logic.GameOver();
    }

    public float getPlayerSpeed()
    {
        return speed;
    }

    public void ActivateShield()
    {
        shieldActive = true;
        // display shield visual
    }

    public void DeactivateShield()
    {
        shieldActive = false;
        // hide shield visual
    }

    public bool isShieldActive() {
        return shieldActive;
    }

    public void IncreaseGlow(float glow)
    {
        currentGlow += glow;
        currentGlow = Mathf.Clamp(currentGlow, 0f, 1f);
    }

}
