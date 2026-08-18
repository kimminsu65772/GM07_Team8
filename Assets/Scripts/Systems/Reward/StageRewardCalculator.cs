using UnityEngine;

public static class StageRewardCalculator
{
    private const int BaseGoldReward = 1000;
    private const int KillGoldReward = 100;
    private const int BaseGearReward = 100;
    private const int KillGearReward = 10;
    private const int FixedGemReward = 300;

    private const float GearDropRate = 0.1f;

    private const float GoldRewardGrowthRate = 1.06f;
    private const float GearRewardGrowthRate = 1.06f;

    public static RewardBundle CalculateFirstClearReward(int stageNumber)
    {
        if (stageNumber <= 0)
        {
            Debug.LogError("스테이지는 0보다 커야 합니다.");
            return new RewardBundle(new CurrencyReward[0]);
        }

        int goldReward = CalculateGoldReward(stageNumber);
        int gearReward = CalculateGearReward(stageNumber);
        int gemReward = FixedGemReward;
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

        int goldReward = CalculateKillGoldReward(stageNumber);
        int gearReward = CalculateKillGearReward(stageNumber);
        // 랜덤으로 기어 드랍 여부 결정
        // Random.value는 0.0 이상 1.0 미만의 난수를 반환.
        bool isGearDropped = Random.value < GearDropRate;
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

    private static int CalculateGoldReward(int stageNumber)
    {
        return BaseGoldReward * stageNumber;
    }

    private static int CalculateKillGoldReward(int stageNumber)
    {
        if (stageNumber <= 0)
        {
            Debug.LogError("스테이지는 0보다 커야 합니다.");
            return 0;
        }

        float goldReward = KillGoldReward * Mathf.Pow(GoldRewardGrowthRate, stageNumber - 1);

        return Mathf.RoundToInt(goldReward);
    }

    private static int CalculateGearReward(int stageNumber)
    {
        return BaseGearReward * stageNumber;
    }

    private static int CalculateKillGearReward(int stageNumber)
    {
        if (stageNumber <= 0)
        {
            Debug.LogError("스테이지는 0보다 커야 합니다.");
            return 0;
        }

        float gearReward = KillGearReward * Mathf.Pow(GearRewardGrowthRate, stageNumber - 1);
        return Mathf.RoundToInt(gearReward);
    }
}
