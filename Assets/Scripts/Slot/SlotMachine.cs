using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class SlotMachine : MonoBehaviour
{
    public Image[] reels;
    public Button spinButton;

    public SlotSymbol[] slotSymbols;
    public SlotSymbol jackpotSymbol;
    private List<SlotSymbol> finalSymbols;

    public LightsAnimation lightsAnim;

    public Image gemSlotImage;
    public List<GemEffect> possibleGems;

    private bool isSpinning = false;

    private void Start()
    {
        spinButton.onClick.AddListener(SpinReels);
    }

    void SpinReels()
    {
        if (isSpinning) return;
        isSpinning = true;

        lightsAnim.ShowIdle();

        finalSymbols = new List<SlotSymbol>(new SlotSymbol[reels.Length]);
        StartCoroutine(SpinAllReels());
    }

    IEnumerator SpinAllReels()
    {
        float baseSpinTime = 1.5f;
        float delayBetweenStops = 0.4f;

        Coroutine[] spinCoroutines = new Coroutine[reels.Length];

        // Start spinning all reels
        for (int i = 0; i < reels.Length; i++)
        {
            spinCoroutines[i] = StartCoroutine(SpinReel(reels[i]));
        }

        // Stop reels one by one
        for (int i = 0; i < reels.Length; i++)
        {
            yield return new WaitForSeconds(baseSpinTime + i * delayBetweenStops);
            StopCoroutine(spinCoroutines[i]);

            // Set final symbol
            int pick = Random.Range(0, slotSymbols.Length);
            reels[i].sprite = slotSymbols[pick].sprite;
            finalSymbols[i] = slotSymbols[pick];


        }

        isSpinning = false;

        EvaluateRewards(finalSymbols);
    }

    IEnumerator SpinReel(Image reelImage)
    {
        while (true)
        {
            int idx = Random.Range(0, slotSymbols.Length);
            reelImage.sprite = slotSymbols[idx].sprite;
            yield return new WaitForSeconds(0.1f);
        }
    }


    private void EvaluateRewards(List<SlotSymbol> results)
    {
        // Group symbols into matches of 2 or more, but EXCLUDE any with rewardType None (e.g. cherries)
        var groups = results
            .GroupBy(s => s)
            .Where(g => g.Count() >= 2 && g.Key.rewardType != SlotRewardType.None)
            .ToList();

        // If no valid matches remain (including only cherry?matches), stay idle
        if (groups.Count == 0)
        {
            Debug.Log("No winning matches this spin (cherries don't pay). Lights stay idle.");
            return;
        }

        // Jackpot check: 3× of the special jackpot symbol
        if (groups.Any(g => g.Key == jackpotSymbol && g.Count() == 3))
        {
            Debug.Log("?? JACKPOT! You got 3× Jackpot symbols! You earn 1 GEM!");
            lightsAnim.ShowJackpot();

            // Add this line below to show a random gem
            ShowRandomGem();

            // GameManager.Instance.AddGems(1);
            return;
        }

            // Otherwise it's at least one 2×/3× non?jackpot, non?cherry match
            Debug.Log("Nice match! Playing match lights.");
        lightsAnim.ShowMatch();

        // Payout for each winning group
        foreach (var g in groups)
        {
            var symbol = g.Key;
            int count = g.Count();
            int payout = symbol.rewardAmount * count;

            switch (symbol.rewardType)
            {
                case SlotRewardType.Coin:
                    Debug.Log($"You won {payout} coins!");
                    // GameManager.Instance.AddCoins(payout);
                    break;
                case SlotRewardType.Heal:
                    Debug.Log($"You healed {payout} HP!");
                    // Player.Instance.Heal(payout);
                    break;
                case SlotRewardType.Health:
                    Debug.Log($"Max HP increased by {payout}!");
                    // Player.Instance.AddMaxHealth(payout);
                    break;
                case SlotRewardType.Gem:
                    Debug.Log($"You won {payout} gems!");
                    // GameManager.Instance.AddGems(payout);
                    break;
                default:
                    Debug.Log($"Matched {count}× {symbol.symbolName}, no handler defined.");
                    break;
            }
        }
    }
    public void TestJackpotSpin()
    {
        if (isSpinning) return;
        isSpinning = true;

        // Reset lights and prepare result list
        lightsAnim.ShowIdle();
        finalSymbols = new List<SlotSymbol>(new SlotSymbol[reels.Length]);

        // Kick off the special spin coroutine
        StartCoroutine(SpinAllReelsTest());
    }

    private IEnumerator SpinAllReelsTest()
    {
        float baseSpinTime = 1.5f;
        float delayBetweenStops = 0.4f;
        Coroutine[] spinCoroutines = new Coroutine[reels.Length];

        // 1) Start spinning each reel as usual
        for (int i = 0; i < reels.Length; i++)
            spinCoroutines[i] = StartCoroutine(SpinReel(reels[i]));

        // 2) Stop them one by one, but always pick jackpotSymbol
        for (int i = 0; i < reels.Length; i++)
        {
            yield return new WaitForSeconds(baseSpinTime + i * delayBetweenStops);
            StopCoroutine(spinCoroutines[i]);

            reels[i].sprite = jackpotSymbol.sprite;
            finalSymbols[i] = jackpotSymbol;
        }

        isSpinning = false;

        // Evaluate as a full jackpot
        EvaluateRewards(finalSymbols);
    }

    private void ShowRandomGem()
    {
        if (possibleGems == null || gemSlotImage == null || possibleGems.Count == 0)
            return;

        // Exclude Prism gem
        List<GemEffect> nonPrismGems = possibleGems
            .Where(g => g.elementType != ElementType.Prism)
            .ToList();

        if (nonPrismGems.Count == 0)
            return;

        GemEffect chosen = nonPrismGems[Random.Range(0, nonPrismGems.Count)];
        StartCoroutine(FadeInGem(chosen.sprite, chosen.uiSize));
    }

    private IEnumerator FadeInGem(Sprite gemSprite, Vector2 targetSize)
    {
        gemSlotImage.sprite = gemSprite;
        gemSlotImage.rectTransform.sizeDelta = targetSize;

        float duration = 0.8f;
        float timer = 0f;

        // Start fully transparent
        gemSlotImage.color = new Color(1, 1, 1, 0);

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.SmoothStep(0f, 1f, timer / duration);
            gemSlotImage.color = new Color(1, 1, 1, alpha);
            yield return null;
        }

        // Ensure fully visible at the end
        gemSlotImage.color = new Color(1, 1, 1, 1);
    }
}
