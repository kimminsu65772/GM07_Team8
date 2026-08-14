using UnityEngine;

public class ParallaxMap : MonoBehaviour
{
    private Transform mainCamera;

    [SerializeField, Range(0f,1f)] private float parallaxIntensityX;
    [SerializeField, Range(0f,1f)] private float parallaxIntensityY;
    [SerializeField, Range(-1f, 1f)] private float independantSpeed;

    private float spriteWidth;
    private Vector3 cameraStartPosition;
    private Vector3 layerStartPosition;
    private float translationOffset = 0;

    private void Start()
    {
        mainCamera = Camera.main.transform;
        spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x / 3;
        cameraStartPosition = mainCamera.position;
        layerStartPosition = transform.position;
    }

    private void LateUpdate()
    {
        Vector3 cameraDelta = mainCamera.position - cameraStartPosition;

        translationOffset += independantSpeed * Time.deltaTime * parallaxIntensityX;

        float parallaxOffsetX = cameraDelta.x * (1f - parallaxIntensityX * 0.5f);
        float parallaxOffsetY = cameraDelta.y * (1f - parallaxIntensityY);

        transform.position = new Vector3(
            layerStartPosition.x + parallaxOffsetX + translationOffset, 
            layerStartPosition.y + parallaxOffsetY, 
            layerStartPosition.z);

        HorizontalPallax();
    }

    private void HorizontalPallax()
    {
        float cameraOffsetX = mainCamera.position.x - transform.position.x;

        if (cameraOffsetX > spriteWidth / 2)
            layerStartPosition.x += spriteWidth;
        else if (cameraOffsetX < -spriteWidth / 2)
            layerStartPosition.x -= spriteWidth;
    }
}
