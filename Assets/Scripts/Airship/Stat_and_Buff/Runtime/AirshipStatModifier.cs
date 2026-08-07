using System;
using UnityEngine;

public enum AirshipStatType
{
    Attack,
    Defense,
    MaxHealth,
    CriticalChance,
    MoveSpeed,
    AttackSpeed
}
public enum AirshipModifierType
{
    Flat,
    PercentAdd,
    PercentMultiply
}
/// <summary>
/// 런타임에서 계산에 쓰는 실제 modifier.
/// </summary>
[Serializable]
public class AirshipStatModifier
{
    // 생성 후 바뀌지 않는 런타임 스탯 변경값.
    public AirshipStatType StatType { get; }
    public AirshipModifierType ModifierType { get; }
    public float Value { get; }

    public AirshipStatModifier(
        AirshipStatType statType,
        AirshipModifierType modifierType,
        float value)
    {
        StatType = statType;
        ModifierType = modifierType;
        Value = value;
    }
}
