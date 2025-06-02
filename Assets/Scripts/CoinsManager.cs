using UnityEngine;
using TMPro;
using System.Collections;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    [Header("UI Reference")]
    public TextMeshProUGUI coinText;

    public GameObject floatingTextPrefab;
    public float floatDistance = 50f;
    public float floatDuration = 1f;

    [Header("Settings")]
    [Tooltip("How many coins the player starts with when clicking 'Play'")]
    public int defaultCoins = 0;

    private int coins;
    public int Coins => coins;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (coinText == null)
        {
            TryReconnectUI();
        }

        if (PlayerPrefs.HasKey("Coins"))
        {
            coins = PlayerPrefs.GetInt("Coins");
        }
        else
        {
            coins = defaultCoins;
        }

        UpdateUI();
    }


    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("Coins", coins);
    }

    public void TryReconnectUI()
    {
        GameObject obj = GameObject.FindWithTag("CoinText");
        if (obj != null)
        {
            coinText = obj.GetComponent<TextMeshProUGUI>();
            UpdateUI();
        }
    }

    public void ResetCoins()
    {
        coins = defaultCoins;
        PlayerPrefs.SetInt("Coins", coins);
        UpdateUI();
    }

    public void SpendCoins(int amount)
    {
        StartCoroutine(CoinManager.Instance.ShowThenApplyDelta(amount));
        coins = Mathf.Max(0, coins - amount);
        PlayerPrefs.SetInt("Coins", coins);
        UpdateUI();
    }

    public void AddCoins(int amount)
    {
        StartCoroutine(CoinManager.Instance.ShowThenApplyDelta(amount));
        coins += amount;
        PlayerPrefs.SetInt("Coins", coins);
        UpdateUI();
    }

    public void SetCoins(int amount)
    {
        coins = Mathf.Max(0, amount);
        PlayerPrefs.SetInt("Coins", coins);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (coinText != null)
            coinText.text = "" + coins;
    }
    void OnEnable()
    {
        TryReconnectUI();
    }
    void Update()
    {
        if (coinText == null)
        {
            TryReconnectUI();
        }
    }

    public IEnumerator ShowThenApplyDelta(int delta)
    {
        if (floatingTextPrefab == null || coinText == null)
            yield break;

        Canvas parentCanvas = coinText.GetComponentInParent<Canvas>();
        if (parentCanvas == null)
            yield break;

        // Instantiate floating text under the same Canvas
        GameObject floatGO = Instantiate(floatingTextPrefab, parentCanvas.transform);
        RectTransform floatRect = floatGO.GetComponent<RectTransform>();
        RectTransform coinRect = coinText.GetComponent<RectTransform>();

        // Compute screen position of the coinText, then convert to local Canvas space
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(parentCanvas.worldCamera, coinText.transform.position);
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            screenPoint,
            parentCanvas.worldCamera,
            out localPos
        );

        // Offset slightly above the coinText
        floatRect.anchoredPosition = new Vector2(localPos.x, localPos.y + 10f);

        // Set the text to show “+delta” or “-delta”
        TextMeshProUGUI tmp = floatGO.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
            tmp.text = (delta > 0 ? "" : "-") + delta.ToString();

        // Ensure there is a CanvasGroup to fade out
        CanvasGroup cg = floatGO.GetComponent<CanvasGroup>();
        if (cg == null) cg = floatGO.AddComponent<CanvasGroup>();

        Vector2 startPos = floatRect.anchoredPosition;
        Vector2 endPos = startPos + Vector2.up * floatDistance;
        float elapsed = 0f;

        // Animate upward + fade-out over floatDuration seconds
        while (elapsed < floatDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / floatDuration);
            floatRect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            cg.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }

        Destroy(floatGO);

        // Finally, apply the delta to coins and update UI
        coins = Mathf.Max(0, coins + delta);
        PlayerPrefs.SetInt("Coins", coins);
        UpdateUI();
    }

}
