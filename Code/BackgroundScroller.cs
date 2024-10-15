using UnityEngine;

/// <summary>
/// A class that manages the movement of the cave background.
/// </summary>
public class BackgroundScroller : MonoBehaviour
{
    [Range(-1f,1f)]
    [SerializeField] float scrollSpeed=0.1f;
    private float offset;
    [SerializeField] Transform player;
    [SerializeField] float widthMultiplier;
    private Material mat;

    /// <summary>
    /// Getting the background and intially setting it.
    /// </summary>
    void Start()
    {
        mat=GetComponent<Renderer>().material;
        AdjustBackgroundScale();
    }

    // <summary>
    /// Adjusting the background as the player moves horizontally.
    /// </summary>
    void Update()
    {
        // Move background with player
        Vector3 newPosition = transform.position;
        newPosition.x = player.position.x;
        transform.position = newPosition;

        //Scroll image
        offset=player.position.x * scrollSpeed;
        mat.SetTextureOffset("_MainTex", new Vector2(offset,0 ));

    }

    // <summary>
    /// Scales the background appropriately for the current scene based on the camera. 
    /// </summary>
    void AdjustBackgroundScale()
    {
        Camera mainCamera = Camera.main;
        float height = 2f * mainCamera.orthographicSize;
        float width = height * mainCamera.aspect;

        MeshRenderer mr = GetComponent<MeshRenderer>();
        Bounds bounds = mr.bounds;
        float meshWidth = bounds.size.x;
        float meshHeight = bounds.size.y;

        transform.localScale = new Vector3(widthMultiplier * width / meshWidth, transform.localScale.y, transform.localScale.z);
    }
}
