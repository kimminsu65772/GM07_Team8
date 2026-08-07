using UnityEngine;

public class ParallaxMap : MonoBehaviour
{
    private Transform mainCamera;

    [SerializeField, Range(0f,1f)] private float parallaxIntensityX;
    [SerializeField, Range(0f,1f)] private float parallaxIntensityY;
    [SerializeField, Range(-1f, 1f)] private float independantSpeed;

    private float cameraSize;
    private float spriteWidth;
    private Vector2 initialPos;
    private float translationOffset = 0;

    private void Start()
    {
        mainCamera = Camera.main.transform;
        cameraSize = Camera.main.orthographicSize;
        spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x / 3;

        transform.position = new Vector2(mainCamera.position.x, 0f);
        initialPos = transform.position;
    }

    private void LateUpdate()
    {
        translationOffset += independantSpeed * Time.deltaTime * parallaxIntensityX;

        float parallaxOffsetX = (mainCamera.position.x * (1 - (parallaxIntensityX / 2))) + translationOffset;
        float parallaxOffsetY = ((mainCamera.position.y / cameraSize) / 0.7f) * (1 - parallaxIntensityY);

        transform.position = new Vector2(initialPos.x + parallaxOffsetX, initialPos.y + parallaxOffsetY);

        float cameraOffsetX = mainCamera.position.x - transform.position.x;

        if (cameraOffsetX > spriteWidth / 2)
            initialPos.x += spriteWidth;
        else if (cameraOffsetX < -spriteWidth / 2)
            initialPos.x -= spriteWidth;
    }
}
