using UnityEngine;

public class HitLine : MonoBehaviour
{
    [SerializeField] private float lineWidth = 0.05f; // Độ dày của đường hit
    private LineRenderer lineRenderer;
    private BoxCollider2D hitCollider;

    private void Start()
    {
        SetupHitLine();
    }

    private void SetupHitLine()
    {
        // Lấy kích thước màn hình trong world space từ camera
        Camera mainCamera = Camera.main;
        float screenHeight = mainCamera.orthographicSize * 2;
        float screenWidth = screenHeight * mainCamera.aspect;

        // Đặt vị trí đường hit ở 1/4 màn hình từ dưới lên
        float bottomPosition = mainCamera.transform.position.y - mainCamera.orthographicSize;
        float hitLineY = bottomPosition + (screenHeight * 0.25f); // 1/4 màn hình từ dưới lên
        transform.position = new Vector3(mainCamera.transform.position.x, hitLineY, 0);

        // Tạo LineRenderer
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = new Color(1f, 1f, 1f, 0.5f);
        lineRenderer.endColor = new Color(1f, 1f, 1f, 0.5f);
        lineRenderer.positionCount = 2;
        lineRenderer.sortingOrder = 1; // Đảm bảo hiển thị trên các đối tượng khác
        
        // Tính toán vị trí trong world space
        float leftEdge = transform.position.x - (screenWidth / 2f);
        float rightEdge = transform.position.x + (screenWidth / 2f);
        
        // Đặt vị trí của đường
        lineRenderer.SetPosition(0, new Vector3(leftEdge, transform.position.y, 0));
        lineRenderer.SetPosition(1, new Vector3(rightEdge, transform.position.y, 0));

        // Tạo collider cho đường hit
        hitCollider = gameObject.AddComponent<BoxCollider2D>();
        hitCollider.isTrigger = true;
        hitCollider.size = new Vector2(screenWidth, lineWidth);
        hitCollider.offset = Vector2.zero; // Đặt offset về 0 để collider nằm giữa object
        
    }

    // Phương thức để thay đổi vị trí đường hit
    public void SetHitLinePosition(float yPosition)
    {
        transform.position = new Vector3(Camera.main.transform.position.x, yPosition, 0);
        // Cập nhật vị trí của LineRenderer
        if (lineRenderer != null)
        {
            float screenWidth = Camera.main.orthographicSize * 2 * Camera.main.aspect;
            float leftEdge = transform.position.x - (screenWidth / 2f);
            float rightEdge = transform.position.x + (screenWidth / 2f);
            lineRenderer.SetPosition(0, new Vector3(leftEdge, yPosition, 0));
            lineRenderer.SetPosition(1, new Vector3(rightEdge, yPosition, 0));
        }
    }

    // Phương thức để thay đổi độ dày đường hit
    public void SetLineWidth(float width)
    {
        lineWidth = width;
        if (lineRenderer != null)
        {
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
        }
        if (hitCollider != null)
        {
            hitCollider.size = new Vector2(hitCollider.size.x, width);
        }
    }

    // Phương thức để thay đổi màu sắc đường hit
    public void SetLineColor(Color color)
    {
        if (lineRenderer != null)
        {
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
        }
    }
} 