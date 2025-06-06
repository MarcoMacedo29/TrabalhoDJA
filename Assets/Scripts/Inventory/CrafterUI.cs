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

    [Header("UI References for Selected Slots (in your crafting panel)")]
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
    public Vector2 bladeMergeOffset = new Vector2(0f, +80f);
    [Tooltip("Relative to guardSlot, where should the hilt sprite be placed?")]
    public Vector2 hiltMergeOffset = new Vector2(0f, -80f);

    [Header("Outside Container for Shrinking")]
    [Tooltip("An empty RectTransform under your Canvas; we'll reparent the three slots here during the shrink.")]
    public RectTransform outsideContainer;

    public GameObject swordInventorySlot;

    [Header("Scene Sword Renderer")]
    public SceneSwordDisplay sceneSwordDisplay;

    [Header("Scene Sword")]
    public SwordAssembler sceneSwordAssembler;

    [Header("Crafting Button")]
    public Button hammerButton;

    private bool isCrafting = false;

    // Durations (in seconds)
    private float mergeDuration = 0.6f;
    private float shrinkDuration = 0.7f;

    // We’ll store each slot’s original parent, so we can reparent them back:
    private Transform origHiltParent;
    private Transform origGuardParent;
    private Transform origBladeParent;

    private Canvas rootCanvas;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Find the root Canvas for coordinate conversions
        rootCanvas = GetComponentInParent<Canvas>();
        if (rootCanvas == null)
            Debug.LogError("CrafterUI: No Canvas found in parents. Make sure this script sits under a Canvas.");
    }

    private void Start()
    {
        // Hide all sliders at start
        hiltSlider.SetActive(false);
        guardSlider.SetActive(false);
        bladeSlider.SetActive(false);

        RefreshAllPartImages();
        RefreshCraftButton();
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
                    item != null && item.partName == partImg.swordPart.partName);

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

        // Animate the flying clone into the crafting slot, as before
        StartCoroutine(AnimatePartMove(clicked.GetComponent<Image>(), destSlot, clicked.swordPart.sprite));
        RefreshAllPartImages();
        RefreshCraftButton();
    }

    private IEnumerator AnimatePartMove(Image source, Image destination, Sprite finalSprite)
    {
        // Instantiate a flying copy under the same parent, then reparent it to the root Canvas
        Image fly = Instantiate(source, source.transform.parent);
        fly.transform.SetAsLastSibling();
        fly.raycastTarget = false;

        RectTransform flyRT = fly.rectTransform;
        RectTransform srcRT = source.rectTransform;
        RectTransform dstRT = destination.rectTransform;

        // Convert source world ? canvas local point
        Vector2 srcPos, dstPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rootCanvas.transform as RectTransform,
            RectTransformUtility.WorldToScreenPoint(null, srcRT.position),
            rootCanvas.worldCamera,
            out srcPos);
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
            float p = Mathf.SmoothStep(0f, 1f, t / mergeDuration);
            flyRT.anchoredPosition = Vector2.Lerp(srcPos, dstPos, p);
            flyRT.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.8f, p);
            yield return null;
        }

        // Once it lands, set the real slot’s sprite & destroy the flying copy
        destination.sprite = finalSprite;
        destination.color = Color.white;
        Destroy(fly.gameObject);
    }

    /// <summary>
    /// Called by the merge button. Animates blade & hilt ? guard, then moves all three slot?Images into
    /// an outside container, shrinks that container, then re?parents back—keeping each slot’s own sprite.
    /// </summary>
    public void CraftAndMerge()
    {
        if (isCrafting)
            return;

        if (hammerButton != null)
            hammerButton.interactable = false;

        // 1) Ensure all three parts are chosen
        if (selectedHilt == null || selectedGuard == null || selectedBlade == null)
        {
            Debug.LogWarning("You must select a Hilt, Guard, and Blade before crafting!");
            isCrafting = false;
            RefreshCraftButton();
            return;
        }

        // 2) Immediately set the guard slot’s sprite
        if (selectedGuardSlot != null)
        {
            selectedGuardSlot.sprite = selectedGuard.sprite;
            selectedGuardSlot.color = Color.white;
        }

        // 3) Set blade & hilt slot sprites & make them visible
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

        // 4) Animate blade ? guard, and hilt ? guard
        RectTransform guardRT = selectedGuardSlot.rectTransform;
        Vector2 guardAnchored = guardRT.anchoredPosition;
        Vector2 bladeTarget = guardAnchored + bladeMergeOffset;
        Vector2 hiltTarget = guardAnchored + hiltMergeOffset;

        if (selectedBladeSlot != null)
            StartCoroutine(AnimateMergeMoveSlot(selectedBladeSlot, bladeTarget));

        if (selectedHiltSlot != null)
            StartCoroutine(AnimateMergeMoveSlot(selectedHiltSlot, hiltTarget));

        // 5) After mergeDuration, begin the “reparent?to?outsideContainer & shrink” sequence
        StartCoroutine(ReparentAndShrinkRoutine());
    }

    /// <summary>
    /// Moves a slot?Image’s RectTransform from its current anchoredPosition ? targetAnchored over mergeDuration.
    /// </summary>
    private IEnumerator AnimateMergeMoveSlot(Image partImage, Vector2 targetAnchoredPos)
    {
        RectTransform rt = partImage.rectTransform;
        Vector2 startAnchored = rt.anchoredPosition;
        float t = 0f;

        while (t < mergeDuration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.SmoothStep(0f, 1f, t / mergeDuration);
            rt.anchoredPosition = Vector2.Lerp(startAnchored, targetAnchoredPos, p);
            yield return null;
        }

        rt.anchoredPosition = targetAnchoredPos;
    }

    /// <summary>
    /// 1) Waits for mergeDuration real?seconds  
    /// 2) Stores each slot’s original parent  
    /// 3) Reparents all three into outsideContainer (preserving their on-screen positions)  
    /// 4) Shrinks outsideContainer ? zero over shrinkDuration real-seconds  
    /// 5) Reparents each slot back to its original parent, sets anchoredPosition=(0,0), scale=1, keeps its own sprite  
    /// 6) Removes parts from inventory, clears selection, refreshes UI  
    /// </summary>
    private IEnumerator ReparentAndShrinkRoutine()
    {
        // 1) Wait for the blade/hilt?onto?guard animations to finish
        yield return new WaitForSecondsRealtime(mergeDuration);

        // 2) Capture original parents
        origHiltParent = selectedHiltSlot.transform.parent;
        origGuardParent = selectedGuardSlot.transform.parent;
        origBladeParent = selectedBladeSlot.transform.parent;

        // 3) Reparent each slot?Image into outsideContainer,
        //    preserving their on-screen (canvas) positions
        ReparentToContainerPreservingPosition(selectedHiltSlot, outsideContainer);
        ReparentToContainerPreservingPosition(selectedGuardSlot, outsideContainer);
        ReparentToContainerPreservingPosition(selectedBladeSlot, outsideContainer);

        // 4) Shrink outsideContainer over shrinkDuration (real time)
        Vector3 startScale = outsideContainer.localScale;
        Vector3 targetScale = Vector3.zero;
        float t = 0f;

        while (t < shrinkDuration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.SmoothStep(0f, 1f, t / shrinkDuration);
            outsideContainer.localScale = Vector3.Lerp(startScale, targetScale, p);
            yield return null;
        }
        outsideContainer.localScale = targetScale;

        var craftedHilt = selectedHilt;
        var craftedGuard = selectedGuard;
        var craftedBlade = selectedBlade;

        // 5) Reparent each slot back to its original parent,
        //    set anchoredPosition = (0,0), localScale = (1,1,1), and keep each slot’s own sprite
        RestoreAndRepositionPreservingSprite(
            selectedHiltSlot,
            origHiltParent,
            selectedHiltSlot.sprite
        );
        RestoreAndRepositionPreservingSprite(
            selectedGuardSlot,
            origGuardParent,
            selectedGuardSlot.sprite
        );
        RestoreAndRepositionPreservingSprite(
            selectedBladeSlot,
            origBladeParent,
            selectedBladeSlot.sprite
        );

        // Reset outsideContainer’s scale so it’s ready for next time
        outsideContainer.localScale = Vector3.one;

        UpdateSwordInventorySlotDisplay(craftedHilt, craftedGuard, craftedBlade);

        if (sceneSwordDisplay != null)
        {
            sceneSwordDisplay.SetSwordParts(craftedHilt, craftedGuard, craftedBlade);
        }

        if (sceneSwordAssembler != null)
        {
            sceneSwordAssembler.SetCraftedParts(craftedHilt, craftedGuard, craftedBlade);
        }

        // 6) Consume the parts from inventory, clear selection, refresh UI
        InventoryManager.Instance.inventory.Remove(selectedHilt);
        InventoryManager.Instance.inventory.Remove(selectedGuard);
        InventoryManager.Instance.inventory.Remove(selectedBlade);

        InventoryUI.Instance.RefreshUI();

        selectedHilt = null;
        selectedGuard = null;
        selectedBlade = null;

        RefreshAllPartImages();
        isCrafting = false;
        RefreshCraftButton();
    }

    /// <summary>
    /// Reparents slotImg under originalParent. Then:
    ///   - rectTransform.anchoredPosition = (0,0)
    ///   - rectTransform.localScale = (1,1,1)
    ///   - slotImg.sprite is left unchanged (preserving each slot’s own part sprite)
    /// </summary>
    private void RestoreAndRepositionPreservingSprite(Image slotImg, Transform originalParent, Sprite originalSprite)
    {
        RectTransform rt = slotImg.rectTransform;
        rt.SetParent(originalParent, false);
        rt.anchoredPosition = Vector2.zero;
        rt.localScale = Vector3.one;

        slotImg.sprite = originalSprite;
        slotImg.color = Color.white;
    }

    /// <summary>
    /// Reparents slotImg under newParent (outsideContainer) while preserving its on-screen position.
    /// </summary>
    private void ReparentToContainerPreservingPosition(Image slotImg, RectTransform newParent)
    {
        RectTransform rt = slotImg.rectTransform;

        // 1) Get the slot’s screen point
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, rt.position);

        // 2) Convert that screen point to newParent’s local coordinates
        Vector2 newAnchored;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            newParent,
            screenPoint,
            rootCanvas.worldCamera,
            out newAnchored);

        // 3) Reparent (worldPositionStays = false), then set the anchoredPosition to match
        rt.SetParent(newParent, false);
        rt.anchoredPosition = newAnchored;
    }

    private void UpdateSwordInventorySlotDisplay(SwordPart hiltPart, SwordPart guardPart, SwordPart bladePart)
    {
        if (swordInventorySlot == null) return;

        Transform hilt = swordInventorySlot.transform.Find("ItemButton/Sword/Hilt");
        Transform guard = swordInventorySlot.transform.Find("ItemButton/Sword/Guard");
        Transform blade = swordInventorySlot.transform.Find("ItemButton/Sword/Blade");

        if (hilt != null && hiltPart != null)
            hilt.GetComponent<Image>().sprite = hiltPart.sprite;

        if (guard != null && guardPart != null)
            guard.GetComponent<Image>().sprite = guardPart.sprite;

        if (blade != null && bladePart != null)
            blade.GetComponent<Image>().sprite = bladePart.sprite;

        float defaultAlpha = 1f;
        foreach (var img in new[] { hilt, guard, blade })
        {
            if (img != null)
            {
                var image = img.GetComponent<Image>();
                var c = image.color;
                image.color = new Color(c.r, c.g, c.b, defaultAlpha);
            }
        }
    }

    private void RefreshCraftButton()
    {
        bool allSelected = (selectedHilt != null && selectedGuard != null && selectedBlade != null);
        bool canClick = allSelected && !isCrafting;

        if (hammerButton != null)
            hammerButton.interactable = canClick;

    }

}
