using UnityEngine;

[CreateAssetMenu(fileName = "EquipData", menuName = "Game Data/Equipment Data")]
public class EquipmentSO : ScriptableObject
{
    [Header("장비 정보")]
    [SerializeField] private string equipName;
    [SerializeField] private Sprite equipIcon;
    [Header("장비 스탯")]
    [SerializeField] private EquipGradeEnum equipGrade;
    [SerializeField] private EquipPartEnum equipPart;
    [SerializeField] private float bonusHP;
    [SerializeField] private float bonusAtk;
    [SerializeField] private float bonusDef;
    [SerializeField] private float bonusCriChance;

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
}

public class Equipment
{
    public string EquipID { get; private set; }
    public int EquipLv { get; private set; }
    public EquipGradeEnum EquipGrade { get; private set; }
    public EquipPartEnum EquipPart { get; private set; }
    public float BonusHP { get; private set; }
    public float BonusAtk { get; private set; }
    public float BonusDef { get; private set; }
    public float BonusCriChance { get; private set; }

    public void EquipInit(EquipmentSO equipSO, string id)
    {
        EquipID = id;
        EquipGrade = equipSO.EquipGrade;
        EquipPart = equipSO.EquipPart;
        BonusHP = equipSO.BonusHP;
        BonusAtk = equipSO.BonusAtk;
        BonusDef = equipSO.BonusDef;
        BonusCriChance = equipSO.BonusCriChance;
    }
}
