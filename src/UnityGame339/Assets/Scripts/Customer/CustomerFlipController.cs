using System.Collections;
using UnityEngine;

public class CustomerFlipController : MonoBehaviour
{
    [Header("Flip Settings")]
    public float minWaitBetweenFlips = 2f;
    public float maxWaitBetweenFlips = 6f;
    public float flipDuration = 0.2f;

    private float _scaleX = -1f;

    void OnEnable()
    {
        _scaleX = -1f;
        StartCoroutine(FlipLoop());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    void LateUpdate()
    {
        Vector3 s = transform.localScale;
        s.x = _scaleX;
        transform.localScale = s;
    }

    IEnumerator FlipLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minWaitBetweenFlips, maxWaitBetweenFlips));
            yield return StartCoroutine(Flip());
        }
    }

    IEnumerator Flip()
    {
        float from = _scaleX;
        float to = -_scaleX;
        float elapsed = 0f;

        while (elapsed < flipDuration)
        {
            elapsed += Time.deltaTime;
            _scaleX = Mathf.Lerp(from, to, elapsed / flipDuration);
            yield return null;
        }

        _scaleX = to;
    }
}
