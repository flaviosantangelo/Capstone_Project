using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class FadingFlashingUI : MonoBehaviour
{
    public float _flashSpeed = 1.5f;
    private CanvasGroup _canvasGroup;

    void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        float alpha = (Mathf.Sin(Time.time * _flashSpeed) + 1f) / 2f;
        _canvasGroup.alpha = alpha;
    }
}