using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [Range(-1f,1f)]
    public float scrollSpeed=0.1f;
    private float offset;
    public Transform player;
    public float widthMultiplier;
    private Material mat;

    void Start()
    {
        mat=GetComponent<Renderer>().material;
        AdjustBackgroundScale();
    }

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
