using UnityEngine;

public enum SlotRewardType {None,Gem,Heal,Health,Coin }

[CreateAssetMenu(fileName = "New Slot Symbol", menuName = "SlotMachine/Symbol")]
public class SlotSymbol : ScriptableObject
{
    public Sprite sprite;
    public string symbolName;
    public SlotRewardType rewardType;
    public int rewardAmount;
}
