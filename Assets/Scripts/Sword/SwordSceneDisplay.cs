using UnityEngine;
public class SceneSwordDisplay : MonoBehaviour
{
    [Header("Assign the SpriteRenderer components")]
    public SpriteRenderer hiltRenderer;
    public SpriteRenderer guardRenderer;
    public SpriteRenderer bladeRenderer;

    public void SetSwordParts(SwordPart hilt, SwordPart guard, SwordPart blade)
    {
        SetSprite(hiltRenderer, hilt?.sprite);
        SetSprite(guardRenderer, guard?.sprite);
        SetSprite(bladeRenderer, blade?.sprite);
    }

    private void SetSprite(SpriteRenderer renderer, Sprite sprite)
    {
        if (renderer == null) return;

        renderer.sprite = sprite;
        renderer.color = Color.white;
    }
}
