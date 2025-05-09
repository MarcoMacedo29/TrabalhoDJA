using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AnimationController1 : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform animationOverlay;
    public RectTransform bag;
    public RectTransform itemSlot;
    public GameObject starObject;

    [Header("Reward Reference")]
    public RectTransform rewardItemSlot;  // Slot where the item should return after animation

    [Header("Timing Settings")]
    public float bagEnterDuration = 0.5f;
    public float itemFlyDuration = 0.5f;
    public float starBlinkDuration = 0.8f;

    private Vector2 bagTargetPos;
    private Transform originalParent;
    private Vector2 originalAnchoredPos;

    private void Awake()
    {
        // Cache original parent/position of the item
        originalParent = itemSlot.parent;
        originalAnchoredPos = itemSlot.anchoredPosition;

        bagTargetPos = bag.anchoredPosition;
        bag.gameObject.SetActive(false);
        starObject.SetActive(false);
    }

    public void PlayKeepAnimation()
    {
        // Detach item and move it visually under the overlay
        itemSlot.SetParent(animationOverlay, true);
        itemSlot.pivot = bag.pivot;

        StartCoroutine(KeepSequence(itemSlot));
    }

    private IEnumerator KeepSequence(RectTransform item)
    {
        // 1) Prepare and slide in the bag from the right
        bag.gameObject.SetActive(true);

        Vector2 bagStartPos = new Vector2(
            animationOverlay.rect.width + bag.rect.width,
            bagTargetPos.y
        );
        bag.anchoredPosition = bagStartPos;

        yield return LerpAnchored(bag, bagTargetPos, bagEnterDuration);

        // 2) Convert bag position to local space of animation overlay
        Vector3 worldBagPos = bag.position;
        item.SetParent(animationOverlay, true); // Keep world position
        item.pivot = bag.pivot;

        Vector2 localBagPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            animationOverlay,
            RectTransformUtility.WorldToScreenPoint(null, worldBagPos),
            null,
            out localBagPos
        );

        // 3) Move item straight to bag
        Vector2 startPos = item.anchoredPosition;
        Vector2 targetPos = localBagPos;

        yield return LerpAnchored(item, targetPos, itemFlyDuration);

        // 4) Re-parent item to bag and center
        item.SetParent(bag, false);
        item.anchoredPosition = Vector2.zero;

        // 4.5) Smoothly shrink and fade out the item
        CanvasGroup itemGroup = item.GetComponent<CanvasGroup>();
        if (itemGroup == null)
            itemGroup = item.gameObject.AddComponent<CanvasGroup>();

        Vector3 originalScale = item.localScale;
        float shrinkDuration = 0.5f;
        float t = 0f;

        while (t < shrinkDuration)
        {
            t += Time.deltaTime;
            float normalized = t / shrinkDuration;
            item.localScale = Vector3.Lerp(originalScale, Vector3.zero, normalized);
            itemGroup.alpha = Mathf.Lerp(1f, 0f, normalized);
            yield return null;
        }

        // 5) Activate and start blinking the star after the item shrink is complete
        starObject.SetActive(true);
        yield return StartCoroutine(BlinkStar(starObject, starBlinkDuration));
        yield return new WaitForSeconds(starBlinkDuration);
        starObject.SetActive(false);

        // 6) Hide bag
        bag.gameObject.SetActive(false);

        // 7) Return item to the reward slot
        item.SetParent(rewardItemSlot, false);
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
