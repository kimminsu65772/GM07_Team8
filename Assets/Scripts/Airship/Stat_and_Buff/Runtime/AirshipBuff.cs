using System;
using System.Collections.Generic;

/// <summary>
/// 버프 SO 데이터를 사용해 런타임에서 구성한 버프. <br/>
/// 나중에 displayname하고 특정 버프 제거를 위한 id 변수 구분할 듯.
/// TODO 버프 제거 및 중복 버프 추가에 관한 로직
/// </summary>
[Serializable]
public class AirshipBuff
{ 
    public string BuffName { get; }
    public float Duration { get; }
    public float RemainingTime { get; private set; }
    // 버프를 부여한 주체. 특정 주체가 준 버프를 제거할 때 사용.
    public object Owner { get; }

    // 외부에서 직접 추가 X
    private readonly List<AirshipStatModifier> modifiers = new();
    public IReadOnlyList<AirshipStatModifier> Modifiers => modifiers;

    public bool HasInfiniteDuration => Duration < 0f;
    public bool HasExpired => !HasInfiniteDuration && RemainingTime <= 0f;

    public AirshipBuff(
        string buffName,
        float duration = -1f,
        object owner = null)
    {
        BuffName = buffName;
        Duration = duration;
        RemainingTime = duration;
        Owner = owner;
    }

    public void AddModifier(AirshipStatModifier modifier)
    {
        if (modifier == null)
        {
            return;
        }

        modifiers.Add(modifier);
    }

    public void Tick(float deltaTime)
    {
        if (HasInfiniteDuration)
        {
            return;
        }

        RemainingTime -= deltaTime;
    }
}