using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PartSlotImage : MonoBehaviour, IPointerClickHandler
{
    public SwordPart swordPart; // assign this in the inspector for each slot image
    private Image image;
    private bool isOwned = false;

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
        isOwned = owned;
        if (image == null) return;

        var color = image.color;
        color.a = owned ? 1f : 0.3f;
        image.color = color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isOwned) return;              

        image.color = new Color(1, 1, 1, 1);

        CrafterUI.Instance.SelectPart(this);
    }
}
