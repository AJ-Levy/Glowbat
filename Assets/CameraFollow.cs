using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float smoothing = 5f;
    float leftBound;

    void Start(){
        leftBound = player.position.x;
    }
    void FixedUpdate()
    {

        Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, transform.position.z);
        //clamping
        targetPosition.x = Mathf.Max(targetPosition.x, leftBound);
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing * Time.deltaTime);
    }
}