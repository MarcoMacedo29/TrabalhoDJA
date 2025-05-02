using UnityEngine;
using System.Collections;

public class RouletteSpin : MonoBehaviour
{
    private bool isSpinning = false;

    [Header("Spin Settings")]
    public float spinDuration = 4f;
    public int numberOfSlots = 16;

    [Header("Slot Items")]
    public SwordPart[] items; // Assign these in the Inspector
    public SwordPart selectedItem;

    public void StartSpin()
    {
        if (!isSpinning)
        {
            float fullSpins = Random.Range(2f, 4f);
            float extraAngle = Random.Range(0f, 360f);
            float targetAngle = (fullSpins * 360f) + extraAngle;

            StartCoroutine(SpinRoulette(targetAngle));
        }
    }

    private IEnumerator SpinRoulette(float totalAngle)
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

            transform.Rotate(0f, 0f, deltaAngle);
            currentAngle = newAngle;

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        isSpinning = false;

        // Final rotation value
        float finalZ = transform.eulerAngles.z;
        float adjustedAngle = (360f - finalZ % 360f + 180f) % 360f;

        float segmentSize = 360f / numberOfSlots;
        int landedSlot = Mathf.FloorToInt(adjustedAngle / segmentSize) % numberOfSlots;

        // Centering adjustment
        float slotCenterAngle = landedSlot * segmentSize + (segmentSize / 2f);
        float correctionAngle = adjustedAngle - slotCenterAngle;

        // Apply the correction
        transform.Rotate(0f, 0f, correctionAngle);

        if (items != null && items.Length == numberOfSlots)
        {
            selectedItem = items[landedSlot];
            // selectedItem is now perfectly aligned under the picker
        }
    }
}
