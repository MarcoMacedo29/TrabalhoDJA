using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI coinText;

    [Header("Floating Text")]
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private float floatDistance = 50f;
    [SerializeField] private float floatDuration = 1f;

    [Header("Coins")]
    public int defaultCoins = 100;
    private int coins;

    public int Coins => coins;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            coins = PlayerPrefs.GetInt("Coins", defaultCoins);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(ReconnectUI());
    }

    private IEnumerator ReconnectUI()
    {
        float timeout = 0.5f;
        float elapsed = 0f;
        float retryDelay = 0.05f;

        while (elapsed < timeout)
        {
            if (coinText == null)
            {
                GameObject found = GameObject.FindWithTag("CoinText");
                if (found != null)
                {
                    coinText = found.GetComponent<TextMeshProUGUI>();
                    Debug.Log("? CoinText encontrado após " + elapsed + "s");
                    UpdateUI();
                    yield break;
                }
            }

            yield return new WaitForSeconds(retryDelay);
            elapsed += retryDelay;
        }

        Debug.LogWarning("?? CoinText NÃO encontrado após " + timeout + " segundos.");
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0) return;

        coins += amount;
        PlayerPrefs.SetInt("Coins", coins);
        UpdateUI();

        if (floatingTextPrefab != null && coinText != null)
            StartCoroutine(ShowFloatingText("+" + amount));
    }

    public void SpendCoins(int amount)
    {
        if (amount <= 0 || coins < amount) return;

        coins -= amount;
        PlayerPrefs.SetInt("Coins", coins);
        UpdateUI();

        if (floatingTextPrefab != null && coinText != null)
            StartCoroutine(ShowFloatingText("-" + amount));
    }

    public void ResetCoins()
    {
        coins = defaultCoins;
        PlayerPrefs.SetInt("Coins", coins);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (coinText != null)
        {
            coinText.text = coins.ToString();
        }
    }

    private IEnumerator ShowFloatingText(string text)
    {
        Canvas canvas = coinText.GetComponentInParent<Canvas>();
        if (canvas == null) yield break;

        GameObject instance = Instantiate(floatingTextPrefab, canvas.transform);
        RectTransform rt = instance.GetComponent<RectTransform>();

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, coinText.transform.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPoint, canvas.worldCamera, out Vector2 localPos);

        rt.anchoredPosition = localPos + Vector2.up * 10f;

        TextMeshProUGUI tmp = instance.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null) tmp.text = text;

        CanvasGroup cg = instance.GetComponent<CanvasGroup>() ?? instance.AddComponent<CanvasGroup>();

        Vector2 start = rt.anchoredPosition;
        Vector2 end = start + Vector2.up * floatDistance;
        float time = 0f;

        while (time < floatDuration)
        {
            time += Time.deltaTime;
            float t = time / floatDuration;
            rt.anchoredPosition = Vector2.Lerp(start, end, t);
            cg.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }

        Destroy(instance);
    }
}