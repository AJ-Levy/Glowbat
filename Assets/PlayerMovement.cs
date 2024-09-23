using System.Runtime.CompilerServices;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    private Camera mainCamera;
    private float leftBound;
    private Vector3 newPosition;
    public float speed = -5f;
    
    void Start(){
        mainCamera = Camera.main;
        leftBound = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + 0.7f;
    }
    void Update()
    {
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
        
    }
}
