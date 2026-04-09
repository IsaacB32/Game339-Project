using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DrinkGiveSequence : MonoBehaviour
{
    [Header("Drink")]
    public RectTransform drinkRect;
    public Image drinkImage;
    public Vector2 drinkStartPos;
    public Vector2 drinkGivePos;
    public float drinkSize = 1f;
    public float drinkMoveDuration = 0.5f;
    public float drinkPopDuration = 0.3f;

    [Header("Give Button")]
    public Button giveButton;

    [Header("Capture")]
    public CaptureBallManager captureBall;
    public float shrinkDuration = 1.5f;
    public float pauseBeforeCapture = 0.5f;
    public float captureSpinSpeed = 360f;

    private RectTransform _customerImage;
    private CustomerOrder _currentOrder;
    private Action _onSequenceComplete;

    public void StartSequence(RectTransform customerImage, CustomerOrder order, Action onComplete)
    {
        _customerImage = customerImage;
        _currentOrder = order;
        _onSequenceComplete = onComplete;

        drinkImage.sprite = order.drinkIcon;
        drinkRect.sizeDelta = order.drinkIconSize * drinkSize;
        drinkRect.anchoredPosition = drinkStartPos;
        drinkRect.gameObject.SetActive(true);
        drinkRect.localScale = Vector3.zero;

        StartCoroutine(PopIn());
    }

    IEnumerator PopIn()
    {
        float elapsed = 0f;
        while (elapsed < drinkPopDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / drinkPopDuration);
            drinkRect.localScale = Vector3.one * t;
            yield return null;
        }
        drinkRect.localScale = Vector3.one;

        giveButton.gameObject.SetActive(true);
        giveButton.interactable = true;
        giveButton.onClick.AddListener(OnGivePressed);
    }

    IEnumerator PopOut()
    {
        float elapsed = 0f;
        while (elapsed < drinkPopDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / drinkPopDuration);
            drinkRect.localScale = Vector3.one * (1f - t);
            yield return null;
        }
        drinkRect.localScale = Vector3.zero;
        drinkRect.gameObject.SetActive(false);
    }

    void OnGivePressed()
    {
        giveButton.onClick.RemoveListener(OnGivePressed);
        giveButton.gameObject.SetActive(false);
        StartCoroutine(GiveAndCapture());
    }

    IEnumerator GiveAndCapture()
    {
        Vector2 startPos = drinkRect.anchoredPosition;
        float elapsed = 0f;
        while (elapsed < drinkMoveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / drinkMoveDuration);
            drinkRect.anchoredPosition = Vector2.Lerp(startPos, drinkGivePos, t);
            yield return null;
        }
        drinkRect.anchoredPosition = drinkGivePos;

        yield return StartCoroutine(PopOut());

        Animator customerAnimator = _customerImage.GetComponent<Animator>();
        if (customerAnimator) customerAnimator.enabled = false;
        CustomerFlipController flipController = _customerImage.GetComponent<CustomerFlipController>();
        if (flipController) flipController.enabled = false;

        yield return new WaitForSeconds(pauseBeforeCapture);

        int originalSiblingIndex = _customerImage.GetSiblingIndex();
        Transform originalParent = _customerImage.parent;
        _customerImage.SetParent(captureBall.ballContainer, worldPositionStays: true);
        _customerImage.SetSiblingIndex(captureBall.CaptureSiblingIndex);

        Vector2 miniSize = _currentOrder.customerSize * captureBall.customerScale;
        float aspect = _currentOrder.customerSize.x / _currentOrder.customerSize.y;
        float targetHeight = miniSize.y;
        float targetWidth = targetHeight * aspect;
        if (targetWidth > miniSize.x)
        {
            targetWidth = miniSize.x;
            targetHeight = targetWidth / aspect;
        }

        Vector3 customerStartScale = _customerImage.localScale;
        Vector2 customerStartSize = _customerImage.sizeDelta;
        Vector2 customerWorldStart = _customerImage.position;
        Vector2 ballTarget = captureBall.ballContainer.position;
        float scaleRatio = targetWidth / customerStartSize.x;

        elapsed = 0f;
        while (elapsed < shrinkDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / shrinkDuration);

            float currentScale = Mathf.Lerp(1f, scaleRatio, t);
            _customerImage.localScale = customerStartScale * currentScale;
            _customerImage.position = Vector2.Lerp(customerWorldStart, ballTarget, t);
            _customerImage.localEulerAngles = new Vector3(0f, 0f, captureSpinSpeed * elapsed);

            yield return null;
        }

        _customerImage.localScale = customerStartScale;
        _customerImage.localEulerAngles = Vector3.zero;
        _customerImage.SetParent(originalParent, worldPositionStays: true);
        _customerImage.SetSiblingIndex(originalSiblingIndex);
        if (customerAnimator) customerAnimator.enabled = true;
        if (flipController) flipController.enabled = true;
        _customerImage.gameObject.SetActive(false);

        captureBall.AddCustomer(_currentOrder.customerSprite, _currentOrder.customerSize);
        
        yield return new WaitForSeconds(1f);

        _onSequenceComplete?.Invoke();
    }
}