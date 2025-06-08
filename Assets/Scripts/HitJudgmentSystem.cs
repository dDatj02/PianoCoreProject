using UnityEngine;
using System;

public class HitJudgmentSystem : MonoBehaviour
{
    [Header("Hit Detection Settings")]
    [Tooltip("Time window for perfect hit (in seconds)")]
    [SerializeField] private float perfectThreshold = 0.1f;

    [Tooltip("Time window for good hit (in seconds)")]
    [SerializeField] private float goodThreshold = 0.2f;

    // Event để thông báo kết quả hit
    public event Action<HitJudgment> OnHitJudged;

    public HitJudgment JudgeHit(float timeSinceExit)
    {
        if (timeSinceExit < perfectThreshold)
        {
            return HitJudgment.Perfect;
        }
        else if (timeSinceExit < goodThreshold)
        {
            return HitJudgment.Good;
        }
        else
        {
            return HitJudgment.Miss;
        }
    }

    public void TriggerHitJudgment(HitJudgment judgment)
    {
        OnHitJudged?.Invoke(judgment);
    }
}

public enum HitJudgment
{
    Perfect,
    Good,
    Miss
} 