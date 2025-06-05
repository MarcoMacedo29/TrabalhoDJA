using UnityEngine;
using UnityEngine.UI;

public class SwordDisplayUI : MonoBehaviour
{
    [Header("Assign these to the child Image objects")]
    public Image hiltImage;
    public Image guardImage;
    public Image bladeImage;

    /// <summary>
    /// Updates the displayed sword parts with the crafted ones.
    /// </summary>
    public void SetSwordParts(SwordPart hilt, SwordPart guard, SwordPart blade)
    {
        SetImage(hiltImage, hilt?.sprite);
        SetImage(guardImage, guard?.sprite);
        SetImage(bladeImage, blade?.sprite);
    }

    private void SetImage(Image image, Sprite sprite)
    {
        if (image == null) return;

        image.sprite = sprite;
        image.color = Color.white; // Alpha = 1
    }
}
