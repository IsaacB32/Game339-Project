using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaptureBallManager : MonoBehaviour
{
    [Header("Ball")]
    public RectTransform ballContainer;
    public float ballRadius = 100f;

    [Header("Mini Customer Settings")]
    public float customerScale = 0.08f;
    public float gravity = -20f;
    public float bounceDamping = 0.7f;
    public float maxStartSpeed = 60f;

    [Header("Slide")]
    public float offScreenX = 600f;
    public float slideDuration = 0.5f;

    private float _onScreenX;
    private List<MiniCustomer> _miniCustomers = new List<MiniCustomer>();

    private class MiniCustomer
    {
        public RectTransform rect;
        public Vector2 velocity;
        public float angularVelocity;
    }

    void Awake()
    {
        _onScreenX = ballContainer.anchoredPosition.x;
    }

    public IEnumerator SlideIn()
    {
        yield return Slide(_onScreenX + offScreenX, _onScreenX);
    }

    public IEnumerator SlideOut()
    {
        yield return Slide(_onScreenX, _onScreenX + offScreenX);
    }

    IEnumerator Slide(float fromX, float toX)
    {
        float elapsed = 0f;
        float currentY = ballContainer.anchoredPosition.y;
        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / slideDuration);
            ballContainer.anchoredPosition = new Vector2(Mathf.Lerp(fromX, toX, t), currentY);
            yield return null;
        }
        ballContainer.anchoredPosition = new Vector2(toX, currentY);
    }

    public void AddCustomer(Sprite customerSprite, Vector2 originalSize)
    {
        GameObject obj = new GameObject("MiniCustomer");
        obj.transform.SetParent(ballContainer, false);

        Image img = obj.AddComponent<Image>();
        img.sprite = customerSprite;
        img.preserveAspect = true;
        img.raycastTarget = false;

        RectTransform rect = obj.GetComponent<RectTransform>();

        float aspect = originalSize.x / originalSize.y;
        float targetHeight = originalSize.y * customerScale;
        float targetWidth = targetHeight * aspect;
        float maxSize = Mathf.Max(originalSize.x, originalSize.y) * customerScale;
        if (targetWidth > maxSize)
        {
            targetWidth = maxSize;
            targetHeight = targetWidth / aspect;
        }

        rect.sizeDelta = new Vector2(targetWidth, targetHeight);
        rect.anchoredPosition = Vector2.zero;

        MiniCustomer mini = new MiniCustomer
        {
            rect = rect,
            velocity = new Vector2(
                Random.Range(-maxStartSpeed, maxStartSpeed),
                Random.Range(0f, maxStartSpeed)),
            angularVelocity = Random.Range(-90f, 90f)
        };

        _miniCustomers.Add(mini);
    }

    void Update()
    {
        foreach (var mini in _miniCustomers)
        {
            mini.velocity.y += gravity * Time.deltaTime;

            Vector2 pos = mini.rect.anchoredPosition;
            pos += mini.velocity * Time.deltaTime;

            Vector3 rot = mini.rect.localEulerAngles;
            rot.z += mini.angularVelocity * Time.deltaTime;
            mini.rect.localEulerAngles = rot;

            float dist = pos.magnitude;
            float customerRadius = Mathf.Max(mini.rect.sizeDelta.x, mini.rect.sizeDelta.y) * 0.5f;
            float maxDist = ballRadius - customerRadius;

            if (dist > maxDist && maxDist > 0f)
            {
                Vector2 normal = pos.normalized;
                pos = normal * maxDist;

                float dotVel = Vector2.Dot(mini.velocity, normal);
                if (dotVel > 0f)
                {
                    mini.velocity -= 2f * dotVel * normal;
                    float speedChange = Random.Range(0.9f, 1.10f);
                    mini.velocity *= speedChange;
                    mini.angularVelocity *= speedChange;
                }
            }

            mini.rect.anchoredPosition = pos;
        }
    }

    public void SetVisible(bool visible)
    {
        ballContainer.gameObject.SetActive(visible);
    }
}