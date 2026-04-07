using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimonSaysMinigame : MinigameBase
{
    [Space]
    [SerializeField] private PumpButton[] _buttons;
    [SerializeField] private GameObject _waitImage;
    [SerializeField] private GameObject _goImage;

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
                yield return new WaitForSeconds(0.15f);
                index++;
                yield return null;
            }
            yield return new WaitForSeconds(.55f);
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
            EndMinigame();
        }
    }

    private void FindSequence()
    {
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

    public override void ApplyDifficulty(float curseLevel)
    {
        throw new System.NotImplementedException();
    }
}
