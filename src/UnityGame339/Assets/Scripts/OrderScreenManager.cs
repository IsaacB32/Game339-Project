using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using Game.Runtime;
using System.Collections.Generic;
using Game339.Shared.Services.Implementation;

public class OrderScreenManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject orderScreenPanel;
    public GameObject minigamePanel;
    public MinigameBase[] minigames;

    [Header("Customer")]
    public RectTransform customerImage;
    public Image customerImageRenderer;
    public float slideInX = -1200f;
    public float slideOnScreenX = 0f;
    public float slideDuration = 1f;

    [Header("Order Sign")]
    public RectTransform orderSignRect;
    public float signSlideOffset = 350f;
    public float signOnScreenY = -220f;
    public float signSlideDuration = 0.7f;

    [Header("Score Sign")]
    public RectTransform scoreSignRect;
    public float scoreSignSlideOffset = 350f;
    public float scoreSignOnScreenY = -220f;
    public float scoreSignSlideDuration = 0.7f;
    public float scoreSignDelay = 0.2f;

    [Header("Order Display")]
    public TextMeshProUGUI orderText;
    public Image drinkIcon;

    [Header("Make Button")]
    public Button makeButton;

    [Header("Orders")]
    public CustomerOrder[] possibleOrders;

    [Header("Day Settings")]
    public int customersPerDay = 3;
    private int _currentCustomer;

    [Header("Transition")]
    public CanvasGroup transitionOverlay;
    public float transitionDuration = 0.3f;

    [Header("Score Display")]
    public TextMeshProUGUI dayScoreDisplay;
    public TextMeshProUGUI totalScoreDisplay;

    [Header("Drink Give")]
    public DrinkGiveSequence drinkGiveSequence;

    private int _currentMinigameIndex;
    private Action _onDayComplete;
    private CustomerOrder _currentOrder;

    private List<int> _orderQueue = new List<int>();
    private int _orderQueueIndex;
    private bool _initialized = false;

    private int _globalCustomerCount;
    private int _totalCustomers;

    private static ScoreService scoreService => ServiceResolver.Resolve<ScoreService>();

    public void StartDay(Action onDayComplete)
    {
        _onDayComplete = onDayComplete;
        _currentCustomer = 0;

        if (!_initialized)
        {
            _globalCustomerCount = 0;
            _totalCustomers = customersPerDay * FindFirstObjectByType<GameManager>().totalDays;
            ShuffleOrderQueue();
            _initialized = true;
        }

        ShowOrderScreen();
        StartCoroutine(ServeNextCustomer());
    }

    void ShuffleOrderQueue()
    {
        _orderQueue.Clear();
        for (int i = 0; i < possibleOrders.Length; i++)
            _orderQueue.Add(i);

        for (int i = _orderQueue.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (_orderQueue[i], _orderQueue[j]) = (_orderQueue[j], _orderQueue[i]);
        }

        _orderQueueIndex = 0;
    }

    void ApplyDifficulty()
    {
        float curseLevel = (_totalCustomers > 1)
            ? (float)(_globalCustomerCount - 1) / (_totalCustomers - 1)
            : 0f;

        foreach (var minigame in minigames)
        {
            if (minigame is GrindBeanMinigame grind)
                grind.ApplyDifficulty(curseLevel);
            if (minigame is FillToLineMinigame fill)
                fill.ApplyDifficulty(curseLevel);
        }
    }

    void ShowOrderScreen()
    {
        orderScreenPanel.SetActive(true);
        minigamePanel.SetActive(false);
        makeButton.interactable = false;
    }

    void ShowMinigamePanel()
    {
        orderScreenPanel.SetActive(false);
        minigamePanel.SetActive(true);
    }

    void UpdateOrderScreenScores()
    {
        if (dayScoreDisplay) dayScoreDisplay.text = "Day Score: " + scoreService.DayScore.Value;
        if (totalScoreDisplay) totalScoreDisplay.text = "Total Score: " + (scoreService.TotalScore.Value + scoreService.DayScore.Value);
    }

    void PickRandomOrder()
    {
        if (possibleOrders.Length == 0)
        {
            Debug.LogWarning("No orders assigned to OrderScreenManager.");
            return;
        }

        if (_orderQueueIndex >= _orderQueue.Count)
        {
            ShuffleOrderQueue();
        }

        int index = _orderQueue[_orderQueueIndex];
        _orderQueueIndex++;

        CustomerOrder order = possibleOrders[index];
        _currentOrder = order;
        orderText.text = order.orderText;
        drinkIcon.sprite = order.drinkIcon;
        drinkIcon.rectTransform.sizeDelta = order.drinkIconSize;
        customerImageRenderer.sprite = order.customerSprite;
        customerImage.sizeDelta = order.customerSize;
    }

    public void OnMakePressed()
    {
        makeButton.interactable = false;
        makeButton.gameObject.SetActive(false);
        StartCoroutine(TransitionToMinigames());
    }

    IEnumerator ServeNextCustomer()
    {
        _currentCustomer++;
        _globalCustomerCount++;
        customerImage.gameObject.SetActive(true);
        customerImage.anchoredPosition = new Vector2(slideInX, customerImage.anchoredPosition.y);
        orderSignRect.anchoredPosition = new Vector2(orderSignRect.anchoredPosition.x, signOnScreenY + signSlideOffset);
        scoreSignRect.anchoredPosition = new Vector2(scoreSignRect.anchoredPosition.x, scoreSignOnScreenY + scoreSignSlideOffset);
        orderSignRect.gameObject.SetActive(true);
        scoreSignRect.gameObject.SetActive(true);
        makeButton.gameObject.SetActive(false);
        PickRandomOrder();
        ApplyDifficulty();
        UpdateOrderScreenScores();
        yield return StartCoroutine(FadeOverlay(1f, 0f));
        yield return StartCoroutine(CustomerEnter());
    }

    IEnumerator CustomerEnter()
    {
        float elapsed = 0f;
        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / slideDuration);
            customerImage.anchoredPosition = new Vector2(
                Mathf.Lerp(slideInX, slideOnScreenX, t),
                customerImage.anchoredPosition.y);
            yield return null;
        }
        customerImage.anchoredPosition = new Vector2(slideOnScreenX, customerImage.anchoredPosition.y);

        StartCoroutine(SlideSign(orderSignRect, signOnScreenY, signSlideOffset, signSlideDuration));
        yield return new WaitForSeconds(scoreSignDelay);
        yield return StartCoroutine(SlideSign(scoreSignRect, scoreSignOnScreenY, scoreSignSlideOffset, scoreSignSlideDuration));

        makeButton.gameObject.SetActive(true);
        makeButton.interactable = true;
    }

    IEnumerator SlideSign(RectTransform sign, float targetY, float offset, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            sign.anchoredPosition = new Vector2(
                sign.anchoredPosition.x,
                Mathf.Lerp(targetY + offset, targetY, t));
            yield return null;
        }
        sign.anchoredPosition = new Vector2(sign.anchoredPosition.x, targetY);
    }

    IEnumerator CustomerExit()
    {
        makeButton.gameObject.SetActive(false);
        makeButton.interactable = false;

        float elapsed = 0f;
        float currentX = customerImage.anchoredPosition.x;

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / slideDuration);
            customerImage.anchoredPosition = new Vector2(
                Mathf.Lerp(currentX, slideInX, t),
                customerImage.anchoredPosition.y);
            yield return null;
        }
    }

    void PlayMinigame(int index)
    {
        _currentMinigameIndex = index;
        minigames[index].OnMinigameEnd += OnMinigameEnd;
        minigames[index].StartMinigame();
    }

    void OnMinigameEnd()
    {
        minigames[_currentMinigameIndex].OnMinigameEnd -= OnMinigameEnd;
        _currentMinigameIndex++;

        StartCoroutine(TransitionToNext());
    }

    IEnumerator FinishDay()
    {
        ShowOrderScreen();
        orderSignRect.gameObject.SetActive(false);
        scoreSignRect.gameObject.SetActive(false);

        yield return StartCoroutine(FadeOverlay(1f, 0f));

        drinkGiveSequence.StartSequence(customerImage, _currentOrder, () =>
        {
            if (_currentCustomer < customersPerDay)
            {
                StartCoroutine(FadeAndServeNext());
            }
            else
            {
                StartCoroutine(FadeAndEndDay());
            }
        });
    }

    IEnumerator FadeAndServeNext()
    {
        yield return StartCoroutine(FadeOverlay(0f, 1f));
        ShowOrderScreen();
        yield return StartCoroutine(ServeNextCustomer());
    }

    IEnumerator FadeAndEndDay()
    {
        yield return StartCoroutine(FadeOverlay(0f, 1f));
        _onDayComplete?.Invoke();
    }

    IEnumerator TransitionToNext()
    {
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(FadeOverlay(0f, 1f));

        minigames[_currentMinigameIndex - 1].Disable();

        if (_currentMinigameIndex >= minigames.Length)
        {
            yield return StartCoroutine(FinishDay());
        }
        else
        {
            PlayMinigame(_currentMinigameIndex);
            yield return StartCoroutine(FadeOverlay(1f, 0f));
        }
    }

    IEnumerator TransitionToMinigames()
    {
        yield return StartCoroutine(FadeOverlay(0f, 1f));
        ShowMinigamePanel();
        PlayMinigame(0);
        yield return StartCoroutine(FadeOverlay(1f, 0f));
    }

    public void FadeIn()
    {
        StartCoroutine(FadeOverlay(1f, 0f));
    }

    public Coroutine FadeOut()
    {
        return StartCoroutine(FadeOverlay(0f, 1f));
    }

    IEnumerator FadeOverlay(float from, float to)
    {
        float elapsed = 0f;
        transitionOverlay.alpha = from;
        transitionOverlay.blocksRaycasts = true;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            transitionOverlay.alpha = Mathf.Lerp(from, to, elapsed / transitionDuration);
            yield return null;
        }

        transitionOverlay.alpha = to;
        transitionOverlay.blocksRaycasts = to > 0.5f;
    }
}