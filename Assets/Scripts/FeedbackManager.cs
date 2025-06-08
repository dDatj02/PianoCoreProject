using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class FeedbackManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI judgmentText;

    [Header("Feedback Settings")]
    [SerializeField] private Color perfectColor = Color.yellow;
    [SerializeField] private Color goodColor = Color.green;
    [SerializeField] private Color missColor = Color.red;

    [Header("Animation Settings")]
    [SerializeField] private float zoomInScale = 1.5f;
    [SerializeField] private float zoomDuration = 0.3f;
    [SerializeField] private AnimationCurve zoomCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip perfectSound;
    [SerializeField] private AudioClip goodSound;
    [SerializeField] private AudioClip missSound;

    private HitJudgmentSystem hitJudgmentSystem;
    private Vector3 originalScale;

    private void Start()
    {
        hitJudgmentSystem = FindObjectOfType<HitJudgmentSystem>();
        if (hitJudgmentSystem != null)
        {
            hitJudgmentSystem.OnHitJudged += HandleHitJudgment;
        }
        else
        {
            Debug.LogError("HitJudgmentSystem not found!");
        }

        // Lưu scale gốc của text
        if (judgmentText != null)
        {
            originalScale = judgmentText.transform.localScale;
        }
        else
        {
            Debug.LogError("JudgmentText not assigned!");
        }
    }

    private void HandleHitJudgment(HitJudgment judgment)
    {
        switch (judgment)
        {
            case HitJudgment.Perfect:
                ShowFeedback("PERFECT", perfectColor, perfectSound);
                UpdateScore(100);
                break;
            case HitJudgment.Good:
                ShowFeedback("GOOD", goodColor, goodSound);
                UpdateScore(50);
                break;
            case HitJudgment.Miss:
                ShowFeedback("MISS", missColor, missSound);
                UpdateScore(0);
                break;
        }
    }

    private void ShowFeedback(string text, Color color, AudioClip sound)
    {
        if (judgmentText != null)
        {
            judgmentText.text = text;
            judgmentText.color = color;
            StopAllCoroutines();
            StartCoroutine(ZoomAnimation());
        }
        else
        {
            Debug.LogError("JudgmentText is null!");
        }

        if (audioSource != null && sound != null)
        {
            audioSource.PlayOneShot(sound);
        }
    }

    private IEnumerator ZoomAnimation()
    {
        if (judgmentText == null)
        {
            Debug.LogError("JudgmentText is null in ZoomAnimation!");
            yield break;
        }
        
        judgmentText.gameObject.SetActive(true);
        judgmentText.transform.localScale = Vector3.one * 1.5f;

        float elapsedTime = 0f;
        float fadeInDuration = 0.2f;
        Color startColor = judgmentText.color;
        startColor.a = 0f;
        Color targetColor = judgmentText.color;
        targetColor.a = 1f;

        // Fade in
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeInDuration;
            judgmentText.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        yield return new WaitForSeconds(0.3f);

        // Fade out và scale down
        elapsedTime = 0f;
        float fadeOutDuration = 0.5f;
        Vector3 startScale = judgmentText.transform.localScale;
        Vector3 targetScale = Vector3.one * 0.5f; // Scale nhỏ hơn khi biến mất

        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeOutDuration;
            
            // Fade out
            Color currentColor = judgmentText.color;
            currentColor.a = Mathf.Lerp(1f, 0f, t);
            judgmentText.color = currentColor;
            
            // Scale down
            judgmentText.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            
            yield return null;
        }

        judgmentText.gameObject.SetActive(false);
    }

    private void UpdateScore(int points)
    {
        if (scoreText != null)
        {
            int currentScore = int.Parse(scoreText.text);
            currentScore += points;
            scoreText.text = currentScore.ToString();
            StartCoroutine(ScoreAnimation());
        }
    }

    private IEnumerator ScoreAnimation()
    {
        if (scoreText == null) yield break;

        float elapsedTime = 0f;
        Vector3 startScale = scoreText.transform.localScale;
        Vector3 targetScale = startScale * 1.2f;

        // Zoom in
        while (elapsedTime < 0.1f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / 0.1f;
            scoreText.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        // Zoom out
        elapsedTime = 0f;
        while (elapsedTime < 0.1f)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / 0.1f;
            scoreText.transform.localScale = Vector3.Lerp(targetScale, startScale, t);
            yield return null;
        }

        scoreText.transform.localScale = startScale;
    }

    public void ResetScore()
    {
        if (scoreText != null)
        {
            scoreText.text = "0";
            scoreText.transform.localScale = Vector3.one;
        }
        else
        {
            Debug.LogError("ScoreText is not assigned in FeedbackManager!");
        }
    }

    private void OnDestroy()
    {
        if (hitJudgmentSystem != null)
        {
            hitJudgmentSystem.OnHitJudged -= HandleHitJudgment;
        }
    }
} 