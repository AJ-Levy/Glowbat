using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;


public class PlayerMovement : MonoBehaviour
{
    private Camera mainCamera;
    private float leftBound;
    private Vector3 newPosition;
    public float speed = 5f;
    private float playerOffset = 0.5f;

    private float score;
    private float maxScore;
    private float incFactor;
    public Text scoreText;
    
    void Start(){
        mainCamera = Camera.main;
        leftBound = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + playerOffset;

        UpdateScoreDisplay();
    }
    void FixedUpdate()
    {   
        leftBound = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + playerOffset;
        
        // Get input from WASD keys
        float moveX = Input.GetAxis("Horizontal"); // A and D keys or Left and Right arrows
        float moveY = Input.GetAxis("Vertical");   // W and S keys or Up and Down arrows

        // Create a movement vector
        Vector3 movement = new Vector3(moveX, moveY, 0);
        newPosition = transform.position + movement * speed * Time.deltaTime;

        if (newPosition.x < leftBound)
        {
            newPosition.x = leftBound; // Clamp to left boundary
        }
        // Move the player
        transform.position = newPosition;
        UpdateScore();
    }

     void UpdateScore()
    {
        if (score < 100) {
            incFactor = 5f;
        }
        else if (score < 1000) {
            incFactor = 10f;
        } else {
            incFactor = 100f;
        } 
        
        // Calculate score in meters, round to the nearest incFactor
        score = Mathf.Floor(transform.position.x / incFactor) * incFactor;
        if (score > maxScore){
            maxScore = score;
        }
        UpdateScoreDisplay();
    }

    void UpdateScoreDisplay()
    {
        // Update the score text in the UI
        if (scoreText != null)
        {
            scoreText.text = maxScore + " m";
        }
    }
}
