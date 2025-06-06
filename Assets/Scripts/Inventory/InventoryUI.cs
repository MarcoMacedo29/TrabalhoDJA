using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Image[] slotImages;
    public Sprite emptySlotSprite;

    [Range(0f, 1f)]
    public float emptySlotAlpha = 184f / 255f;

    public Vector2 emptySlotSize = new Vector2(64f, 64f);

    public static InventoryUI Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    void Start()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        var inventory = InventoryManager.Instance.inventory;

        for (int i = 0; i < slotImages.Length; i++)
        {
            RectTransform rt = slotImages[i].GetComponent<RectTransform>();

            if (i < inventory.Count)
            {
                slotImages[i].sprite = inventory[i].sprite;
                slotImages[i].color = Color.white;
                rt.sizeDelta = new Vector2(44f, 44f); // Set to 44x44 for real items
            }
            else
            {
                slotImages[i].sprite = emptySlotSprite;

                // Keep the alpha from the placeholder setup

                slotImages[i].color = new Color(1f, 1f, 1f, emptySlotAlpha);
                rt.sizeDelta = emptySlotSize; // now 64�64
            }
        }
    }

}