using Game.Runtime;
using Game339.Shared.Services.Implementation;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

        float curseLevel = (float)(_currentDay - 1) / (totalDays - 1);
        orderScreenManager.StartDay(curseLevel, OnDayComplete);
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
        if (_currentDay >= totalDays)
        {
            buttonText.text = "Finish";
        }
        else
        {
            buttonText.text = "Next Day";
        }
    }

    void OnNextDayPressed()
    {
        scoreScreenPanel.SetActive(false);

        if (_currentDay >= totalDays)
        {
            ShowFinalScreen();
        }
        else
        {
            StartDay();
        }
    }

    void ShowFinalScreen()
    {
        scoreScreenPanel.SetActive(false);
        finalScreenPanel.SetActive(true);
        finalScoreText.text = "Final Score: " + scoreService.TotalScore.Value;
    }
}