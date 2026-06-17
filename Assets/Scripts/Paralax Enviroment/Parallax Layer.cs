using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Header("Camera")]
    [Tooltip("The camera to track. If null, will automatically find Camera.main.")]
    public Transform cameraTransform;

    [Header("Parallax Strength")]
    [Tooltip("How much the layer moves with the camera horizontally. 1.0 = moves 100% with camera (sky). 0.0 = stays static in world space.")]
    [Range(0f, 1f)]
    public float parallaxFactorX = 0.5f;

    [Tooltip("How much the layer moves with the camera vertically. Usually smaller than horizontal factor to prevent background from flying off screen.")]
    [Range(0f, 1f)]
    public float parallaxFactorY = 0.2f;

    [Header("Infinite Scrolling")]
    [Tooltip("Should the background tile/wrap horizontally?")]
    public bool infiniteHorizontal = true;
    [Tooltip("Should the background tile/wrap vertically?")]
    public bool infiniteVertical = false;

    private Vector3 startPosition;
    private Vector3 startCameraPosition;
    private float textureUnitSizeX;
    private float textureUnitSizeY;

    private void Start()
    {
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        startPosition = transform.position;
        if (cameraTransform != null)
        {
            startCameraPosition = cameraTransform.position;
        }

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Sprite sprite = spriteRenderer.sprite;
            if (sprite != null)
            {
                // Calculate size in world units considering the transform's localScale
                textureUnitSizeX = (sprite.rect.width / sprite.pixelsPerUnit) * transform.localScale.x;
                textureUnitSizeY = (sprite.rect.height / sprite.pixelsPerUnit) * transform.localScale.y;
            }
        }
    }

    private void LateUpdate()
    {
        if (cameraTransform == null) return;

        Vector3 cameraPosition = cameraTransform.position;
        Vector3 cameraMovement = cameraPosition - startCameraPosition;

        // Position based on initial position, camera movement, and parallax factor
        float posX = startPosition.x + (cameraMovement.x * parallaxFactorX);
        float posY = startPosition.y + (cameraMovement.y * parallaxFactorY);

        // Update position
        transform.position = new Vector3(posX, posY, transform.position.z);

        // Infinite horizontal wrapping
        if (infiniteHorizontal && textureUnitSizeX > 0)
        {
            float relativeCameraX = cameraMovement.x * (1f - parallaxFactorX);
            if (Mathf.Abs(relativeCameraX) >= textureUnitSizeX)
            {
                float offsetValueX = Mathf.Round(relativeCameraX / textureUnitSizeX) * textureUnitSizeX;
                startPosition.x += offsetValueX;
            }
        }

        // Infinite vertical wrapping
        if (infiniteVertical && textureUnitSizeY > 0)
        {
            float relativeCameraY = cameraMovement.y * (1f - parallaxFactorY);
            if (Mathf.Abs(relativeCameraY) >= textureUnitSizeY)
            {
                float offsetValueY = Mathf.Round(relativeCameraY / textureUnitSizeY) * textureUnitSizeY;
                startPosition.y += offsetValueY;
            }
        }
    }
}