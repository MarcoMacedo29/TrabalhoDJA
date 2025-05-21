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
    [Tooltip("Duration of the jingle before rolling (seconds)")] public float jingleDuration = 0.5f;
    [Tooltip("Vertical amplitude of jingle in pixels")] public float jingleAmplitude = 10f;
    [Tooltip("Jingles per second")] public float jingleFrequency = 4f;

    [Header("Spin Settings")]
    [Tooltip("Minimum number of face changes before final result")] public int minSpinIterations = 10;
    [Tooltip("Maximum number of face changes before final result")] public int maxSpinIterations = 15;
    [Tooltip("Base interval between face changes (seconds)")] public float baseInterval = 0.05f;
    [Tooltip("Max random jitter added/subtracted from base interval")] public float intervalJitter = 0.02f;

    private bool isSpinning = false;
    private Vector2 dice1OrigPos;
    private Vector2 dice2OrigPos;
    private Vector2 resultOrigPos;

    private void Awake()
    {
        // Cache original positions
        dice1OrigPos = dice1Image.rectTransform.anchoredPosition;
        dice2OrigPos = dice2Image.rectTransform.anchoredPosition;
        resultOrigPos = resultImage.rectTransform.anchoredPosition;
    }

    public void RollDice()
    {
        if (isSpinning) return;
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
        isSpinning = false;
    }

    private float GetJitteredInterval()
    {
        return baseInterval + Random.Range(-intervalJitter, intervalJitter);
    }
}
