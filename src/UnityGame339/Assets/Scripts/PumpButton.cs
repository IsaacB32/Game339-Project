using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PumpButton : MonoBehaviour
{
    [SerializeField] private Color _flashColor;
    private Color _startingColor;
    private Image _image;
    private Button _button;

    [SerializeField] private int _pumpIndex;
    public int PumpIndex => _pumpIndex;
    [SerializeField] private GameObject _pos;
    public Vector2 Pos => _pos.transform.position;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _button = GetComponent<Button>();
        _startingColor = _image.color;
    }

    public IEnumerator Flash(float duration)
    {
        _image.color = _flashColor;
        yield return new WaitForSeconds(duration);
        _image.color = _startingColor;
    }
    
    public IEnumerator FlashWrong(Action finished = null)
    {
        for (int i = 0; i < 4; i++)
        {
            _image.color = Color.red;
            yield return new WaitForSeconds(0.35f);
            _image.color = _startingColor;
            yield return null;
        }
        finished?.Invoke();
    }

    public void SetActive(bool value)
    {
        _button.interactable = value;
    }
}
