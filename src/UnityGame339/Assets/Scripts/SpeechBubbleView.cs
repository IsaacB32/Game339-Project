using System;
using System.Collections;
using Game.Runtime;
using Game339.Shared.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeechBubbleView : MonoBehaviour
{
    private static readonly int Start = Animator.StringToHash("start");
    
    private TextMeshProUGUI _text;
    private Animator _animator;
    private Image _image;

    private const float TIME_PER_CHAR = 0.04f;

    private void Awake()
    {
        _text = GetComponentInChildren<TextMeshProUGUI>();
        _animator = GetComponent<Animator>();
        _image = GetComponent<Image>();
        
        _animator.enabled = false;
    }

    public void UpdateText(string text)
    {
        _text.maxVisibleCharacters = 0;
        _text.text = text;
    }

    public void Hide()
    {
        Color temp = _image.color;
        temp.a = 0f;
        _image.color = temp;
        UpdateText("");
    }

    public void ShowText()
    {
        Color temp = _image.color;
        temp.a = 1f;
        _image.color = temp;
        _text.maxVisibleCharacters = _text.text.Length;
    }

    public IEnumerator TextAnimation()
    {
        _animator.enabled = true;
        _animator.SetTrigger(Start);
        yield return new WaitForSeconds(1.3f);
        
        while (_text.maxVisibleCharacters < _text.text.Length)
        {
            _text.maxVisibleCharacters++;
            yield return new WaitForSeconds(TIME_PER_CHAR);
        }
        _text.maxVisibleCharacters = _text.text.Length;
        _animator.enabled = false;
    }
}
