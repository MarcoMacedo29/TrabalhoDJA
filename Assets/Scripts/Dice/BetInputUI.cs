using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BetInputUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField inputField;     // Assign your TMP_InputField here
    public Button confirmButton;
    public Button clearButton;

    [Header("Settings")]
    public int maxDigits = 6;

    private int currentBet = 0;

    [Header("References")]
    public DiceRollVisual diceRoller;

    private void Start()
    {
        confirmButton.onClick.AddListener(OnConfirm);
        clearButton.onClick.AddListener(OnClear);
        RefreshDisplay();
    }

    private void Update()
    {
        HandleKeyboardInput();
    }

    private void HandleKeyboardInput()
    {
        foreach (char c in Input.inputString)
        {
            if (char.IsDigit(c))
            {
                if (inputField.text.Length < maxDigits)
                {
                    int digit = c - '0';
                    int next = currentBet * 10 + digit;
                    if (next <= CoinManager.Instance.Coins)
                        currentBet = next;
                }
            }
            else if (c == '\b') // backspace
            {
                currentBet /= 10;
            }
            else if (c == '\n' || c == '\r') // enter
            {
                OnConfirm();
            }
        }

        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        currentBet = Mathf.Clamp(currentBet, 0, CoinManager.Instance.Coins);
        inputField.text = (currentBet > 0) ? currentBet.ToString() : "";
        confirmButton.interactable = currentBet > 0;
    }

    public void OnConfirm()
    {
        if (currentBet <= 0 || currentBet > CoinManager.Instance.Coins)
            return;

        // 1) Permanently remove the bet from the player’s coins
        CoinManager.Instance.SpendCoins(currentBet);
        Debug.Log($"Bet confirmed: {currentBet} coins");

        // 2) Immediately start the dice?spin with that bet amount
        if (diceRoller != null)
        {
            diceRoller.RollDice(currentBet);
        }
        else
        {
            Debug.LogError("DiceRollVisual reference is missing on BetInputUI!");
        }

        // 3) Clear the local bet
        currentBet = 0;
        RefreshDisplay();
    }

    public void OnClear()
    {
        currentBet = 0;
        inputField.text = ""; // ? This clears the input field
        Debug.Log("Input cleared");
        RefreshDisplay();
    }
}
