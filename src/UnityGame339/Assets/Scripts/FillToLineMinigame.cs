using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FillToLineMinigame : MinigameBase
{
    [Header("References")]
    [SerializeField] private Button _startFillButton;
    [SerializeField] private Slider _slider;
    [SerializeField] private GameObject _liquidFiller;
    [SerializeField] private TextMeshProUGUI _scoreText;
    private TextMeshProUGUI _fillButtonText;

    [Header("Values")]
    [SerializeField] private float _fillRate;

    private const float PerfectScoreFillAmount = 0.852f;
    private const float ScoreMultiplier = 10000f;


    private bool _filling = false;

    new void Awake()
    {
        base.Awake();
        _fillButtonText = _startFillButton.GetComponentInChildren<TextMeshProUGUI>();
        _startFillButton.interactable = true;
        _fillButtonText.text = "Hold to Pour"; 
        _liquidFiller.SetActive(false);
        _slider.value = 0f;
        // _scoreText.gameObject.SetActive(false);
    }

    protected override void BeginMinigame()
    {
        
    }

    public void ButtonDown()
    {
        if (!_startFillButton.interactable) return;
        _startFillButton.interactable = true;
        _fillButtonText.text = "Filling...";
        _liquidFiller.SetActive(true);
        _filling = true;
        StartCoroutine(FillCup());
    }

    public void ButtonUp()
    {
        if (!_startFillButton.interactable) return;
        _fillButtonText.text = "stop";
        _startFillButton.interactable = false;
        _liquidFiller.SetActive(false);
        _filling = false;

        float finalFill = _slider.value;
        float score = Mathf.Round(Mathf.Abs(PerfectScoreFillAmount - finalFill) * ScoreMultiplier);
        _scoreText.gameObject.SetActive(true);
        _scoreText.text = score.ToString();

        StartCoroutine(WaitToEnd());
    }

    IEnumerator WaitToEnd()
    {
        yield return new WaitForSeconds(3f);
        EndMinigame();
    }

    IEnumerator FillCup()
    {
        while (_filling)
        {
            _slider.value += _fillRate * Time.deltaTime;
            yield return null;
        }
    }
}
