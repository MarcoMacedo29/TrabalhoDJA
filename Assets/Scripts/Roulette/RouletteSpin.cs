using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RouletteSpin : MonoBehaviour
{
    private bool isSpinning = false;
    private bool waitingForDecision = false;

    private SoundEffects soundEffects;

    [Header("Spin Settings")]
    public float spinDuration = 4f;

    [SerializeField]
    public RouletteSlot[] slots;
    public SwordPart selectedItem;

    [Header("Reward Slot")]
    public Image rewardedItemSlot;

    public Button spinButton;

    [Header("References")]
    public Transform itemPicker;  // This rotates around the wheel
    public Transform wheelCenter; // Center point of the wheel (should be this.transform or assigned)

    void Start()
    {
        if (rewardedItemSlot != null)
        {
            Color c = rewardedItemSlot.color;
            c.a = 0f;
            rewardedItemSlot.color = c;
        }
    }
    public void StartSpin()
    {
        if (!isSpinning || waitingForDecision)
        {
            if (CoinManager.Instance.Coins < 20)
                return;

            CoinManager.Instance.SpendCoins(20);

            soundEffects?.Button1();
            soundEffects?.PlaySpinSound();

            float fullSpins = Random.Range(5f, 7f);
            float extraAngle = Random.Range(0f, 360f);
            float targetAngle = (fullSpins * 360f) + extraAngle;

            StartCoroutine(RotateItemPicker(targetAngle));
        }
    }

    private void Update()
    {
        if (spinButton != null)
        {
            spinButton.interactable =
                (CoinManager.Instance.Coins >= 20) &&
                !isSpinning &&
                !waitingForDecision;
        }
    }


    private IEnumerator RotateItemPicker(float totalAngle)
    {
        isSpinning = true;

        float timeElapsed = 0f;
        float currentAngle = 0f;

        while (timeElapsed < spinDuration)
        {
            float t = timeElapsed / spinDuration;
            float easedT = 1f - Mathf.Pow(1f - t, 3f); // Ease out cubic
            float newAngle = Mathf.Lerp(0f, totalAngle, easedT);
            float deltaAngle = newAngle - currentAngle;

            itemPicker.RotateAround(wheelCenter.position, Vector3.forward, deltaAngle);
            currentAngle = newAngle;

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        float pickerAngle = GetPickerAngle();
        int selectedIndex = GetClosestSlotIndex(pickerAngle);

        // Smoothly align picker to the selected slot
        float correction = Mathf.DeltaAngle(pickerAngle, slots[selectedIndex].centerAngle);
        yield return StartCoroutine(AlignPickerToSlot(correction, 0.3f));

        isSpinning = false;

        soundEffects?.StopSpinSound();

        selectedItem = slots[selectedIndex].swordPart;
        RewardData selectedNoReward = slots[selectedIndex].noReward; // Check for no-reward slot

        Debug.Log("Selected: " + (selectedItem != null ? selectedItem.name : "No Reward"));

        if (rewardedItemSlot != null)
        {
            if (selectedNoReward != null)
            {                        
                ClearRewardSlot();

                waitingForDecision = false;
                if (spinButton != null)
                    spinButton.interactable = true;
            }
            else if (selectedItem != null && selectedItem.sprite != null)
            {
                rewardedItemSlot.sprite = selectedItem.sprite;
                rewardedItemSlot.enabled = true;
                Color c = rewardedItemSlot.color;
                c.a = 1f; // fully visible
                rewardedItemSlot.color = c;

                waitingForDecision = true;
                if (spinButton != null)
                    spinButton.interactable = false;
            }
        }
        
    }
    public void OnPlayerDecided()
    {
        waitingForDecision = false;
        if (spinButton != null )
            spinButton.interactable = true;
    }
    public void OnAccept()
    {
        OnPlayerDecided();
    }

    public void OnReject()
    {
        ClearRewardSlot();
        OnPlayerDecided();

    }

    public void ClearRewardSlot()
    {
        if (rewardedItemSlot != null)
        {
            rewardedItemSlot.sprite = null;
            rewardedItemSlot.enabled = false;
        }
    }

    private float GetPickerAngle()
    {
        Vector3 direction = itemPicker.position - wheelCenter.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return (angle + 360f) % 360f;
    }

    private int GetClosestSlotIndex(float angle)
    {
        float minDiff = float.MaxValue;
        int closestIndex = 0;

        for (int i = 0; i < slots.Length; i++)
        {
            float diff = Mathf.Abs(Mathf.DeltaAngle(angle, slots[i].centerAngle));
            if (diff < minDiff)
            {
                minDiff = diff;
                closestIndex = i;
            }
        }

        return closestIndex;
    }

    private IEnumerator AlignPickerToSlot(float correctionAngle, float duration)
    {
        float timeElapsed = 0f;
        float currentAngle = 0f;

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            float easedT = t * t * (3f - 2f * t); // SmoothStep
            float newAngle = Mathf.Lerp(0f, correctionAngle, easedT);
            float delta = newAngle - currentAngle;

            itemPicker.RotateAround(wheelCenter.position, Vector3.forward, delta);
            currentAngle = newAngle;

            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    public void RecalculateCenterAngles()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].image != null)
            {
                Vector3 localPos = slots[i].image.rectTransform.localPosition;
                float angle = Mathf.Atan2(localPos.y, localPos.x) * Mathf.Rad2Deg;
                angle = (angle + 360f) % 360f;
                slots[i].centerAngle = angle;
            }
        }

        Debug.Log("Center angles recalculated.");
    }
}