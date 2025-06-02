using UnityEngine;
using TMPro;
using System.Collections;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    [Header("UI Reference")]
    public TextMeshProUGUI coinText;

    [Header("Settings")]
    [Tooltip("How many coins the player always starts (and resets) with")]
    public int defaultCoins = 100;

    private int coins;
    public int Coins => coins;

    [Header("Floating Text Prefab")]
    [Tooltip("Prefab: a simple TextMeshProUGUI that will float up when coins change")]
    public GameObject floatingTextPrefab;

    [Header("Animation Settings")]
    [Tooltip("How far (in pixels) the floating text moves upward")]
    public float floatDistance = 50f;
    [Tooltip("How long (in seconds) the floating text animation lasts")]
    public float floatDuration = 1f;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            coins = defaultCoins;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        UpdateUI();
    }

    /// <summary>
    /// Starts a “–amount” float animation, then deducts coins after it finishes.
    /// </summary>
    public void SpendCoins(int amount)
    {
        if (amount <= 0) return;
        StartCoroutine(ShowThenApplyDelta(-amount));
    }

    /// <summary>
    /// Starts a “+amount” float animation, then adds coins after it finishes.
    /// </summary>
    public void AddCoins(int amount)
    {
        if (amount <= 0) return;
        StartCoroutine(ShowThenApplyDelta(+amount));
    }

    /// <summary>
    /// Immediately sets the coin total (no animation). 
    /// </summary>
    public void SetCoins(int amount)
    {
        coins = Mathf.Max(0, amount);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (coinText != null)
            coinText.text = coins.ToString();
    }

    /// <summary>
    /// Instantiates a floating-text above the coinText,
    /// animates it upward over floatDuration, then updates the coin total.
    /// </summary>
    private IEnumerator ShowThenApplyDelta(int delta)
    {
        if (floatingTextPrefab == null || coinText == null)
        {
            Debug.LogWarning("FloatingTextPrefab or coinText is not assigned!");
            yield break;
        }

        // 1) Instantiate the floating text as a child of the same Canvas
        Canvas parentCanvas = coinText.GetComponentInParent<Canvas>();
        if (parentCanvas == null)
        {
            Debug.LogWarning("coinText has no Canvas parent! Cannot show floating text.");
            yield break;
        }

        GameObject floatGO = Instantiate(
            floatingTextPrefab,
            parentCanvas.transform);

        // 2) Position it so it sits directly above coinText
        RectTransform floatRect = floatGO.GetComponent<RectTransform>();
        RectTransform coinRect = coinText.GetComponent<RectTransform>();

        // Convert coinText’s world position into the Canvas’s local coordinates
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(
            parentCanvas.worldCamera, coinText.transform.position);

        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            screenPoint,
            parentCanvas.worldCamera,
            out localPos);

        // Optionally offset upward by a few pixels so it doesn’t overlap
        float yOffset = 10f;
        floatRect.anchoredPosition = new Vector2(localPos.x, localPos.y + yOffset);

        // 3) Set the text: “+N” or “?N”
        TextMeshProUGUI tmp = floatGO.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.text = (delta > 0 ? "+" : "") + delta.ToString();
        }

        // 4) Animate over time: move up by `floatDistance`, fade out (optional)
        CanvasGroup cg = floatGO.GetComponent<CanvasGroup>();
        if (cg == null) cg = floatGO.AddComponent<CanvasGroup>();

        Vector2 startPos = floatRect.anchoredPosition;
        Vector2 endPos = startPos + Vector2.up * floatDistance;
        float elapsed = 0f;

        while (elapsed < floatDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / floatDuration);

            // Lerp position
            floatRect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

            // Fade alpha from 1 ? 0
            cg.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        // 5) Destroy the floating text object
        Destroy(floatGO);

        // 6) Only now, modify `coins` and update the main UI
        coins = Mathf.Max(0, coins + delta);
        UpdateUI();
    }
}
