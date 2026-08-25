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
    [SerializeField, Min(0)] private long baseUpgradeCost;
    [SerializeField, Min(0)] private long upgradeCostPerLevel;

    public AirshipStatType StatType => statType;

    public int MaxLevel =>
        statType == AirshipStatType.CriticalChance
            ? Math.Max(1, maxLevel)
            : int.MaxValue;

    public double GetValue(int level)
    {
        int safeLevel =
            Math.Max(1, level);

        if (statType == AirshipStatType.CriticalChance)
        {
            safeLevel =
                Math.Min(safeLevel, MaxLevel);
        }

        return baseValue +
               valuePerLevel *
               (safeLevel - 1d);
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

        safeTargetLevel =
            Math.Min(safeTargetLevel, MaxLevel);

        if (safeTargetLevel <= safeCurrentLevel)
        {
            return 0L;
        }

        long safeBaseCost =
            Math.Max(0L, baseUpgradeCost);

        long safeCostPerLevel =
            Math.Max(0L, upgradeCostPerLevel);

        long totalCost = 0L;

        // 현재 레벨부터 목표 레벨 직전까지의 비용 합산
        for (int level = safeCurrentLevel;
             level < safeTargetLevel;
             level++)
        {
            long levelCost =
                CalculateCost(
                    safeBaseCost,
                    safeCostPerLevel,
                    level
                );

            // 총 비용 오버플로 방지
            if (levelCost >
                long.MaxValue - totalCost)
            {
                return long.MaxValue;
            }

            totalCost += levelCost;
        }

        return totalCost;
    }

    private static long CalculateCost(
        long baseCost,
        long costPerLevel,
        int currentLevel)
    {
        long levelOffset =
            currentLevel - 1L;

        if (levelOffset <= 0L)
        {
            return baseCost;
        }

        if (costPerLevel > 0L &&
            levelOffset >
            (long.MaxValue - baseCost) /
            costPerLevel)
        {
            return long.MaxValue;
        }

        return baseCost +
               costPerLevel * levelOffset;
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