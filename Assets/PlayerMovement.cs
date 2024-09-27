using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;


public class PlayerMovement : MonoBehaviour
{
    private Camera mainCamera;
    private float leftBound;
    private Vector3 newPosition;
    public float speed = 5f;
    private float playerOffset = 0.5f;
    private LogicScript logic;
    private SpriteRenderer spriteRenderer;

    private Light2D batGlow;  
    private float minRadius = 0f;
    private float maxOuterRadius = 10f;
    private float innerRadiusFraction = 0.6f;


    private float glowDecayRate = 0.075f;
    private float fireflyGlow = 0.75f;
    private float minGlow = 0f; 
    private float maxGlow = 5f; 
    private float startGlow = 3f;
    private float currentGlow;

    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    void Start(){
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        leftBound = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + playerOffset;

        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();

        batGlow = GetComponent<Light2D>();
        currentGlow = startGlow;
    }
    void FixedUpdate()
    {   
        leftBound = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + playerOffset;
        
        // Get input from WASD keys
        float moveX = Input.GetAxis("Horizontal"); // A and D keys or Left and Right arrows
        float moveY = Input.GetAxis("Vertical");   // W and S keys or Up and Down arrows

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
    }
    

     private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player collides with the ground or roof tilemaps
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Roof"))
        {
            audioManager.PlaySFX(audioManager.death);
            StartCoroutine(RestartAfterDelay());
        }

        if(collision.gameObject.CompareTag("Firefly"))
        {
            EatFirefly();
            Destroy(collision.gameObject);
        }
    }

   private void Glow(){
        currentGlow -= glowDecayRate * Time.deltaTime;
        currentGlow = Mathf.Clamp(currentGlow, minGlow, maxGlow); 


        batGlow.intensity = currentGlow;

        // adjust radii of spotlight based on currentGlow
        float normalisedGlow = (currentGlow - minGlow) / (maxGlow - minGlow);
        float radius = Mathf.Lerp(minRadius, maxOuterRadius, normalisedGlow);    
        
        batGlow.pointLightInnerRadius = radius * innerRadiusFraction;
        batGlow.pointLightOuterRadius = radius; 

        Debug.Log("Light Level:" + currentGlow.ToString());
        if (currentGlow <= minGlow)
        {
            logic.restartGame();
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
        logic.restartGame();
    }


}
