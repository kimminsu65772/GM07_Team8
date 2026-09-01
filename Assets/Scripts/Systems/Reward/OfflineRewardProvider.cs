using System;
using UnityEngine;
using System.Collections.Generic;

public static class OfflineRewardProvider
{

    private static readonly TimeSpan maxTimeSpan = TimeSpan.FromHours(16);
    private static OfflineReward offlineRewards;
    private static TimeSpan offlineTime;

    private static int offlineMinutes;
    public static OfflineReward OfflineRewards => offlineRewards;
    public static int OfflineMinutes => offlineMinutes;
    public static int MaxOfflineMinutes => (int)maxTimeSpan.TotalMinutes;

    private const int MinOfflineMinutes = 10;
    private const int LowMaterialIntervalMinutes = 45;
    private const int MidMaterialIntervalMinutes = 90;
    private const int HighMaterialIntervalMinutes = 180;

    private const float MidBaseChance = 0.25f;
    private const float MidChancePerFiveStage = 0.03f;

    private const float HighBaseChance = 0.10f;
    private const float HighChancePerTenStage = 0.03f;

    private const float MaxMidChance = 0.70f;
    private const float MaxHighChance = 0.40f;

    public static void ProvideOfflineReward()
    {
        TimeSpan offlineTime = CalculateOfflineTime(PlayerInfo.Instance.SaveData.LastSavedAtUtc);
        int playerMaxCleardStage = PlayerInfo.Instance.MaxClearedStage;

        offlineRewards = CalculateOfflineRewards(offlineTime, playerMaxCleardStage);

        if (offlineRewards.CurrencyRewards != null && offlineRewards.CurrencyRewards.Length != 0)
        {
            foreach (CurrencyReward reward in offlineRewards.CurrencyRewards)
            {
                if (reward.Amount <= 0) continue;
                PlayerInfo.Instance.AddCurrency(reward.Type, reward.Amount, SavePolicy.Soon);
            }
        }
        
        if (offlineRewards.ItemRewards != null && offlineRewards.ItemRewards.Length != 0)
        {
            foreach (ItemAmount itemReward in offlineRewards.ItemRewards)
            {
                if (itemReward.Amount <= 0) continue;
                PlayerInfo.Instance.AddItem(itemReward.ItemId, itemReward.Amount, SavePolicy.Soon);
            }
        }
    }

    private static TimeSpan CalculateOfflineTime(string lastSavedTime)
    {

        if (string.IsNullOrEmpty(lastSavedTime))
        {
            return TimeSpan.Zero;
        }

        if (!DateTime.TryParse(
            lastSavedTime,
            null,
            System.Globalization.DateTimeStyles.RoundtripKind,
            out DateTime lastSavedAt))
        {
            return TimeSpan.Zero;
        }

        offlineTime = DateTime.UtcNow - lastSavedAt;

        if (offlineTime > maxTimeSpan)
        {
            offlineTime = maxTimeSpan;
        }

        return offlineTime > TimeSpan.Zero ? offlineTime : TimeSpan.Zero;
    }

    private static OfflineReward CalculateOfflineRewards(TimeSpan offlineTime, int playerMaxCleardStage)
    {
        offlineMinutes = (int)offlineTime.TotalMinutes;

        if (offlineMinutes <= MinOfflineMinutes || playerMaxCleardStage < 0)
        {
            return new OfflineReward();
        }

        long goldReward = (long)offlineMinutes * (playerMaxCleardStage + 1);
        long gearReward = (long)offlineMinutes * (playerMaxCleardStage + 1) / 10;

        CurrencyReward[] offlineRewards = new CurrencyReward[]
        {
            new CurrencyReward(CurrencyType.Gold, goldReward),
            new CurrencyReward(CurrencyType.Gear, gearReward)
        };

        ItemAmount[] itemRewards = CalculateItemRewards(offlineMinutes, playerMaxCleardStage);

        return new OfflineReward(offlineRewards, itemRewards);
    }

    private static ItemAmount[] CalculateItemRewards(int offlineMinutes, int playerMaxCleardStage)
    {
        // 아이템이 몇종류가 나올지 알 수 없으므로 List로 생성
        List<ItemAmount> itemRewards = new();

        int lowMaterialAmount = offlineMinutes / LowMaterialIntervalMinutes;

        if (lowMaterialAmount > 0)
        {
            itemRewards.Add(new ItemAmount
            {
                ItemId = 10000,
                Amount = lowMaterialAmount
            });
        }

        int midRollCount = offlineMinutes / MidMaterialIntervalMinutes;
        float midMaterialChance = GetMidMaterialChance(playerMaxCleardStage);
        int midAmount = RollRewardAmount(midRollCount, midMaterialChance);

        if (midAmount > 0)
        {
            itemRewards.Add(new ItemAmount
            {
                ItemId = 10001,
                Amount = midAmount
            });
        }

        int highRollCount = offlineMinutes / HighMaterialIntervalMinutes;
        float highMaterialChance = GetHighMaterialChance(playerMaxCleardStage);
        int highAmount = RollRewardAmount(highRollCount, highMaterialChance);

        if (highAmount > 0)
        {
            itemRewards.Add(new ItemAmount
            {
                ItemId = 10002,
                Amount = highAmount
            });
        }

        return itemRewards.ToArray();
    }

    private static float GetMidMaterialChance(int maxClearedStage)
    {
        if (maxClearedStage < 5)
        {
            return 0f;
        }

        int stageStep = maxClearedStage / 5;
        float chance = MidBaseChance + stageStep * MidChancePerFiveStage;

        return Mathf.Clamp(chance, 0f, MaxMidChance);
    }

    private static float GetHighMaterialChance(int maxClearedStage)
    {
        if (maxClearedStage < 10)
        {
            return 0f;
        }
        int stageStep = maxClearedStage / 10;
        float chance = HighBaseChance + stageStep * HighChancePerTenStage;
        return Mathf.Clamp(chance, 0f, MaxHighChance);
    }

    private static int RollRewardAmount(int rollCount, float chance)
    {
        int amount = 0;
        for (int i = 0; i < rollCount; i++)
        {
            if (UnityEngine.Random.value < chance)
            {
                amount++;
            }
        }
        return amount;
    }
}

