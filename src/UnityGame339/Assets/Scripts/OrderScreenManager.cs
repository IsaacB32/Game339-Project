using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

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
    private int _currentCustomer;
    
    private System.Collections.Generic.List<int> _usedOrdersToday = new System.Collections.Generic.List<int>();
    private int _currentMinigameIndex;
    private int _dayScore;
    private Action<int> _onDayComplete;

    public void StartDay(float curseLevel, Action<int> onDayComplete)
    {
        _onDayComplete = onDayComplete;
        _dayScore = 0;
        _currentCustomer = 0;
        _usedOrdersToday.Clear();

        ApplyDifficulty(curseLevel);
        ShowOrderScreen();
        StartCoroutine(ServeNextCustomer());
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

        int index;
        if (_usedOrdersToday.Count >= possibleOrders.Length)
        {
            _usedOrdersToday.Clear();
        }

        do
        {
            index = UnityEngine.Random.Range(0, possibleOrders.Length);
        } while (_usedOrdersToday.Contains(index));

        _usedOrdersToday.Add(index);
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
        ShowMinigamePanel();
        PlayMinigame(0);
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
    
    IEnumerator ServeNextCustomer()
    {
        _currentCustomer++;
        yield return StartCoroutine(CustomerEnter());
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

        if (_currentMinigameIndex >= minigames.Length)
        {
            StartCoroutine(FinishDay());
        }
        else
        {
            PlayMinigame(_currentMinigameIndex);
        }
    }

    IEnumerator FinishDay()
    {
        yield return StartCoroutine(CustomerExit());

        if (_currentCustomer < customersPerDay)
        {
            ShowOrderScreen();
            yield return StartCoroutine(ServeNextCustomer());
        }
        else
        {
            _onDayComplete?.Invoke(_dayScore);
        }
    }
}