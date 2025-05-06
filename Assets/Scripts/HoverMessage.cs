using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverFadeMessage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject messageObject; 
    public float fadeDuration = 0.3f;

    private Graphic[] graphics;
    private Coroutine fadeCoroutine;

    void Awake()
    {
        if (messageObject != null)
        {
            graphics = messageObject.GetComponentsInChildren<Graphic>(true); 
            foreach (var g in graphics)
                SetAlpha(g, 0f);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (messageObject == null) return;
        messageObject.SetActive(true);
        StartFade(1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (messageObject == null) return;
        StartFade(0f);
    }

    private void StartFade(float targetAlpha)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeTo(targetAlpha));
    }

    private System.Collections.IEnumerator FadeTo(float targetAlpha)
    {
        float[] startAlphas = new float[graphics.Length];
        for (int i = 0; i < graphics.Length; i++)
            startAlphas[i] = graphics[i].color.a;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;
            for (int i = 0; i < graphics.Length; i++)
                SetAlpha(graphics[i], Mathf.Lerp(startAlphas[i], targetAlpha, t));
            yield return null;
        }

        for (int i = 0; i < graphics.Length; i++)
            SetAlpha(graphics[i], targetAlpha);

        if (targetAlpha == 0f)
            messageObject.SetActive(false);
    }

    private void SetAlpha(Graphic g, float alpha)
    {
        Color c = g.color;
        c.a = alpha;
        g.color = c;
    }
}
