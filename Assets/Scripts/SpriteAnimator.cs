using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    public Sprite[] frames;               // Array of 6 sprite frames
    public float frameRate = 10f;         // Frames per second

    private SpriteRenderer spriteRenderer;
    private int currentFrame;
    private float timer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (frames == null || frames.Length == 0)
        {
            Debug.LogWarning("No animation frames assigned!");
        }
    }

    private void Update()
    {
        if (frames == null || frames.Length == 0)
            return;

        timer += Time.deltaTime;

        if (timer >= 1f / frameRate)
        {
            timer -= 1f / frameRate;
            currentFrame = (currentFrame + 1) % frames.Length;
            spriteRenderer.sprite = frames[currentFrame];
        }
    }
}
