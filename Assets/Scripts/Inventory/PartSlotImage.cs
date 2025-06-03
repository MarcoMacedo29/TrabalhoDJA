using UnityEngine;
using UnityEngine.UI;

public class PartSlotImage : MonoBehaviour
{
    public SwordPart swordPart; // assign this in the inspector for each slot image
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        if (image == null)
        {
            Debug.LogError("PartSlotImage: No Image component found on " + gameObject.name);
        }
    }

    public void UpdateVisual(bool owned)
    {
        if (image == null) return;

        var color = image.color;
        color.a = owned ? 1f : 0.3f;
        image.color = color;
    }
}
