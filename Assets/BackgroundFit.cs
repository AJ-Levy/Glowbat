using UnityEngine;

public class BackgroundFit : MonoBehaviour
{
    void Start()
    {
        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        float spriteWidth = sr.sprite.bounds.size.x;
        float spriteHeight = sr.sprite.bounds.size.y;

        transform.localScale = new Vector3(width / spriteWidth, height / spriteHeight, 1);
    }
}