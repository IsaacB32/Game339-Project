using System;
using System.Collections;
using Game.Runtime;
using UnityEngine;
using Game339.Shared.Services.Implementation;

public abstract class MinigameBase : MonoBehaviour
{
    protected static ScoreService scoreService => ServiceResolver.Resolve<ScoreService>();
    
    [SerializeField] protected GameObject panel;
    protected CanvasGroup _group;
    
    [Header("Grade Thresholds (total points)")]
    public int perfectThreshold = 60;
    public int goodThreshold = 30;

    public event Action OnMinigameEnd;
    protected bool minigameActive;

    void Awake()
    {
        _group = panel.GetComponent<CanvasGroup>();
        Disable();
        Init();
    }

    protected virtual void Init() { }

    protected void Disable()
    {
        _group.alpha = 0;
        _group.blocksRaycasts = false;
        _group.interactable = false;
        scoreService.MinigameGrade.Value = ScoreService.Grade.Hidden;
    }

    protected void Enable()
    {
        _group.alpha = 1f;
        _group.blocksRaycasts = true;
        _group.interactable = true;
    }

    public void StartMinigame()
    {
        scoreService.MinigameGrade.Value = ScoreService.Grade.Hidden;
        Enable();
        minigameActive = true;
        BeginMinigame();
    }
    
    protected void EndMinigame()
    {
        scoreService.SetMinigameGrade(goodThreshold, perfectThreshold);
        Debug.Log("Minigame ended. Grade: " + scoreService.MinigameGrade.Value + " | Score: " + scoreService.DayScore.Value);
        minigameActive = false;

        StartCoroutine(Wait());
        return;

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(3f);
            Disable();
            OnMinigameEnd?.Invoke();
        }
    }

    protected abstract void BeginMinigame();
}