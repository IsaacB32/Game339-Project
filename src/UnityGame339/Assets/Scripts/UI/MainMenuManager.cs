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

    void Start()
    {
        menuPanel.SetActive(true);
        startButton.onClick.AddListener(OnStartPressed);
        quitButton.onClick.AddListener(OnQuitPressed);
    }

    void OnStartPressed()
    {
        menuPanel.SetActive(false);
        gameManager.StartGame();
    }

    void OnQuitPressed()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}