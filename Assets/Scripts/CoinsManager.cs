using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    [Header("UI Reference")]
    public TextMeshProUGUI coinText;

    [Header("Settings")]
    [Tooltip("How many coins the player always starts (and resets) with")]
    public int defaultCoins = 1000;

    private int coins;
    public int Coins => coins;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Always reset to default on load
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
        // Update UI at start
        UpdateUI();
    }

    /// <summary>
    /// Spend coins for each bet; immediately resets back to defaultCoins
    /// so the next time you bet you again have defaultCoins available.
    /// </summary>
    public void SpendCoins(int amount)
    {
        // Optionally check amount <= coins if you like
        coins = Mathf.Max(0, coins - amount);

        // Update UI to show the temporary deduction
        UpdateUI();

        // Then immediately reset for the next bet
        coins = defaultCoins;
        UpdateUI();
    }

    /// <summary>
    /// Add coins (e.g. winnings). Does not persist beyond this session.
    /// </summary>
    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateUI();
    }

    /// <summary>
    /// Force?set the coin amount (e.g. refunds or admin reset).
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
}
