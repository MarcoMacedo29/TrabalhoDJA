using UnityEngine;

public enum SwordPartType { Hilt, Guard, Blade }

[CreateAssetMenu(fileName = "NewSwordPart", menuName = "Sword/Sword Part")]
public class SwordPart : ScriptableObject
{
    public string partName;
    public SwordPartType partType;
    public int damage;
    public Sprite sprite;

    [Header("Hilt Only")]
    public float attackSpeed;

    [Header("Guard Only")]
    public float blockStrength;

    [Header("Blade Only")]
    public float critChance;
}
