// CrafterUI.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CrafterUI : MonoBehaviour
{
    public static CrafterUI Instance;

    [Header("Assign the arrays of images for each slider in Inspector")]
    public PartSlotImage[] hiltImages;
    public PartSlotImage[] guardImages;
    public PartSlotImage[] bladeImages;

    [Header("Sliders GameObjects")]
    public GameObject hiltSlider;
    public GameObject guardSlider;
    public GameObject bladeSlider;

    [Header("UI References for Selected Slots")]
    [Tooltip("Where the chosen Hilt ends up")]
    public Image selectedHiltSlot;
    [Tooltip("Where the chosen Guard ends up")]
    public Image selectedGuardSlot;
    [Tooltip("Where the chosen Blade ends up")]
    public Image selectedBladeSlot;

    [Header("Currently Selected Parts (Data)")]
    public SwordPart selectedHilt;
    public SwordPart selectedGuard;
    public SwordPart selectedBlade;

    [Header("Merge Offsets (tweak these so blade/guard/hilt line up)")]
    [Tooltip("Relative to guardSlot, where should the blade sprite be placed?")]
    public Vector2 bladeMergeOffset = new Vector2(0, +80f);
    [Tooltip("Relative to guardSlot, where should the hilt sprite be placed?")]
    public Vector2 hiltMergeOffset = new Vector2(0, -80f);

    // Duration (in seconds) of the merge transition for blade & hilt
    private float mergeDuration = 0.35f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        hiltSlider.SetActive(false);
        guardSlider.SetActive(false);
        bladeSlider.SetActive(false);

        RefreshAllPartImages();
    }

    public void ToggleSlider(GameObject targetSlider)
    {
        bool isActive = targetSlider.activeSelf;
        hiltSlider.SetActive(false);
        guardSlider.SetActive(false);
        bladeSlider.SetActive(false);
        targetSlider.SetActive(!isActive);
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
                bool inInventory = inventory.Exists(item =>
                    item != null &&
                    item.partName == partImg.swordPart.partName);

                bool alreadySelected = false;
                if (partImg.swordPart.partType == SwordPartType.Hilt)
                    alreadySelected = (partImg.swordPart == selectedHilt);
                else if (partImg.swordPart.partType == SwordPartType.Guard)
                    alreadySelected = (partImg.swordPart == selectedGuard);
                else if (partImg.swordPart.partType == SwordPartType.Blade)
                    alreadySelected = (partImg.swordPart == selectedBlade);

                // Show “owned” only if in inventory and not already placed in a craft slot
                owned = inInventory && !alreadySelected;
            }

            partImg.UpdateVisual(owned);
        }
    }

    public void SelectPart(PartSlotImage clicked)
    {
        if (clicked.swordPart == null) return;

        Image destSlot = null;
        switch (clicked.swordPart.partType)
        {
            case SwordPartType.Hilt:
                destSlot = selectedHiltSlot;
                selectedHilt = clicked.swordPart;
                break;
            case SwordPartType.Guard:
                destSlot = selectedGuardSlot;
                selectedGuard = clicked.swordPart;
                break;
            case SwordPartType.Blade:
                destSlot = selectedBladeSlot;
                selectedBlade = clicked.swordPart;
                break;
        }
        if (destSlot == null) return;

        // Animate the flying clone into the crafting slot
        StartCoroutine(AnimatePartMove(clicked.GetComponent<Image>(), destSlot, clicked.swordPart.sprite));

        // Immediately fade out that slot in the slider
        RefreshAllPartImages();
    }

    private IEnumerator AnimatePartMove(Image source, Image destination, Sprite finalSprite)
    {
        Image fly = Instantiate(source, source.transform.parent);
        fly.transform.SetAsLastSibling();
        fly.raycastTarget = false;

        RectTransform flyRT = fly.rectTransform;
        RectTransform srcRT = source.rectTransform;
        RectTransform dstRT = destination.rectTransform;

        Canvas rootCanvas = fly.canvas.rootCanvas;
        Vector2 srcPos, dstPos;

        // Convert source world?canvas?local
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rootCanvas.transform as RectTransform,
            RectTransformUtility.WorldToScreenPoint(null, srcRT.position),
            rootCanvas.worldCamera,
            out srcPos);

        // Convert destination world?canvas?local
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rootCanvas.transform as RectTransform,
            RectTransformUtility.WorldToScreenPoint(null, dstRT.position),
            rootCanvas.worldCamera,
            out dstPos);

        flyRT.SetParent(rootCanvas.transform, false);
        flyRT.anchoredPosition = srcPos;

        float t = 0f;
        while (t < mergeDuration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.SmoothStep(0, 1, t / mergeDuration);
            flyRT.anchoredPosition = Vector2.Lerp(srcPos, dstPos, p);
            flyRT.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.8f, p);
            yield return null;
        }

        destination.sprite = finalSprite;
        destination.color = Color.white;
        Destroy(fly.gameObject);
    }

    /// <summary>
    /// Called by the hammer Button. Plays a transition to move blade & hilt onto guard, then consumes parts.
    /// </summary>
    public void CraftAndMerge()
    {
        // 1) Ensure all three parts are chosen
        if (selectedHilt == null || selectedGuard == null || selectedBlade == null)
        {
            Debug.LogWarning("You must select a Hilt, Guard, and Blade before crafting!");
            return;
        }

        // 2) Assign the guard sprite immediately (no need to move it)
        if (selectedGuardSlot != null)
        {
            selectedGuardSlot.sprite = selectedGuard.sprite;
            selectedGuardSlot.color = Color.white;
        }

        // 3) Prepare blade & hilt at their slots, then animate them into merged positions
        //    (a) Set their sprites & make them visible
        if (selectedBladeSlot != null)
        {
            selectedBladeSlot.sprite = selectedBlade.sprite;
            selectedBladeSlot.color = Color.white;
        }
        if (selectedHiltSlot != null)
        {
            selectedHiltSlot.sprite = selectedHilt.sprite;
            selectedHiltSlot.color = Color.white;
        }

        // (b) Compute target anchoredPositions for blade and hilt relative to guard
        RectTransform guardRT = selectedGuardSlot.rectTransform;
        Vector2 guardAnchored = guardRT.anchoredPosition;

        Vector2 bladeTarget = guardAnchored + bladeMergeOffset;
        Vector2 hiltTarget = guardAnchored + hiltMergeOffset;

        // (c) Start coroutines to animate each part from its current slot to the merged target
        if (selectedBladeSlot != null)
            StartCoroutine(AnimateMergeMove(selectedBladeSlot, bladeTarget));

        if (selectedHiltSlot != null)
            StartCoroutine(AnimateMergeMove(selectedHiltSlot, hiltTarget));

        // 4) After mergeDuration, consume the parts from inventory and clear selection
        StartCoroutine(ConsumeAfterDelay(mergeDuration));
    }

    /// <summary>
    /// Animates a single part's Image from its current anchoredPosition to the target anchoredPosition.
    /// </summary>
    private IEnumerator AnimateMergeMove(Image partImage, Vector2 targetAnchoredPos)
    {
        RectTransform rt = partImage.rectTransform;
        Vector2 startAnchored = rt.anchoredPosition;
        float t = 0f;

        while (t < mergeDuration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.SmoothStep(0, 1, t / mergeDuration);
            rt.anchoredPosition = Vector2.Lerp(startAnchored, targetAnchoredPos, p);
            yield return null;
        }

        rt.anchoredPosition = targetAnchoredPos;
    }

    /// <summary>
    /// Waits for the merge animation to finish, then removes the parts from inventory and updates UI.
    /// </summary>
    private IEnumerator ConsumeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Remove chosen parts from inventory so they cannot be used again
        InventoryManager.Instance.inventory.Remove(selectedHilt);
        InventoryManager.Instance.inventory.Remove(selectedGuard);
        InventoryManager.Instance.inventory.Remove(selectedBlade);

        // Clear the selection references
        selectedHilt = null;
        selectedGuard = null;
        selectedBlade = null;

        // Refresh sliders so those parts no longer appear as owned
        RefreshAllPartImages();
    }
}
