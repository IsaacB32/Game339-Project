using System;
using Game.Runtime;
using UnityEngine;

public abstract class MinigameBase : MonoBehaviour
{
    protected static ScoreService scoreService => ServiceResolver.Resolve<ScoreService>();
    
    [SerializeField] protected GameObject panel;
    protected CanvasGroup _group;

    public event Action OnMinigameEnd;

    protected void EndMinigame()
    {
        Disable();
        OnMinigameEnd?.Invoke();
    }

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
    }

    protected void Enable()
    {
        _group.alpha = 1f;
        _group.blocksRaycasts = true;
        _group.interactable = true;
    }

    public void StartMinigame()
    {
        Enable();
        BeginMinigame();
    }

    protected abstract void BeginMinigame();
}