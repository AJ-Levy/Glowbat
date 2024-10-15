using UnityEngine;

/// <summary>
/// A class that makes sure the camera tracks the player correctly.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float smoothing = 5f;
    float leftBound;

    /// <summary>
    /// Set the leftmost camera bound (i.e. restrict the centre of the camera) to the player's x postion.
    /// </summary>
    void Start(){
        leftBound = player.position.x;
    }

    /// <summary>
    /// Continue setting this bound as the player moves to the right in a smooth way.
    /// </summary>
    void FixedUpdate()
    {
        if (player.position.x > leftBound){
            leftBound = player.position.x + 0.01f;
        }
        Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, transform.position.z);
        //clamping
        targetPosition.x = Mathf.Max(targetPosition.x, leftBound);
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing * Time.deltaTime);
    }
}