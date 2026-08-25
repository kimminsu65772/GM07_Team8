using UnityEngine;
using System;

public static class StageRewardCalculator
{
    private const long BaseGoldReward = 1000;
    private const long KillGoldReward = 100;
    private const long BaseGearReward = 100;
    private const long KillGearReward = 10;
    private const long FixedGemReward = 300;

    private const float GearDropRate = 0.1f;

    private const double GoldRewardGrowthRate = 1.06d;
    private const double GearRewardGrowthRate = 1.06d;

    public static RewardBundle CalculateFirstClearReward(int stageNumber)
    {
        if (stageNumber <= 0)
        {
            Debug.LogError("스테이지는 0보다 커야 합니다.");
            return new RewardBundle(new CurrencyReward[0]);
        }

        long goldReward = CalculateGoldReward(stageNumber);
        long gearReward = CalculateGearReward(stageNumber);
        long gemReward = FixedGemReward;
        CurrencyReward[] rewards = new CurrencyReward[]
        {
            new CurrencyReward(CurrencyType.Gold, goldReward),
            new CurrencyReward(CurrencyType.Gear, gearReward),
            new CurrencyReward(CurrencyType.Gems, gemReward)
        };
        return new RewardBundle(rewards);
    }

    public static RewardBundle CalculateEnemyKillReward(int stageNumber)
    {
        if (stageNumber <= 0)
        {
            Debug.LogError("스테이지는 0보다 커야 합니다.");
            return new RewardBundle(new CurrencyReward[0]);
        }

        long goldReward = CalculateKillGoldReward(stageNumber);
        long gearReward = CalculateKillGearReward(stageNumber);
        // 랜덤으로 기어 드랍 여부 결정
        // Random.value는 0.0 이상 1.0 미만의 난수를 반환.
        bool isGearDropped = UnityEngine.Random.value < GearDropRate;
        CurrencyReward[] rewards;
        if (isGearDropped)
        {
            rewards = new CurrencyReward[]
            {
                new CurrencyReward(CurrencyType.Gold, goldReward),
                new CurrencyReward(CurrencyType.Gear, gearReward)
            };
        }
        else
        {
            rewards = new CurrencyReward[]
            {
                new CurrencyReward(CurrencyType.Gold, goldReward)
            };
        }
        return new RewardBundle(rewards);
    }

    private static long CalculateGoldReward(int stageNumber)
    {
        return BaseGoldReward * stageNumber;
    }

    private static long CalculateKillGoldReward(int stageNumber)
    {
        if (stageNumber <= 0)
        {
            Debug.LogError("스테이지는 0보다 커야 합니다.");
            return 0;
        }

        double goldReward = KillGoldReward * Math.Pow(GoldRewardGrowthRate, stageNumber - 1);

        return ConvertRewardToLong(goldReward);
    }

    private static long CalculateGearReward(int stageNumber)
    {
        return BaseGearReward * stageNumber;
    }

    private static long CalculateKillGearReward(int stageNumber)
    {
        if (stageNumber <= 0)
        {
            Debug.LogError("스테이지는 0보다 커야 합니다.");
            return 0;
        }

        double gearReward = KillGearReward * Math.Pow(GearRewardGrowthRate, stageNumber - 1);
        return ConvertRewardToLong(gearReward);
    }
    
    
    // 보상을 long으로 변환하는 함수
    private static long ConvertRewardToLong(double value)
    {
        if (double.IsNaN(value) ||
            value <= 0d)
        {
            return 0L;
        }

        if (double.IsInfinity(value) ||
            value >= long.MaxValue)
        {
            return long.MaxValue;
        }

        return (long)Math.Round(
            value,
            MidpointRounding.AwayFromZero);
    }
}
