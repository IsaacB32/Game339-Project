using Game.Runtime;
using Game339.Shared.Services.Implementation;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static ScoreService scoreService => ServiceResolver.Resolve<ScoreService>();

    [Header("References")]
    public OrderScreenManager orderScreenManager;

    [Header("Day Settings")]
    public int totalDays = 4;

    [Header("Score Screen")]
    public GameObject scoreScreenPanel;
    public TextMeshProUGUI dayScoreText;
    public TextMeshProUGUI totalScoreText;
    public Button nextDayButton;

    [Header("Final Screen")]
    public GameObject finalScreenPanel;
    public TextMeshProUGUI finalScoreText;

    private int _currentDay;

    void Start()
    {
        scoreScreenPanel.SetActive(false);
        finalScreenPanel.SetActive(false);
        nextDayButton.onClick.AddListener(OnNextDayPressed);

        _currentDay = 0;
        scoreService.TotalScore.Value = 0;

        StartDay();
    }

    void StartDay()
    {
        _currentDay++;
        scoreService.DayScore.Value = 0;
        orderScreenManager.StartDay(OnDayComplete);
    }

    void OnDayComplete()
    {
        scoreService.TotalScore.Value += scoreService.DayScore.Value;
        ShowScoreScreen();
    }

    void ShowScoreScreen()
    {
        scoreScreenPanel.SetActive(true);
        dayScoreText.text = "Day " + _currentDay + " Score: " + scoreService.DayScore.Value;
        totalScoreText.text = "Total: " + scoreService.TotalScore.Value;

        var buttonText = nextDayButton.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = _currentDay >= totalDays ? "Finish" : "Next Day";
        orderScreenManager.FadeIn();
    }

    void OnNextDayPressed()
    {
        nextDayButton.interactable = false;

        if (_currentDay >= totalDays)
        {
            StartCoroutine(FadeAndShowFinal());
        }
        else
        {
            StartCoroutine(FadeAndStartDay());
        }
    }

    IEnumerator FadeAndStartDay()
    {
        yield return orderScreenManager.FadeOut();
        scoreScreenPanel.SetActive(false);
        StartDay();
    }

    IEnumerator FadeAndShowFinal()
    {
        yield return orderScreenManager.FadeOut();
        scoreScreenPanel.SetActive(false);
        ShowFinalScreen();
        orderScreenManager.FadeIn();
    }

    void ShowFinalScreen()
    {
        scoreScreenPanel.SetActive(false);
        finalScreenPanel.SetActive(true);
        finalScoreText.text = "Final Score: " + scoreService.TotalScore.Value;
    }
}