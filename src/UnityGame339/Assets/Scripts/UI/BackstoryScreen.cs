using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BackstoryScreen : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panel;

    [Header("UI")]
    public TextMeshProUGUI storyText;
    public Button nextButton;
    public TextMeshProUGUI nextButtonText;

    [Header("Typewriter")]
    public float charsPerSecond = 30f;
    public string finalButtonLabel = "Begin";

    [Header("Story Pages")]
    [TextArea(3, 8)]
    public string[] pages;

    private int _pageIndex;
    private bool _typing;
    private Coroutine _typeCoroutine;

    void Awake()
    {
        panel.SetActive(false);
    }

    public void Show(System.Action onComplete)
    {
        panel.SetActive(true);
        _pageIndex = 0;
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(() => OnNextPressed(onComplete));
        nextButton.gameObject.SetActive(false);
        StartCoroutine(TypePage(pages[_pageIndex]));
    }

    void OnNextPressed(System.Action onComplete)
    {
        if (_typing)
        {
            StopCoroutine(_typeCoroutine);
            storyText.text = pages[_pageIndex];
            _typing = false;
            return;
        }

        _pageIndex++;

        if (_pageIndex >= pages.Length)
        {
            onComplete?.Invoke();
            panel.SetActive(false);
            return;
        }

        bool isLast = _pageIndex == pages.Length - 1;
        nextButtonText.text = isLast ? finalButtonLabel : "Next";
        nextButton.gameObject.SetActive(false);
        _typeCoroutine = StartCoroutine(TypePage(pages[_pageIndex]));
    }

    IEnumerator TypePage(string text)
    {
        _typing = true;
        storyText.text = "";
        float delay = 1f / charsPerSecond;

        bool isLast = _pageIndex == pages.Length - 1;
        nextButtonText.text = isLast ? finalButtonLabel : "Next";

        for (int i = 0; i < text.Length; i++)
        {
            storyText.text += text[i];
            yield return new WaitForSeconds(delay);
        }

        _typing = false;
        nextButton.gameObject.SetActive(true);
    }
}