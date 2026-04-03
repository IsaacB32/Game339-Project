using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

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

    [Header("Sign")]
    public RectTransform signRect;
    public float signSlideOffset = 350f;
    public float signOnScreenY = 325f;
    public float signSlideDuration = 0.7f;

    [Header("Order Display")]
    public TextMeshProUGUI orderText;
    public Image drinkIcon;

    [Header("Make Button")]
    public Button makeButton;

    [Header("Orders")]
    public CustomerOrder[] possibleOrders;

    [Header("Day Settings")]
    public int customersPerDay = 3;
    
    [Header("Transition")]
    public CanvasGroup transitionOverlay;
    public float transitionDuration = 0.3f;

    private int _currentMinigameIndex;
    private int _currentCustomer;
    private int _dayScore;
    private Action<int> _onDayComplete;

    private List<int> _orderQueue = new List<int>();
    private int _orderQueueIndex;
    private bool _initialized = false;

    public void StartDay(float curseLevel, Action<int> onDayComplete)
    {
        _onDayComplete = onDayComplete;
        _dayScore = 0;
        _currentCustomer = 0;

        if (!_initialized)
        {
            ShuffleOrderQueue();
            _initialized = true;
        }

        ApplyDifficulty(curseLevel);
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

    void ApplyDifficulty(float curseLevel)
    {
        foreach (var minigame in minigames)
        {
            if (minigame is GrindBeanMinigame grind)
                grind.ApplyDifficulty(curseLevel);
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
        customerImage.anchoredPosition = new Vector2(slideInX, customerImage.anchoredPosition.y);
        signRect.anchoredPosition = new Vector2(signRect.anchoredPosition.x, signOnScreenY + signSlideOffset);
        makeButton.gameObject.SetActive(false);
        PickRandomOrder();
        yield return StartCoroutine(FadeOverlay(1f, 0f));
        yield return StartCoroutine(CustomerEnter());
    }

    IEnumerator CustomerEnter()
    {
        customerImage.anchoredPosition = new Vector2(slideInX, customerImage.anchoredPosition.y);
        signRect.anchoredPosition = new Vector2(signRect.anchoredPosition.x, signOnScreenY + signSlideOffset);
        makeButton.gameObject.SetActive(false);
        makeButton.interactable = false;

        PickRandomOrder();

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

        elapsed = 0f;
        while (elapsed < signSlideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / signSlideDuration);
            signRect.anchoredPosition = new Vector2(
                signRect.anchoredPosition.x,
                Mathf.Lerp(signOnScreenY + signSlideOffset, signOnScreenY, t));
            yield return null;
        }
        signRect.anchoredPosition = new Vector2(signRect.anchoredPosition.x, signOnScreenY);

        makeButton.gameObject.SetActive(true);
        makeButton.interactable = true;
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

    void OnMinigameEnd(int score)
    {
        _dayScore += score;
        minigames[_currentMinigameIndex].OnMinigameEnd -= OnMinigameEnd;
        _currentMinigameIndex++;

        StartCoroutine(TransitionToNext());
    }

    IEnumerator FinishDay()
    {
        if (_currentCustomer < customersPerDay)
        {
            ShowOrderScreen();
            yield return StartCoroutine(ServeNextCustomer());
        }
        else
        {
            ShowOrderScreen();
            yield return StartCoroutine(FadeOverlay(1f, 0f));
            _onDayComplete?.Invoke(_dayScore);
        }
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