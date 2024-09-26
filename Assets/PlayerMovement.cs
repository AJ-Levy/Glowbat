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
    private LogicScript logic;

    
    void Start(){
        mainCamera = Camera.main;
        leftBound = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + playerOffset;

        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
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
        logic.UpdateScore();
    }
    

     private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player collides with the ground or roof tilemaps
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Roof"))
        {
            logic.restartGame();
        }
    }

}
