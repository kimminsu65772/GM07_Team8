using System;
using UnityEngine;

/// <summary>
/// SO에 저장되는 원본 데이터 <br/>
/// 버프SO에서 구성하기 위해 씀.
/// </summary>
[Serializable]
public class AirshipStatModifierData
{
    [SerializeField] private AirshipStatType statType;
    [SerializeField] private AirshipModifierType modifierType;
    [SerializeField] private double value;

    public AirshipStatModifier CreateModifier()
    {
        return new AirshipStatModifier(statType, modifierType, value);
    }
}