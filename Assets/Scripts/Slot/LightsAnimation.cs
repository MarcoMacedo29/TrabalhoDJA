using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightsAnimation : MonoBehaviour
{
    [Header("Idle Animation (UI Image)")]
    public Image lightsImage;
    public List<Sprite> defaultSprites;         // default, no?match loop

    [Header("Match Animation (2× or 3× non-jackpot)")]
    public List<Sprite> matchSprites;

    [Header("Jackpot Animation (3× jackpot)")]
    public List<Sprite> jackpotSprites;

    public float lightsFrameRate = 6f;       // Frames per second

    private Coroutine animateCoroutine;
    private List<Sprite> currentSprites;
    private int frameIndex = 0;

    private void OnEnable()
    {
        // start with idle
        SwitchTo(defaultSprites);
    }

    private IEnumerator AnimateLoop()
    {
        float delay = 1f / lightsFrameRate;
        while (true)
        {
            lightsImage.sprite = currentSprites[frameIndex];
            frameIndex = (frameIndex + 1) % currentSprites.Count;
            yield return new WaitForSeconds(delay);
        }
    }

    private void SwitchTo(List<Sprite> sprites)
    {
        if (animateCoroutine != null)
            StopCoroutine(animateCoroutine);

        currentSprites = sprites ?? defaultSprites;
        frameIndex = 0;
        animateCoroutine = StartCoroutine(AnimateLoop());
    }

    public void ShowIdle()
    {
        SwitchTo(defaultSprites);
    }

    public void ShowMatch()
    {
        SwitchTo(matchSprites);
    }

    public void ShowJackpot()
    {
        SwitchTo(jackpotSprites);
    }
}
