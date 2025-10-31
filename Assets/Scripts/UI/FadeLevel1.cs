using UnityEngine;
using UnityEngine.UI; 
using System.Collections;

public class FadeLevel1 : MonoBehaviour
{
    [SerializeField] private Image _fadePanel;
    [SerializeField] private float _fadeDuration = 1.5f;

    void Start()
    {
        if (_fadePanel == null)
        {
            return;
        }
        StartCoroutine(FadeInRoutine());
    }

    
    private IEnumerator FadeInRoutine()
    {
        _fadePanel.gameObject.SetActive(true);
        Color panelColor = _fadePanel.color;
        panelColor.a = 1;
        _fadePanel.color = panelColor;

        float timer = 0f;
        while (timer < _fadeDuration)
        {
            timer += Time.deltaTime;

            panelColor.a = Mathf.Lerp(1f, 0f, timer / _fadeDuration);
            _fadePanel.color = panelColor;

            yield return null; 
        }

        panelColor.a = 0;
        _fadePanel.color = panelColor;
        _fadePanel.gameObject.SetActive(false);
    }
}