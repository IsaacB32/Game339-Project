using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
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
    private int _dayScore;
    private int _totalScore;

    void Start()
    {
        scoreScreenPanel.SetActive(false);
        finalScreenPanel.SetActive(false);
        nextDayButton.onClick.AddListener(OnNextDayPressed);

        _currentDay = 0;
        _totalScore = 0;

        StartDay();
    }

    void StartDay()
    {
        _currentDay++;
        _dayScore = 0;
        orderScreenManager.StartDay(OnDayComplete);
    }

    void OnDayComplete(int dayScore)
    {
        _dayScore = dayScore;
        _totalScore += dayScore;

        ShowScoreScreen();
    }

    void ShowScoreScreen()
    {
        scoreScreenPanel.SetActive(true);
        dayScoreText.text = "Day " + _currentDay + " Score: " + _dayScore;
        totalScoreText.text = "Total: " + _totalScore;

        var buttonText = nextDayButton.GetComponentInChildren<TextMeshProUGUI>();
        if (_currentDay >= totalDays)
            buttonText.text = "Finish";
        else
            buttonText.text = "Next Day";

        orderScreenManager.FadeIn();
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
        finalScoreText.text = "Final Score: " + _totalScore;
    }
}