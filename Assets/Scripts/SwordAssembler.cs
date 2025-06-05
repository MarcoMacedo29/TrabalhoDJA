using UnityEngine;

public class SwordAssembler : MonoBehaviour
{
    public SwordPart hilt;
    public SwordPart guard;
    public SwordPart blade;
    public GemEffect gem;

    public int totalDamage;

    void Start()
    {
        totalDamage = CalculateBaseDamage();


        if (gem != null && gem.elementType != ElementType.NoGem)
        {
            totalDamage += gem.bonusDamage;
        }

    }

    private int CalculateBaseDamage()
    {
        int baseDamage = hilt.damage * guard.damage * blade.damage;
        return baseDamage;
    }

    public void Recalculate()
    {
        totalDamage = CalculateBaseDamage();


        if (gem != null && gem.elementType != ElementType.NoGem)

        {
            totalDamage += gem.bonusDamage;
        }

    }

    public void SetCraftedParts(SwordPart hilt, SwordPart guard, SwordPart blade)
    {

        this.hilt = hilt;
        this.guard = guard;
        this.blade = blade;

        Recalculate();
    }
}
