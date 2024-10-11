using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FireflyAI : MonoBehaviour
{
    [SerializeField] float speed = 2f;
    [SerializeField] float heightVariation = 1f;
    private float maxDistanceFromCamera = 20f;
     private Camera mainCamera; 
    private float originalYPos;

    private Light2D fireflyLight;
    private float minIntensity = 0.2f;
    private float maxIntensity = 1.5f;
    private float minOuterRadius = 0.1f;
    private float maxOuterRadius = 0.4f;
    private float innerRadiusFactor = 0.4f;
    private float flickerSpeed = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalYPos = transform.position.y;
        mainCamera = Camera.main; 
        fireflyLight = gameObject.GetComponentInChildren<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
       transform.position -= new Vector3(speed * Time.deltaTime, 0, 0);

        float newY = originalYPos + Mathf.Sin(Time.time) * heightVariation;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z); 

        if (mainCamera.transform.position.x - transform.position.x > maxDistanceFromCamera)
        {
            Destroy(gameObject); // Destroy the firefly
        }

        // Simulate flickering by varying intensity and light size
        fireflyLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, Mathf.PingPong(Time.time * flickerSpeed, 1));
        fireflyLight.pointLightInnerRadius = Mathf.Lerp(innerRadiusFactor * minOuterRadius, innerRadiusFactor * maxOuterRadius, Mathf.PingPong(Time.time * flickerSpeed, 1));
        fireflyLight.pointLightOuterRadius = Mathf.Lerp(minOuterRadius, maxOuterRadius, Mathf.PingPong(Time.time * flickerSpeed, 1));
    }
}
