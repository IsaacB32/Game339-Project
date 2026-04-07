using System;
using System.Collections;
using Game.Runtime;
using Game339.Shared.Diagnostics;
using TMPro;
using UnityEngine;

public class SpeechBubbleView : MonoBehaviour
{
    private static readonly int Start = Animator.StringToHash("start");
    
    private TextMeshProUGUI _text;
    private Animator _animator;

    private const float TIME_PER_CHAR = 0.04f;

    private void Awake()
    {
        _text = GetComponentInChildren<TextMeshProUGUI>();
        _animator = GetComponent<Animator>();
    }

    public void UpdateText(string text)
    {
        _text.maxVisibleCharacters = 0;
        _text.text = text;
    }

    public IEnumerator TextAnimation()
    {
        _animator.SetTrigger(Start);
        yield return new WaitForSeconds(0.8f);
        
        while (_text.maxVisibleCharacters < _text.text.Length)
        {
            _text.maxVisibleCharacters++;
            yield return new WaitForSeconds(TIME_PER_CHAR);
        }
        _text.maxVisibleCharacters = _text.text.Length;
    }
}
