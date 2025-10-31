using System.Collections;
using UnityEngine;
using UnityEngine.UI; 

public class FadeManager : MonoBehaviour
{
    public static FadeManager _instance; 
    public Image _fadePanel; 
    public float _fadeDuration = 1.5f;

    void Awake()
    {
        if (_instance == null) _instance = this;
    }

    void Start()
    {
        StartFadeIn();
    }

    public void StartFadeIn()
    {
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