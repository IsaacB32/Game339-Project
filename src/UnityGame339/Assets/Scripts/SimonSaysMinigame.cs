using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SimonSaysMinigame : MinigameBase
{
    [Space]
    [SerializeField] private GameObject _coffeeCup;
    [SerializeField] private GameObject _finalSpot;
    [SerializeField] private PumpButton[] _buttons;
    [SerializeField] private GameObject _waitImage;
    [SerializeField] private GameObject _goImage;
    [Space]
    [SerializeField] private Slider[] _sliders;

    private int _totalSequence = 2; //how many buttons in the sequence
    private float _flashDuration = 0.5f;

    private List<PumpButton> _sequence = new List<PumpButton>();
    private int _currentSequenceIndex = 0;

    private void Start()
    {
        StartMinigame();
    }

    protected override void BeginMinigame()
    {
        foreach (Slider slider in _sliders)
        {
            slider.value = 0f;
            slider.enabled = false;
        }
        
        foreach (PumpButton pumpButton in _buttons)
        {
            pumpButton.SetActive(false);
        }
        ShowWait(); 
        FindSequence();
        StartCoroutine(Minigame());
        return;

        IEnumerator Minigame()
        {
            yield return new WaitForSeconds(1f); //introduction
            int index = 0;
            while (index < _totalSequence)
            {
                yield return StartCoroutine(_sequence[index].Flash(_flashDuration));
                yield return new WaitForSeconds(0.1f);
                index++;
                yield return null;
            }
            yield return new WaitForSeconds(.2f);
            ShowGo();
            foreach (PumpButton pumpButton in _buttons)
            {
                pumpButton.SetActive(true);
            }
        }
    }

    public void PressPump(PumpButton button)
    {
        if (button == _sequence[_currentSequenceIndex])
        {
            _currentSequenceIndex++;
            scoreService.DayScore.Value += 10;
        }
        else
        {
            StartCoroutine(_buttons[0].FlashWrong(EndMinigame));
            for (int i = 1; i < _buttons.Length; i++)
            {
                StartCoroutine(_buttons[i].FlashWrong());
            }
        }

        if (_currentSequenceIndex >= _totalSequence)
        {
            foreach (PumpButton pumpButton in _buttons)
            {
                pumpButton.SetActive(false);
            }
            StartCupAnimation();
        }
    }

    private void FindSequence()
    {
        _sequence = new List<PumpButton>();
        for (int i = 0; i < _totalSequence; i++)
        {
            _sequence.Add(_buttons[Random.Range(0, _buttons.Length)]);
        }
    }

    private void ShowWait()
    {
        _goImage.SetActive(false);
        _waitImage.SetActive(true);
    }

    private void ShowGo()
    {
        _waitImage.SetActive(false);
        _goImage.SetActive(true);
    }

    private void StartCupAnimation()
    {
        StartCoroutine(CupAnimation());
        return;

        IEnumerator CupAnimation()
        {
            float elapsed, timeToTake = 0.5f;
            float starting, target;
            foreach (PumpButton pumpButton in _sequence)
            {
                int index = pumpButton.PumpIndex;
                starting = _coffeeCup.transform.position.x;
                target = pumpButton.Pos.x;
                elapsed = 0;
                while (elapsed < timeToTake)
                {
                    elapsed += Time.deltaTime;
                    Vector2 temp = _coffeeCup.transform.position;
                    temp.x = Mathf.Lerp(starting, target, elapsed / timeToTake);
                    _coffeeCup.transform.position = temp;
                    yield return null;
                }
                Vector2 temp2 = _coffeeCup.transform.position;
                temp2.x = target;
                _coffeeCup.transform.position = temp2;
                yield return new WaitForSeconds(0.1f);
                
                elapsed = 0;
                _sliders[index].enabled = true;
                while (elapsed < 0.4)
                {
                    elapsed += Time.deltaTime;
                    _sliders[index].value = Mathf.Lerp(0f, 1f, elapsed / 0.4f);
                    yield return null;
                }
                yield return new WaitForSeconds(0.2f);
                _sliders[index].enabled = false;
                _sliders[index].value = 0f;
                yield return new WaitForSeconds(0.5f);
            }

            timeToTake += 0.3f;
            starting = _coffeeCup.transform.position.x;
            target = _finalSpot.transform.position.x;
            elapsed = 0;
            while (elapsed < timeToTake)
            {
                elapsed += Time.deltaTime;
                Vector2 temp = _coffeeCup.transform.position;
                temp.x = Mathf.Lerp(starting, target, elapsed / timeToTake);
                _coffeeCup.transform.position = temp;
                yield return null;
            }
            Vector2 temp3 = _coffeeCup.transform.position;
            temp3.x = target;
            _coffeeCup.transform.position = temp3;
            yield return new WaitForSeconds(1f);
            EndMinigame();
        }
    }

    public override void ApplyDifficulty(float curseLevel)
    {
        Debug.Log("curse level: " + curseLevel);
    }
}
