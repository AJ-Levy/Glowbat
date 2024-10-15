using UnityEngine;

/// <summary>
/// A class that handles powerup despawning behaviour.
/// </summary>
public class PowerUpDespawn : MonoBehaviour
{
    private Camera mainCamera;
    private float maxDistanceFromCamera = 20f;

    /// <summary>
    /// Getting a reference to the camera.
    /// </summary>
    private void Start()
    {
        mainCamera = Camera.main;
    }

    /// <summary>
    /// Destroying powerups that go off screen.
    /// </summary>
    private void Update()
    {
        // Check if the power-up is too far behind the camera
        if (mainCamera.transform.position.x - transform.position.x > maxDistanceFromCamera)
        {
            Destroy(gameObject);
        }
    }
}