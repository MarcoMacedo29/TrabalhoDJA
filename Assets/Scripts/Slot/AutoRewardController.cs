using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AutoRewardAnim : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform autoRewardOverlay;
    public RectTransform bag;
    public RectTransform gemSlot;
    public GameObject starObject;

    [Header("Reward Reference")]
    public RectTransform rewardItemGem;

    [Header("Timing Settings")]
    public float bagEnterDuration = 0.5f;
    public float itemFlyDuration = 0.5f;
    public float starBlinkDuration = 0.8f;
    public float itemShrinkDuration = 0.5f;

    [Header("Trigger Settings")]
    public float startDelay = 0.2f;

    private Vector2 bagTargetPos;
    private Transform originalParent;
    private Vector2 originalAnchoredPos;
    private bool isAnimating = false;

    private void Awake()
    {
        originalParent = gemSlot.parent;
        originalAnchoredPos = gemSlot.anchoredPosition;

        bagTargetPos = bag.anchoredPosition;
        bag.gameObject.SetActive(false);
        starObject.SetActive(false);
    }

    /*
    private void OnEnable()
    {
        JackpotManager.OnGemCollected += HandleGemCollected;
    }

    private void OnDisable()
    {
        JackpotManager.OnGemCollected -= HandleGemCollected;
    }
    */

    public void TriggerAnim()
    {
        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(startDelay);
        PlayKeepAnimation();
    }

    public void PlayKeepAnimation()
    {
        if (isAnimating) return;
        isAnimating = true;

        gemSlot.SetParent(autoRewardOverlay, true);
        gemSlot.pivot = bag.pivot;

        StartCoroutine(KeepSequence(gemSlot));
    }

    private IEnumerator KeepSequence(RectTransform item)
    {
        bag.gameObject.SetActive(true);

        Vector2 bagStartPos = new Vector2(
            -autoRewardOverlay.rect.width - bag.rect.width,
            bagTargetPos.y
        );
        bag.anchoredPosition = bagStartPos;

        yield return LerpAnchored(bag, bagTargetPos, bagEnterDuration);

        Vector3 worldBagPos = bag.position;
        item.SetParent(autoRewardOverlay, true);
        item.pivot = bag.pivot;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            autoRewardOverlay,
            RectTransformUtility.WorldToScreenPoint(null, worldBagPos),
            null,
            out Vector2 localBagPos
        );

        yield return LerpAnchored(item, localBagPos, itemFlyDuration);

        item.SetParent(bag, false);
        item.anchoredPosition = Vector2.zero;

        CanvasGroup itemGroup = item.GetComponent<CanvasGroup>() ?? item.gameObject.AddComponent<CanvasGroup>();
        Vector3 originalScale = item.localScale;
        float t = 0f;

        while (t < itemShrinkDuration)
        {
            t += Time.deltaTime;
            float normalized = t / itemShrinkDuration;
            item.localScale = Vector3.Lerp(originalScale, Vector3.zero, normalized);
            itemGroup.alpha = Mathf.Lerp(1f, 0f, normalized);
            yield return null;
        }

        starObject.SetActive(true);
        yield return StartCoroutine(BlinkStar(starObject, starBlinkDuration));
        yield return new WaitForSeconds(starBlinkDuration);
        starObject.SetActive(false);

        bag.gameObject.SetActive(false);

        item.SetParent(rewardItemGem, false);
        item.anchoredPosition = Vector2.zero;
        item.localScale = originalScale;
        itemGroup.alpha = 1f;

        Image img = item.GetComponent<Image>();
        if (img != null)
        {
            Color col = img.color;
            col.a = 0f;
            img.color = col;
        }

        isAnimating = false;
    }

    private IEnumerator BlinkStar(GameObject star, float blinkDuration)
    {
        Vector3 originalScale = star.transform.localScale;
        Vector3 targetScale = originalScale * 0.5f;
        float elapsed = 0f;

        while (elapsed < blinkDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.PingPong(elapsed * 2f / blinkDuration, 1f);
            star.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        star.transform.localScale = originalScale;
    }

    private IEnumerator LerpAnchored(RectTransform rt, Vector2 to, float duration)
    {
        Vector2 from = rt.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);
            rt.anchoredPosition = Vector2.Lerp(from, to, t);
            yield return null;
        }

        rt.anchoredPosition = to;
    }
}
