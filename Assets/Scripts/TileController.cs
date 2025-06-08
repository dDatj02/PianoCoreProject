using UnityEngine;

public class TileController : MonoBehaviour
{
    private bool isHit = false;
    private HitJudgmentSystem hitJudgmentSystem;
    private float screenBottom;
    private bool isInHitLine = false;
    private GameManager gameManager;

    private void Start()
    {
        hitJudgmentSystem = FindObjectOfType<HitJudgmentSystem>();
        screenBottom = -Camera.main.orthographicSize;
        
        // Debug check components
        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null)
        {
            Debug.LogError("TileController: No Collider2D found on tile!");
        }

        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene!");
        }
    }

    private void Update()
    {
        // Lấy camera chính
        Camera mainCamera = Camera.main;
        if (mainCamera == null) return;

        // Lấy vị trí mép dưới của màn hình trong world space
        Vector3 bottomEdge = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0f, mainCamera.nearClipPlane));

        // Lấy vị trí mép trên của tile
        float tileTopEdge = transform.position.y + (GetComponent<SpriteRenderer>().bounds.size.y / 2f);

        // Kiểm tra nếu mép trên của tile chạm mép dưới của màn hình
        if (tileTopEdge <= bottomEdge.y)
        {
            if (gameManager != null)
            {
                gameManager.GameOver();
            }
            ObjectPooler.Instance.ReturnToPool("Tile", gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("HitLine"))
        {
            Debug.Log("HitLine detected!");
            isInHitLine = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("HitLine"))
        {
            Debug.Log("Exited HitLine!");
            isInHitLine = false;
        }
    }

    public void Hit()
    {
        if (!isHit)
        {
            isHit = true;
            if (hitJudgmentSystem != null)
            {
                // Nếu tile đang trong hit line -> Perfect
                if (isInHitLine)
                {
                    Debug.Log("Hit Perfect");
                    hitJudgmentSystem.TriggerHitJudgment(HitJudgment.Perfect);
                }
                // Nếu tile trong màn hình nhưng không trong hit line -> Good
                else if (transform.position.y > screenBottom)
                {
                    Debug.Log("Hit Good");
                    hitJudgmentSystem.TriggerHitJudgment(HitJudgment.Good);
                }
                // Nếu tile ngoài màn hình -> Miss
                else
                {
                    Debug.Log("Hit Miss");
                    hitJudgmentSystem.TriggerHitJudgment(HitJudgment.Miss);
                }
            }
            ObjectPooler.Instance.ReturnToPool("Tile", gameObject);
        }
    }

    public void OnObjectSpawn()
    {
        isHit = false;
        isInHitLine = false;
    }
} 