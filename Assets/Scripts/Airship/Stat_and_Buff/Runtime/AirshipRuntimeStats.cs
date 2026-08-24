using System;

/// <summary>
/// 런타임에서 계산된 최종 값.
/// </summary>
[Serializable]
public class AirshipRuntimeStats
{
    public double Attack { get; private set; }
    public double Recovery { get; private set; }
    public double MaxHealth { get; private set; }
    public float CriticalChance { get; private set; }
    public float MoveSpeed { get; private set; }
    public float AttackSpeed { get; private set; }

    public void SetStat(AirshipStatType statType, double value)
    {
        switch (statType)
        {
            case AirshipStatType.Attack:
                Attack = value;
                break;
            case AirshipStatType.Recovery:
                Recovery = value;
                break;
            case AirshipStatType.MaxHealth:
                MaxHealth = value;
                break;
            case AirshipStatType.CriticalChance:
                CriticalChance = (float)value;
                break;
            case AirshipStatType.MoveSpeed:
                MoveSpeed = (float)value;
                break;
            case AirshipStatType.AttackSpeed:
                AttackSpeed = (float)value;
                break;
        }
    }

    // ui쪽 같은데서 직관성을 위해 타입으로 스탯을 가져오기 위한 함수.
    public double GetStat(AirshipStatType statType)
    {
        return statType switch
        {
            AirshipStatType.Attack => Attack,
            AirshipStatType.Recovery => Recovery,
            AirshipStatType.MaxHealth => MaxHealth,
            AirshipStatType.CriticalChance => CriticalChance,
            AirshipStatType.MoveSpeed => MoveSpeed,
            AirshipStatType.AttackSpeed => AttackSpeed,
            _ => 0d
        };
    }
}