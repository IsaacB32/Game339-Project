using System;
using UnityEngine;

public abstract class MinigameBase : MonoBehaviour
{
    [SerializeField] protected GameObject panel;
    protected CanvasGroup _group;

    public int Score { get; protected set; }
    public event Action<int> OnMinigameEnd;

    protected void EndMinigame()
    {
        Disable();
        OnMinigameEnd?.Invoke(Score);
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
        Score = 0;
        Enable();
        BeginMinigame();
    }

    protected abstract void BeginMinigame();
}