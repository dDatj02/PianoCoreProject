using UnityEngine;

public class BackgroundScaler : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private SpriteRenderer backgroundSprite;
    [SerializeField] private SpriteRenderer decorBackgroundSprite;
    [SerializeField] private bool maintainAspectRatio = true;

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (backgroundSprite == null)
        {
            backgroundSprite = GetComponent<SpriteRenderer>();
        }

        if (mainCamera != null && backgroundSprite != null)
        {
            ScaleBackgrounds();
        }
        else
        {
            Debug.LogError("Main camera or background sprite is missing!");
        }
    }

    private void ScaleBackgrounds()
    {
        // Get the screen dimensions in world units
        float screenHeight = 2f * mainCamera.orthographicSize;
        float screenWidth = screenHeight * mainCamera.aspect;

        // Scale main background
        if (backgroundSprite != null && backgroundSprite.sprite != null)
        {
            ScaleSprite(backgroundSprite, screenWidth, screenHeight);
        }

        // Scale decorative background
        if (decorBackgroundSprite != null && decorBackgroundSprite.sprite != null)
        {
            ScaleSprite(decorBackgroundSprite, screenWidth, screenHeight);
        }
    }

    private void ScaleSprite(SpriteRenderer spriteRenderer, float screenWidth, float screenHeight)
    {
        // Get the sprite's original dimensions
        float spriteWidth = spriteRenderer.sprite.bounds.size.x;
        float spriteHeight = spriteRenderer.sprite.bounds.size.y;

        // Calculate the scale needed to cover the screen
        float scaleX = screenWidth / spriteWidth;
        float scaleY = screenHeight / spriteHeight;

        if (maintainAspectRatio)
        {
            // Use the larger scale to ensure the background covers the entire screen
            float scale = Mathf.Max(scaleX, scaleY);
            spriteRenderer.transform.localScale = new Vector3(scale, scale, 1f);
        }
        else
        {
            // Scale independently to fit exactly
            spriteRenderer.transform.localScale = new Vector3(scaleX, scaleY, 1f);
        }

        // Center the sprite
        spriteRenderer.transform.position = new Vector3(
            mainCamera.transform.position.x,
            mainCamera.transform.position.y,
            spriteRenderer.transform.position.z
        );
    }

    // Update scale when screen size changes
    private void Update()
    {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            ScaleBackgrounds();
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
        }
    }

    private int lastScreenWidth;
    private int lastScreenHeight;
} 