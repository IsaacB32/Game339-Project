using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class GrindBeanMinigame : MinigameBase
{
    private float baseTickerSpeed;
    private float baseGreenZoneWidth;
    private float baseYellowZoneWidth;

    [Header("Ticker")]
    public RectTransform ticker;
    public float tickerSpeed = 400f;

    [Header("Bar")]
    public RectTransform bar;

    [Header("Zones (children of bar)")]
    public RectTransform greenZone;
    public RectTransform yellowZoneLeft;
    public RectTransform yellowZoneRight;

    [Header("Zone Widths (normalized 0-1 relative to bar width)")]
    public float greenZoneWidth = 0.15f;
    public float yellowZoneWidth = 0.12f;

    [Header("Timer")]
    public float timeLimit = 5f;
    public TextMeshProUGUI timerText;

    [Header("Score Display")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gradeText;

    [Header("Points")]
    public int greenPoints = 10;
    public int yellowPoints = 4;
    public int missPenalty = 5;

    [Header("Grade Thresholds (total points)")]
    public int perfectThreshold = 60;
    public int goodThreshold = 30;

    [Header("In-Round Scaling")]
    public float greenHitSpeedIncrease = 20f;
    public float greenHitZoneShrink = 0.005f;
    public float yellowHitSpeedIncrease = 8f;
    public float yellowHitZoneShrink = 0.002f;
    public float minGreenZoneWidth = 0.06f;
    public float minYellowZoneWidth = 0.04f;

    private float tickerPos;
    private int tickerDirection = 1;
    private float timeRemaining;
    private int totalScore;
    private bool minigameActive;

    protected override void BeginMinigame()
    {
        baseTickerSpeed = tickerSpeed;
        baseGreenZoneWidth = greenZoneWidth;
        baseYellowZoneWidth = yellowZoneWidth;
        timeRemaining = timeLimit;
        totalScore = 0;
        tickerPos = 0f;
        tickerDirection = 1;
        minigameActive = true;

        if (gradeText) gradeText.gameObject.SetActive(false);

        UpdateZoneVisuals();
        UpdateScoreDisplay();
    }

    void Update()
    {
        if (!minigameActive) return;

        timeRemaining -= Time.deltaTime;
        if (timerText) timerText.text = Mathf.CeilToInt(timeRemaining).ToString();

        if (timeRemaining <= 0f)
        {
            EndMinigame();
            return;
        }

        float normalizedSpeed = tickerSpeed / bar.rect.width;
        tickerPos += tickerDirection * normalizedSpeed * Time.deltaTime;

        if (tickerPos >= 0.5f) { tickerPos = 0.5f; tickerDirection = -1; }
        if (tickerPos <= -0.5f) { tickerPos = -0.5f; tickerDirection = 1; }

        ticker.anchoredPosition = new Vector2(tickerPos * bar.rect.width, 0f);

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            RegisterPress();
    }

    void RegisterPress()
    {
        float tickerPixelPos = tickerPos * bar.rect.width;
        float distFromZoneCenter = Mathf.Abs(tickerPixelPos - greenZone.anchoredPosition.x);

        float halfGreenPx = (greenZoneWidth * bar.rect.width) * 0.5f;
        float halfYellowOuterPx = halfGreenPx + (yellowZoneWidth * bar.rect.width);

        int points = 0;

        if (distFromZoneCenter <= halfGreenPx)
        {
            points = greenPoints;
            tickerSpeed += greenHitSpeedIncrease;
            greenZoneWidth = Mathf.Max(minGreenZoneWidth, greenZoneWidth - greenHitZoneShrink);
            yellowZoneWidth = Mathf.Max(minYellowZoneWidth, yellowZoneWidth - greenHitZoneShrink);
            ShowHitFeedback("GREEN");
        }
        else if (distFromZoneCenter <= halfYellowOuterPx)
        {
            points = yellowPoints;
            tickerSpeed += yellowHitSpeedIncrease;
            greenZoneWidth = Mathf.Max(minGreenZoneWidth, greenZoneWidth - yellowHitZoneShrink);
            yellowZoneWidth = Mathf.Max(minYellowZoneWidth, yellowZoneWidth - yellowHitZoneShrink);
            ShowHitFeedback("YELLOW");
        }
        else
        {
            totalScore = Mathf.Max(0, totalScore - missPenalty);
            tickerSpeed = baseTickerSpeed + (tickerSpeed - baseTickerSpeed) * 0.5f;
            greenZoneWidth = baseGreenZoneWidth - (baseGreenZoneWidth - greenZoneWidth) * 0.5f;
            yellowZoneWidth = baseYellowZoneWidth - (baseYellowZoneWidth - yellowZoneWidth) * 0.5f;
            ShowHitFeedback("MISS");
        }

        RandomiseZonePosition();
        UpdateZoneVisuals();

        totalScore += points;
        UpdateScoreDisplay();
    }

    void RandomiseZonePosition()
    {
        float halfZoneTotal = (greenZoneWidth * 0.5f) + yellowZoneWidth;
        float range = 0.5f - halfZoneTotal;

        float newCenter = range <= 0f ? 0f : Random.Range(-range, range);
        greenZone.anchoredPosition = new Vector2(newCenter * bar.rect.width, 0f);
    }

    void UpdateZoneVisuals()
    {
        if (bar == null) return;
        float barWidth = bar.rect.width;

        float greenWidth = greenZoneWidth * barWidth;
        float yellowWidth = yellowZoneWidth * barWidth;
        float greenCenterX = greenZone.anchoredPosition.x;

        greenZone.sizeDelta = new Vector2(greenWidth, greenZone.sizeDelta.y);
        yellowZoneLeft.sizeDelta = new Vector2(yellowWidth, yellowZoneLeft.sizeDelta.y);
        yellowZoneRight.sizeDelta = new Vector2(yellowWidth, yellowZoneRight.sizeDelta.y);

        yellowZoneLeft.anchoredPosition = new Vector2(greenCenterX - (greenWidth * 0.5f + yellowWidth * 0.5f), 0f);
        yellowZoneRight.anchoredPosition = new Vector2(greenCenterX + (greenWidth * 0.5f + yellowWidth * 0.5f), 0f);
    }

    void UpdateScoreDisplay()
    {
        if (scoreText) scoreText.text = "Score: " + totalScore;
    }

    void ShowHitFeedback(string zone)
    {
        Debug.Log("Hit: " + zone + " | Points: " +
            (zone == "GREEN" ? greenPoints : zone == "YELLOW" ? yellowPoints : -missPenalty));
    }

    void EndMinigame()
    {
        minigameActive = false;

        string grade;
        if (totalScore >= perfectThreshold) grade = "PERFECT";
        else if (totalScore >= goodThreshold) grade = "GOOD";
        else grade = "BAD";

        if (gradeText)
        {
            gradeText.gameObject.SetActive(true);
            gradeText.text = grade + "\n" + totalScore + " pts";
        }

        Debug.Log("Minigame ended. Grade: " + grade + " | Score: " + totalScore);
        // TODO: fire an event or call MinigameManager with grade + score
    }

    public void ApplyDifficulty(float curseLevel)
    {
        tickerSpeed = Mathf.Lerp(300f, 800f, curseLevel);
        greenZoneWidth = Mathf.Lerp(0.15f, 0.05f, curseLevel);
        yellowZoneWidth = Mathf.Lerp(0.12f, 0.04f, curseLevel);
        UpdateZoneVisuals();
    }
}