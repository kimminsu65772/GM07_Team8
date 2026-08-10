using UnityEngine;

[CreateAssetMenu(fileName = "EquipData", menuName = "GameData/Equipment Data")]
public class Equipment : ScriptableObject
{
    [Header("장비 스탯")]
    [SerializeField] private EquipGradeEnum equipGrade;
    [SerializeField] private EquipPartEnum equipPart;
    [SerializeField] private int equipLv;
    [SerializeField] private float bonusHP;
    [SerializeField] private float bonusAtk;
    [SerializeField] private float bonusDef;
    [SerializeField] private float bonusCriChance;

    public EquipGradeEnum EquipGrade => equipGrade;
    public EquipPartEnum EquipPart => equipPart;
    public int EquipLv => equipLv;
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
}
