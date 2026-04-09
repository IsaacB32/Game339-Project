using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panel")]
    public GameObject menuPanel;

    [Header("Buttons")]
    public Button startButton;
    public Button quitButton;

    [Header("References")]
    public GameManager gameManager;
    public BackstoryScreen backstoryScreen;
    public CanvasGroup transitionOverlay;
    public float transitionDuration = 0.3f;

    void Start()
    {
        menuPanel.SetActive(true);
        startButton.onClick.AddListener(OnStartPressed);
        quitButton.onClick.AddListener(OnQuitPressed);
        transitionOverlay.alpha = 0f;
        transitionOverlay.blocksRaycasts = false;
    }

    void OnStartPressed()
    {
        startButton.interactable = false;
        StartCoroutine(TransitionToBackstory());
    }

    IEnumerator TransitionToBackstory()
    {
        yield return StartCoroutine(Fade(0f, 1f));
        menuPanel.SetActive(false);
        backstoryScreen.Show(() => gameManager.StartGame());
        yield return StartCoroutine(Fade(1f, 0f));
    }

    IEnumerator Fade(float from, float to)
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

    void OnQuitPressed()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}