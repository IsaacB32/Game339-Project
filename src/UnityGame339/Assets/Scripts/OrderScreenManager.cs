using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class OrderScreenManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject orderScreenPanel;
    public GameObject minigamePanel;

    [Header("Customer")]
    public RectTransform customerImage;
    public float slideInX = -300f;
    public float slideOnScreenX = 0f;
    public float slideDuration = 0.5f;

    [Header("Sign")]
    public RectTransform signRect;
    public float signSlideOffset = 80f;
    public float signOnScreenY = 0f;
    public float signSlideDuration = 0.4f;

    [Header("Order Display")]
    public TextMeshProUGUI orderText;
    public Image drinkIcon;

    [Header("Make Button")]
    public Button makeButton;

    [Header("Orders")]
    public CustomerOrder[] possibleOrders;

    private CustomerOrder currentOrder;

    void Start()
    {
        makeButton.onClick.AddListener(OnMakePressed);
        ShowOrderScreen();
        StartCoroutine(CustomerEnter());
    }

    public void OnMinigamesComplete()
    {
        StartCoroutine(CustomerExit());
    }

    void ShowOrderScreen()
    {
        orderScreenPanel.SetActive(true);
        makeButton.interactable = false;
    }

    void ShowMinigamePanel()
    {
        orderScreenPanel.SetActive(false);
    }

    void PickRandomOrder()
    {
        if (possibleOrders.Length == 0)
        {
            Debug.LogWarning("No orders assigned to OrderScreenManager.");
            return;
        }

        currentOrder = possibleOrders[Random.Range(0, possibleOrders.Length)];
        orderText.text = currentOrder.orderText;
        drinkIcon.sprite = currentOrder.drinkIcon;
        drinkIcon.gameObject.SetActive(currentOrder.drinkIcon != null);
    }

    void OnMakePressed()
    {
        makeButton.interactable = false;
        makeButton.gameObject.SetActive(false);
        ShowMinigamePanel();
    }

    IEnumerator CustomerEnter()
    {
        // reset positions
        customerImage.anchoredPosition = new Vector2(slideInX, customerImage.anchoredPosition.y);
        signRect.anchoredPosition = new Vector2(signRect.anchoredPosition.x, signOnScreenY + signSlideOffset);
        makeButton.gameObject.SetActive(false);
        makeButton.interactable = false;

        PickRandomOrder();

        // slide customer in
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

        // slide sign down
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

        // pop make button in
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

        ShowOrderScreen();
        StartCoroutine(CustomerEnter());
    }
}
