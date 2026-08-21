using UnityEngine;

[CreateAssetMenu(fileName = "EquipData", menuName = "Game Data/Equipment Data")]
public class Equipment : ScriptableObject
{
    [Header("장비 스탯")]
    [SerializeField] private EquipGradeEnum equipGrade;
    [SerializeField] private EquipPartEnum equipPart;
    [SerializeField] private float bonusHP;
    [SerializeField] private float bonusAtk;
    [SerializeField] private float bonusDef;
    [SerializeField] private float bonusCriChance;

    private int equipLv;
    public int EquipLv => equipLv;

    public EquipGradeEnum EquipGrade => equipGrade;
    public EquipPartEnum EquipPart => equipPart;
    public float BonusHP
    {
        get => bonusHP;
        set => bonusHP = Mathf.Max(0f, value);
    }

    public float BonusAtk
    {
        get => bonusAtk;
        set => bonusAtk = Mathf.Max(0f, value);
    }
    public float BonusDef
    {
        get => bonusDef;
        set => bonusDef = Mathf.Max(0f, value);
    }
    public float BonusCriChance
    {
        get => bonusCriChance;
        set => bonusCriChance = Mathf.Clamp(value, 0f, 100f);
    }

    public void SetData(EquipGradeEnum grade, EquipPartEnum part, float hp, float atk, float def, float criChance)
    {
        equipGrade = grade;
        equipPart = part;

        bonusHP = Mathf.Max(0f, hp);
        bonusAtk = Mathf.Max(0f, atk);
        bonusDef = Mathf.Max(0f, def);
        bonusCriChance = Mathf.Clamp(criChance, 0f, 100f);
    }
}
