using UnityEngine;

public class FireflyAI : MonoBehaviour
{
    [SerializeField] float speed = 2f;
    [SerializeField] float heightVariation = 1f;
    private float maxDistanceFromCamera = 15f;
     private Camera mainCamera; 
    private float originalYPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalYPos = transform.position.y;
        mainCamera = Camera.main; 
    }

    // Update is called once per frame
    void Update()
    {
       transform.position -= new Vector3(speed * Time.deltaTime, 0, 0);

        float newY = originalYPos + Mathf.Sin(Time.time) * heightVariation;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z); 

        if (mainCamera.transform.position.x - transform.position.x >  maxDistanceFromCamera)
        {
            Destroy(gameObject); // Destroy the firefly
        }
    }
}
