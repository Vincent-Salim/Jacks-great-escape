using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;
    [SerializeField] private Image fadeOverlay;

    void Awake()
    {
        Instance = this;
        if (fadeOverlay != null)
            fadeOverlay.color = new Color(0, 0, 0, 0);
    }

    public IEnumerator FadeOut(float duration = 1)
    {
        float elapsed = 0f;
        Color color = fadeOverlay.color;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsed / duration);
            fadeOverlay.color = color;
            yield return null;
        }
    }

    public IEnumerator FadeIn(float duration = 1)
    {
        float elapsed = 0f;
        Color color = fadeOverlay.color;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(1f - elapsed / duration);
            fadeOverlay.color = color;
            yield return null;
        }
    }
}