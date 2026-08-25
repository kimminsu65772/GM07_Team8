using System;

/// <summary>
/// 업그레이드 상태를 나타내는 클래스.
/// </summary>
public class AirshipUpgradeState
{
    // 성장 공식의 시작점은 1레벨.
    public int AttackLevel { get; private set; } = 1;
    public int RecoveryLevel { get; private set; } = 1;
    public int MaxHealthLevel { get; private set; } = 1;
    public int CriticalLevel { get; private set; } = 1;

    public void SetLevels(
        int attackLevel,
        int recoveryLevel,
        int maxHealthLevel,
        int criticalLevel)
    {
        // 기존 세이브에 0이 들어있어도 1레벨로 보정.
        AttackLevel = Math.Max(1, attackLevel);
        RecoveryLevel = Math.Max(1, recoveryLevel);
        MaxHealthLevel = Math.Max(1, maxHealthLevel);
        CriticalLevel = Math.Max(1, criticalLevel);
    }

    public int GetLevel(AirshipStatType statType)
    {
        switch (statType)
        {
            case AirshipStatType.Attack:
                return AttackLevel;

            case AirshipStatType.Recovery:
                return RecoveryLevel;

            case AirshipStatType.MaxHealth:
                return MaxHealthLevel;

            case AirshipStatType.CriticalChance:
                return CriticalLevel;

            default:
                return -1;
        }
    }

    public void IncreaseStatLevel(AirshipStatType statType)
    {
        switch (statType)
        {
            case AirshipStatType.Attack:
                AttackLevel++;
                break;

            case AirshipStatType.Recovery:
                RecoveryLevel++;
                break;

            case AirshipStatType.MaxHealth:
                MaxHealthLevel++;
                break;

            case AirshipStatType.CriticalChance:
                CriticalLevel++;
                break;
        }
    }
}