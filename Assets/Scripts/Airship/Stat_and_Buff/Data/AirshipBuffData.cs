using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 비행선에 적용 되는 버프 SO 데이터 <br/>
/// 영웅의 일시적 버프나 부품의 장착 효과처럼 비행선에 적용될 버프 원본 데이터.
/// </summary>
[CreateAssetMenu(menuName = "Airship/Buff Data")]
public class AirshipBuffData : ScriptableObject
{
    [SerializeField] private string buffName;
    [SerializeField] private float duration = -1f;
    [SerializeField] private List<AirshipStatModifierData> modifiers = new();

    public AirshipBuff CreateBuff(object owner = null)
    {
        AirshipBuff buff = new AirshipBuff(buffName, duration, owner);

        foreach (AirshipStatModifierData modifierData in modifiers)
        {
            if (modifierData == null)
            {
                continue;
            }
            buff.AddModifier(modifierData.CreateModifier());
        }

        return buff;
    }
}