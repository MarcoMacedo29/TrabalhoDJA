using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DiceRollVisual : MonoBehaviour
{
    [Header("Dice Slots")]
    public Image dice1Image;
    public Image dice2Image;
    public Image resultImage;

    [Header("References")]
    public List<DiceVisual> diceSprites;   // values 1–6
    public List<DiceVisual> resultSprites; // values 2–12

    [Header("Jingle Settings")]
    public float jingleDuration = 0.5f;
    public float jingleAmplitude = 10f;
    public float jingleFrequency = 4f;

    [Header("Spin Settings")]
    public int minSpinIterations = 10;
    public int maxSpinIterations = 15;
    public float baseInterval = 0.05f;
    public float intervalJitter = 0.02f;

    private bool isSpinning = false;
    private Vector2 dice1OrigPos;
    private Vector2 dice2OrigPos;
    private Vector2 resultOrigPos;

    private float lastBetAmount;

    private void Awake()
    {
        // Cache original positions
        dice1OrigPos = dice1Image.rectTransform.anchoredPosition;
        dice2OrigPos = dice2Image.rectTransform.anchoredPosition;
        resultOrigPos = resultImage.rectTransform.anchoredPosition;
    }

    public void RollDice(float betAmount)
    {
        if (isSpinning) return;
        lastBetAmount = betAmount;
        isSpinning = true;
        StartCoroutine(JingleAndRoll());
    }

    private IEnumerator JingleAndRoll()
    {
        // Jingle up and down
        float elapsed = 0f;
        while (elapsed < jingleDuration)
        {
            elapsed += Time.deltaTime;
            float offset = Mathf.Sin(elapsed * jingleFrequency * Mathf.PI * 2) * jingleAmplitude;
            Vector2 delta = Vector2.up * offset;

            dice1Image.rectTransform.anchoredPosition = dice1OrigPos + delta;
            dice2Image.rectTransform.anchoredPosition = dice2OrigPos + delta;

            yield return null;
        }

        // Reset positions
        dice1Image.rectTransform.anchoredPosition = dice1OrigPos;
        dice2Image.rectTransform.anchoredPosition = dice2OrigPos;

        // Begin actual spin
        yield return StartCoroutine(SpinWithRandomness());
    }

    private IEnumerator SpinWithRandomness()
    {
        // Determine final roll values up front
        int finalD1 = Random.Range(1, 7);
        int finalD2 = Random.Range(1, 7);
        int finalTotal = finalD1 + finalD2;

        // Randomized spin count
        int spins = Random.Range(minSpinIterations, maxSpinIterations + 1);

        for (int i = 0; i < spins; i++)
        {
            int temp1 = Random.Range(1, 7);
            int temp2 = Random.Range(1, 7);
            int tempTotal = temp1 + temp2;

            // Phase-offset updates for realism
            dice1Image.sprite = diceSprites.Find(d => d.value == temp1)?.sprite;
            yield return new WaitForSeconds(GetJitteredInterval());

            dice2Image.sprite = diceSprites.Find(d => d.value == temp2)?.sprite;
            yield return new WaitForSeconds(GetJitteredInterval());

            resultImage.sprite = resultSprites.Find(r => r.value == tempTotal)?.sprite;
            yield return new WaitForSeconds(GetJitteredInterval());
        }

        // Final roll assignment
        dice1Image.sprite = diceSprites.Find(d => d.value == finalD1)?.sprite;
        yield return new WaitForSeconds(GetJitteredInterval());

        dice2Image.sprite = diceSprites.Find(d => d.value == finalD2)?.sprite;
        yield return new WaitForSeconds(GetJitteredInterval());

        resultImage.sprite = resultSprites.Find(r => r.value == finalTotal)?.sprite;

        Debug.Log($"Dice Roll: {finalD1} + {finalD2} = {finalTotal}");


        ApplyPayout(finalTotal);
        isSpinning = false;
    }

    private float GetJitteredInterval()
    {
        return baseInterval + Random.Range(-intervalJitter, intervalJitter);
    }
    private void ApplyPayout(int total)
    {
        float multiplier;

        if (total <= 3)        // 2–3
            multiplier = 0f;
        else if (total <= 5)   // 4–5
            multiplier = 0.5f;
        else if (total <= 8)   // 6–8
            multiplier = 1f;
        else if (total <= 10)  // 9–10
            multiplier = 1.5f;
        else                    // 11–12
            multiplier = 2f;

        float payout = lastBetAmount * multiplier;
        float profit = payout - lastBetAmount;

        Debug.Log($"Dice Bet Result ? Bet: {lastBetAmount}, Total: {total}, Profit: {profit}");

        // Since we already deducted lastBetAmount in BetInputUI,
        // we now add back the 'payout' amount (which is 0 if the player loses).
        int payoutInt = Mathf.FloorToInt(payout);
        if (payoutInt > 0)
        {
            CoinManager.Instance.AddCoins(payoutInt);
        }

    }
}
