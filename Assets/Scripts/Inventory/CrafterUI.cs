using UnityEngine;

public class CrafterUI : MonoBehaviour
{
    [Header("Assign the arrays of images for each slider in Inspector")]
    public PartSlotImage[] hiltImages;
    public PartSlotImage[] guardImages;
    public PartSlotImage[] bladeImages;

    [Header("Sliders GameObjects")]
    public GameObject hiltSlider;
    public GameObject guardSlider;
    public GameObject bladeSlider;

    private void Start()
    {
        // Initially hide all sliders or set them as needed
        hiltSlider.SetActive(false);
        guardSlider.SetActive(false);
        bladeSlider.SetActive(false);
    }

    // Call this when user clicks a button to toggle slider
    public void ToggleSlider(GameObject targetSlider)
    {
        bool isActive = targetSlider.activeSelf;
        // Disable all sliders first
        hiltSlider.SetActive(false);
        guardSlider.SetActive(false);
        bladeSlider.SetActive(false);

        // Toggle selected slider
        targetSlider.SetActive(!isActive);

        // Refresh UI images for all sliders
        RefreshAllPartImages();
    }

    public void RefreshAllPartImages()
    {
        RefreshPartImages(hiltImages);
        RefreshPartImages(guardImages);
        RefreshPartImages(bladeImages);
    }

    public void RefreshPartImages(PartSlotImage[] partImages)
    {
        var inventory = InventoryManager.Instance.inventory;

        foreach (var partImg in partImages)
        {
            bool owned = false;

            if (partImg.swordPart != null && inventory != null)
            {
                owned = inventory.Exists(item => item != null && item.partName == partImg.swordPart.partName);
            }

            partImg.UpdateVisual(owned);
        }
    }
}
