using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button startButton;
    [SerializeField] private Button restartButton;

    [Header("Game References")]
    [SerializeField] private SpawnTiles spawnTiles;
    [SerializeField] private HitJudgmentSystem hitJudgmentSystem;
    [SerializeField] private FeedbackManager feedbackManager;

    private bool isGameOver = false;
    private int score = 0;

    private void Start()
    {
        gameOverPanel.SetActive(false);
        startPanel.SetActive(true);

        // Đăng ký các sự kiện
        if (startButton != null)
            startButton.onClick.AddListener(StartGame);
        
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        if (hitJudgmentSystem != null)
            hitJudgmentSystem.OnHitJudged += HandleHitJudgment;
    }

    private void OnDestroy()
    {
        // Hủy đăng ký sự kiện
        if (startButton != null)
            startButton.onClick.RemoveListener(StartGame);
        
        if (restartButton != null)
            restartButton.onClick.RemoveListener(RestartGame);

        if (hitJudgmentSystem != null)
            hitJudgmentSystem.OnHitJudged -= HandleHitJudgment;
    }

    public void StartGame()
    {
        startPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        
        isGameOver = false;
        score = 0;

        if (feedbackManager != null)
        {
            feedbackManager.ResetScore();
        }

        if (spawnTiles != null)
        {
            spawnTiles.StartGame();
        }
    }

    private void RestartGame()
    {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        foreach (GameObject tile in tiles)
        {
            Destroy(tile);
        }

        StartGame();
    }

    private void HandleHitJudgment(HitJudgment judgment)
    {
        switch (judgment)
        {
            case HitJudgment.Perfect:
                score += 100;
                break;
            case HitJudgment.Good:
                score += 50;
                break;
            case HitJudgment.Miss:
                GameOver();
                break;
        }
    }

    public void GameOver()
    {
        if (!isGameOver)
        {
            isGameOver = true;
            gameOverPanel.SetActive(true);

            if (spawnTiles != null)
            {
                spawnTiles.StopGame();
            }
        }
    }
} 