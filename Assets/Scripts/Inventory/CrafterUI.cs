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

    [Header("UI References")]
    public Image selectedHiltSlot;
    public Image selectedGuardSlot;
    public Image selectedBladeSlot;

    [Header("Currently Selected Parts (Data)")]
    public SwordPart selectedHilt;
    public SwordPart selectedGuard;
    public SwordPart selectedBlade;

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
                owned = inventory.Exists(item => item != null && item.partName == partImg.swordPart.partName);
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

        StartCoroutine(AnimatePartMove(clicked.GetComponent<Image>(), destSlot, clicked.swordPart.sprite));
    }

    private IEnumerator AnimatePartMove(Image source, Image destination, Sprite finalSprite)
    {
        // 1) Create a flying clone on the same canvas
        Image fly = Instantiate(source, source.transform.parent);
        fly.transform.SetAsLastSibling();           // render on top
        fly.raycastTarget = false;                  // ignore clicks during flight

        RectTransform flyRT = fly.rectTransform;
        RectTransform srcRT = source.rectTransform;
        RectTransform dstRT = destination.rectTransform;

        // 2) Convert both rects to local space of the root canvas
        Canvas rootCanvas = fly.canvas.rootCanvas;
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

        flyRT.SetParent(rootCanvas.transform);      // fly inside root canvas
        flyRT.anchoredPosition = srcPos;

        float t = 0f;
        float dur = 0.35f;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;            // UI usually on unscaled DT
            float p = Mathf.SmoothStep(0, 1, t / dur);
            flyRT.anchoredPosition = Vector2.Lerp(srcPos, dstPos, p);
            flyRT.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.8f, p);
            yield return null;
        }

        // 3) Snap to slot, assign sprite, and tidy up
        destination.sprite = finalSprite;
        destination.color = Color.white;
        Destroy(fly.gameObject);
    }
}
