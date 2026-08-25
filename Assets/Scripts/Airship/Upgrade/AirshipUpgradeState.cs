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

    // 여러 레벨을 한 번에 올린 뒤 최종 레벨을 반영한다.
    public void SetLevel(
        AirshipStatType statType,
        int level)
    {
        int safeLevel =
            Math.Max(1, level);

        switch (statType)
        {
            case AirshipStatType.Attack:
                AttackLevel = safeLevel;
                break;

            case AirshipStatType.Recovery:
                RecoveryLevel = safeLevel;
                break;

            case AirshipStatType.MaxHealth:
                MaxHealthLevel = safeLevel;
                break;

            case AirshipStatType.CriticalChance:
                CriticalLevel = safeLevel;
                break;
        }
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
}