using UnityEngine;

public class PowerUpDespawn : MonoBehaviour
{
    private Camera mainCamera;
    private float maxDistanceFromCamera = 20f;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // Check if the power-up is too far behind the camera
        if (mainCamera.transform.position.x - transform.position.x > maxDistanceFromCamera)
        {
            Destroy(gameObject);
        }
    }
}