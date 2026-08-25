using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 업그레이드 할때 레벨당 증가량.
/// 스탯 테이블SO를 구성하기 위한 값.
/// </summary>
[Serializable]
public class AirshipStatGrowthData
{
    [SerializeField] private AirshipStatType statType;

    [Header("Stat Growth")]
    [SerializeField] private double baseValue;
    [SerializeField] private double valuePerLevel;
    [SerializeField, Min(1)] private int maxLevel = 100;

    [Header("Upgrade Cost")]
    [SerializeField, Min(0)] private int baseUpgradeCost;
    [SerializeField, Min(0)] private int upgradeCostPerLevel;

    public AirshipStatType StatType => statType;
    public int MaxLevel =>
        statType == AirshipStatType.CriticalChance
            ? maxLevel
            : int.MaxValue;

    public double GetValue(int level)
    {
        int safeLevel = Math.Max(1, level);

        if (statType == AirshipStatType.CriticalChance)
        {
            safeLevel = Math.Min(safeLevel, maxLevel);
        }

        return baseValue +
               valuePerLevel *
               (safeLevel - 1d);
    }

    // 현재 레벨에서 다음 레벨로 올라갈 때 필요한 비용
    public int GetUpgradeCost(int currentLevel)
    {
        if (currentLevel < 1 ||
            (statType == AirshipStatType.CriticalChance &&
             currentLevel >= maxLevel))
        {
            return 0;
        }

        double cost =
            baseUpgradeCost +
            (double)upgradeCostPerLevel *
            (currentLevel - 1d);

        return RoundCost(cost);
    }

    // 여러 레벨을 한 번에 올릴 때 필요한 총 비용
    public long GetTotalUpgradeCost(
        int currentLevel,
        int targetLevel)
    {
        int safeCurrentLevel =
            Math.Max(1, currentLevel);

        int safeTargetLevel =
            Math.Max(safeCurrentLevel, targetLevel);

        if (statType == AirshipStatType.CriticalChance)
        {
            safeTargetLevel =
                Math.Min(safeTargetLevel, maxLevel);
        }

        if (safeTargetLevel <= safeCurrentLevel)
        {
            return 0L;
        }

        long totalCost = 0L;

        for (int level = safeCurrentLevel;
             level < safeTargetLevel;
             level++)
        {
            totalCost += GetUpgradeCost(level);
        }

        return totalCost;
    }

    private static int RoundCost(double cost)
    {
        if (double.IsNaN(cost) ||
            cost <= 0d)
        {
            return 0;
        }

        if (double.IsInfinity(cost) ||
            cost >= int.MaxValue)
        {
            return int.MaxValue;
        }

        return (int)Math.Round(
            cost,
            MidpointRounding.AwayFromZero
        );
    }
}

/// <summary>
/// 스탯별 기본값과 선형 성장값을 정의하는 SO 데이터.
/// 테이블은 한개만 존재.
/// </summary>
[CreateAssetMenu(menuName = "Airship/Stat Table")]
public class AirshipStatTable : ScriptableObject
{
    [SerializeField]
    private List<AirshipStatGrowthData> stats =
        new List<AirshipStatGrowthData>();

    public double GetStatValue(
        AirshipStatType statType,
        int level)
    {
        AirshipStatGrowthData stat =
            FindStat(statType);

        if (stat == null)
        {
            return 0d;
        }

        return stat.GetValue(level);
    }

    // 한 레벨 업그레이드 비용
    public int GetUpgradeCost(
        AirshipStatType statType,
        int currentLevel)
    {
        AirshipStatGrowthData stat =
            FindStat(statType);

        if (stat == null)
        {
            return 0;
        }

        return stat.GetUpgradeCost(currentLevel);
    }

    // 여러 레벨 업그레이드 총 비용
    public long GetTotalUpgradeCost(
        AirshipStatType statType,
        int currentLevel,
        int targetLevel)
    {
        AirshipStatGrowthData stat =
            FindStat(statType);

        if (stat == null)
        {
            return 0L;
        }

        return stat.GetTotalUpgradeCost(
            currentLevel,
            targetLevel
        );
    }

    public int GetMaxLevel(
        AirshipStatType statType)
    {
        AirshipStatGrowthData stat =
            FindStat(statType);

        return stat == null
            ? 1
            : stat.MaxLevel;
    }

    private AirshipStatGrowthData FindStat(
        AirshipStatType statType)
    {
        return stats.Find(
            stat => stat.StatType == statType
        );
    }
}
