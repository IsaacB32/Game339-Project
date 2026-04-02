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
    private TextMeshProUGUI _fillButtonText;

    [Header("Values")]
    [SerializeField] private float _fillRate;

    private const float PerfectScoreFillAmount = 0.852f;
    private const float MaxScore = 100f;

    private float _fillAmount;
    private bool _filling = false;
    private bool _poured = false;

    protected override void Init()
    {
        _fillButtonText = _startFillButton.GetComponentInChildren<TextMeshProUGUI>();
        _slider.maxValue = 1f;
    }

    protected override void BeginMinigame()
    {
        _fillAmount = 0f;
        _filling = false;
        _poured = false;
        _startFillButton.interactable = true;
        _fillButtonText.text = "Hold to Pour";
        _liquidFiller.SetActive(false);
        _slider.value = 0f;
    }

    public void ButtonDown()
    {
        if (_poured || !_startFillButton.interactable) return;
        _fillButtonText.text = "Filling...";
        _liquidFiller.SetActive(true);
        _filling = true;
        StartCoroutine(FillCup());
    }

    public void ButtonUp()
    {
        if (_poured || !_filling) return;
        _poured = true;
        _filling = false;
        _startFillButton.interactable = false;
        _liquidFiller.SetActive(false);
        _fillButtonText.text = "Done";

        float error = Mathf.Abs(PerfectScoreFillAmount - _fillAmount);
        scoreService.DayScore.Value += Mathf.RoundToInt(Mathf.Max(0f, MaxScore - error * MaxScore / PerfectScoreFillAmount));
        
        EndMinigame();
    }

    IEnumerator FillCup()
    {
        while (_filling)
        {
            _fillAmount += _fillRate * Time.deltaTime;
            _slider.value = Mathf.Min(_fillAmount, 1f);
            yield return null;  
        }
    }
}