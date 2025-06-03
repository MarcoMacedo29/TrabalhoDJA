using UnityEngine;
using TMPro;
using System.Collections;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    [Header("UI Reference")]
    public TextMeshProUGUI coinText;

    [Header("Floating Text Prefab")]
    public GameObject floatingTextPrefab;

    [Header("Animation Settings")]
    public float floatDistance = 50f;
    public float floatDuration = 1f;

    [Header("Settings")]
    [Tooltip("Quantas moedas o jogador começa quando entra no jogo")]
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
            return;
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

    /// <summary>
    /// Subtrai imediatamente "amount" de coins, atualiza a UI e
    /// dispara a animação de "-amount" que flutua simultaneamente.
    /// </summary>
    public void SpendCoins(int amount)
    {
        if (amount <= 0 || coins <= 0) return;

        // 1) Retira imediatamente as moedas e atualiza UI
        coins = Mathf.Max(0, coins - amount);
        PlayerPrefs.SetInt("Coins", coins);
        UpdateUI();

        // 2) Dispara a animação de "-amount"
        StartCoroutine(ShowFloatingText(-amount));
    }

    /// <summary>
    /// Adiciona imediatamente "amount" em coins, atualiza a UI e
    /// dispara a animação de "+amount" que flutua simultaneamente.
    /// </summary>
    public void AddCoins(int amount)
    {
        if (amount <= 0) return;

        // 1) Adiciona imediatamente as moedas e atualiza UI
        coins += amount;
        PlayerPrefs.SetInt("Coins", coins);
        UpdateUI();

        // 2) Dispara a animação de "+amount"
        StartCoroutine(ShowFloatingText(amount));
    }

    /// <summary>
    /// Ajusta as moedas sem animação (caso você queira simplesmente setar um valor).
    /// </summary>
    public void SetCoins(int amount)
    {
        coins = Mathf.Max(0, amount);
        PlayerPrefs.SetInt("Coins", coins);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (coinText != null)
            coinText.text = coins.ToString();
    }

    /// <summary>
    /// Instancia o prefab de texto flutuante acima de coinText,
    /// faz ele subir e sumir em floatDuration segundos. Não altera "coins" aqui.
    /// </summary>
    private IEnumerator ShowFloatingText(int delta)
    {
        if (floatingTextPrefab == null || coinText == null)
            yield break;

        Canvas parentCanvas = coinText.GetComponentInParent<Canvas>();
        if (parentCanvas == null)
            yield break;

        // Instancia o prefab dentro do mesmo Canvas
        GameObject floatGO = Instantiate(floatingTextPrefab, parentCanvas.transform);
        RectTransform floatRT = floatGO.GetComponent<RectTransform>();

        // Converte a posição world de coinText para coordenadas locais do Canvas
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(
            parentCanvas.worldCamera,
            coinText.transform.position
        );

        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            screenPoint,
            parentCanvas.worldCamera,
            out localPos
        );

        // Posiciona o floatRT ligeiramente acima de coinText
        floatRT.anchoredPosition = new Vector2(localPos.x, localPos.y + 10f);

        // Ajusta o texto para "+N" ou "-N"
        TextMeshProUGUI tmp = floatGO.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
            tmp.text = (delta > 0 ? "+" : "") + delta.ToString();

        // Garante que haja um CanvasGroup para fazermos o fade
        CanvasGroup cg = floatGO.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = floatGO.AddComponent<CanvasGroup>();

        Vector2 startPos = floatRT.anchoredPosition;
        Vector2 endPos = startPos + Vector2.up * floatDistance;
        float elapsed = 0f;

        // Animação de subir + sumir em floatDuration
        while (elapsed < floatDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / floatDuration);

            floatRT.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            cg.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        Destroy(floatGO);
    }
}
