using UnityEngine;

public enum ElementType { NoGem, Fire, Ice, Emerald, Poison, Wind, Prism }
    
[CreateAssetMenu(fileName = "NewGemEffect", menuName = "Sword/Gem Effect")]
public class GemEffect : ScriptableObject
{
    [Header("General Info")]
    public string gemName;
    public ElementType elementType;
    public Color gemColor = Color.white; // Set in inspector or via hex

    [Header("Base Modifiers")]
    public int bonusDamage = 0;
    [Range(0f, 1f)] public float procChance = 0f;

    [Header("Effect Parameters")]
    public float effectDuration = 0f;     // Burn, poison, slow duration
    public float damageOverTime = 0f;     // DOT amount
    public float slowAmount = 0f;         // For Ice (0.5 = 50% slow)
    public float lifestealPercent = 0f;   // For Emerald (0.1 = 10%)
    public float pullRadius = 0f;         // For Wind (0 = disable)
    public float cooldown = 0f;           // Optional (e.g., for Wind pull)

    public static GemEffect NoGem()
    {
        // Return a default "NoGem" object (a gem with no effects)
        GemEffect noGem = ScriptableObject.CreateInstance<GemEffect>();
        noGem.gemName = "NoGem";
        noGem.elementType = ElementType.NoGem;
        noGem.bonusDamage = 0;
        noGem.procChance = 0;
        noGem.effectDuration = 0;
        noGem.damageOverTime = 0;
        noGem.slowAmount = 0;
        noGem.lifestealPercent = 0;
        noGem.pullRadius = 0;
        noGem.cooldown = 0;
        noGem.gemColor = Color.gray; // Gray to indicate no gem
        return noGem;
    }
}
