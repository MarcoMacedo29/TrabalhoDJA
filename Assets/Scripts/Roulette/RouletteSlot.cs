using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class RouletteSlot
{
    public Image image;
    public SwordPart swordPart;
    [Range(0f, 360f)]
    public float centerAngle;
    public RewardData noReward;

    public bool IsEmptySlot()
    {
        return noReward != null;
    }
}