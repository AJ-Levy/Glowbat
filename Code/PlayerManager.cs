using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using Unity.VisualScripting;
using System;

/// <summary>
/// Class that handles all aspects of the player, inclduing:
/// movement, glow, powerup use, and collisions.
/// </summary>
public class PlayerManager : MonoBehaviour
{
    private Camera mainCamera;
    private float leftBound;
    private Vector3 newPosition;
    private float speed = 4f;
    private float speedStore = 4f;
    private float maxSpeed = 12f;
    private float speedIncreaseRate = 0.05f;
    private float playerOffset = 0.5f;
    private LogicScript logic;
    private SpriteRenderer spriteRenderer;
    private bool isAlive;
    private Rigidbody2D rb;
    private Vector3 originalScale;
    private float originalColliderRadius;
    private CircleCollider2D characterCollider;

    private Light2D batGlow;  
    private float minRadius = 0f;
    private float maxOuterRadius = 6f;
    private float innerRadiusFraction = 0.8f;


    private float glowDecayRate = 0.018f;
    private float fireflyGlow = 0.2f;
    private float minGlow = 0f; 
    private float maxGlow = 1f; 
    private float startGlow = 0.7f;
    private float currentGlow;

    AudioManager audioManager;

    private TutorialSpawner tutSpawner;

    private bool shieldActive = false;
    public Animator animator;

    // <summary>
    /// Keeps the AudioManger accessible.
    /// </summary>
    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    /// <summary>
    /// Some intialisation.
    /// </summary>
    void Start(){
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        leftBound = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + playerOffset;

        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
        tutSpawner = GameObject.FindGameObjectWithTag("Spawner")?.GetComponent<TutorialSpawner>();

        batGlow = GetComponent<Light2D>();
        currentGlow = startGlow;
        isAlive = true;

        originalScale = transform.localScale;
        characterCollider = GetComponent<CircleCollider2D>();
        originalColliderRadius = characterCollider.radius;
    }

    /// <summary>
    /// Update the bat's position based on key presses.
    /// Handle score updates and glowing.
    /// Slowly increase the bat's speed over time.
    /// </summary>
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

        // Update score
        logic.UpdateScore();

        Glow();
        
        
        if (speed < maxSpeed && !logic.getIsTutorial()){
            speed += Time.deltaTime * speedIncreaseRate;
            //Debug.Log("Speed " + speed.ToString());
        }
    }
    
    /// <summary>
    /// Allows the Bat to consume firefllies and checks if it collides with obstacles.
    /// Firefly consumption is straightforward and increases the bat's glow.
    /// Obstacle collision is more complex since it is disabled in the tutorial
    /// and when the shield is active.
    /// </summary>
    /// <param name="collision">The object the bat collides with.</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        // Check if the player collides with the ground or roof tilemaps
        if (!animator.GetBool("Dead") && !logic.getIsTutorial() && (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Roof")))
        {   
            // shield powerup check
            if (shieldActive)
            {
                DeactivateShield();
            }
            else
            {
                isAlive = false;
                animator.SetBool("Dead", !isAlive);
                audioManager.PauseMusic();
                audioManager.PlaySFX(audioManager.death);
                rb = GetComponent<Rigidbody2D>();
                rb.gravityScale = 1f;
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

    /// <summary>
    /// Slowly decreases glow over time (unless in the tutorial).
    /// This is manifested as a circle of light whihc slowly decreases in radius and intensity.
    /// </summary>
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

    /// <summary>
    /// The bat's glow increases when it eats a firefly, it is clampled between 0 and 1.
    /// </summary>
    public void EatFirefly()
    {
        currentGlow += fireflyGlow;
        currentGlow = Mathf.Clamp(currentGlow, minGlow, maxGlow);
    }

    /// <summary>
    /// Plays the death sound when the player dies and then intiates a game over.
    /// </summary>
    /// <returns>An IEnumerator that can be used to yield execution in a coroutine.</returns>
    private IEnumerator RestartAfterDelay()
    {
        // Wait for the duration of the audio clip before restarting the game
        yield return new WaitForSeconds(audioManager.death.length);
        logic.GameOver();
    }

    /// <summary>
    /// Get method that returns the player's speed.
    /// </summary>
    /// <returns>The player's current speed.</returns>
    public float getPlayerSpeed()
    {
        return speed;
    }

    /// <summary>
    /// Activates the player's shield when it consumes the associated powerup.
    /// </summary>
    public void ActivateShield()
    {
        shieldActive = true;
        animator.SetBool("ShieldActive", true);
        // display shield visual
    }

    /// <summary>
    /// Deactivates the player's shiled when it collides with an obstacle.
    /// </summary>
    public void DeactivateShield()
    {
        shieldActive = false;
        animator.SetBool("ShieldActive", false);
        audioManager.PlaySFX(audioManager.shieldBreak);
        // hide shield visual
    }

    /// <summary>
    /// Get method for the shieldActive variable.
    /// </summary>
    /// <returns>The state of the player's shield.</returns>
    public bool isShieldActive() {
        return shieldActive;
    }

    /// <summary>
    /// Increases the bat's glow by a certain amount
    /// </summary>
    /// <param name="glow">The amount of glow to increase by.</param>
    public void IncreaseGlow(float glow)
    {
        currentGlow += glow;
        currentGlow = Mathf.Clamp(currentGlow, 0f, 1f);
    }

    /// <summary>
    /// Increases the bat's speed by a fixed amount.
    /// </summary>
    public void SpeedBoost()
    {
        speed = speed + 2.4f;
    }

    /// <summary>
    /// Decreases the bat's speed by a fixed amount.
    /// </summary>
    public void ReduceSpeed()
    {
        speed = speed - 2.4f;
    }

    /// <summary>
    /// Decreases the bat's size by half and the collider by a third.
    /// </summary>
    public void Shrink()
    {
        transform.localScale = originalScale * 0.5f;
        characterCollider.radius = originalColliderRadius * 0.66f;
    }

    /// <summary>
    /// Restores bat to original size.
    /// </summary>
    public void UnShrink()
    {
        transform.localScale = originalScale;
        characterCollider.radius = originalColliderRadius;
    }

}
