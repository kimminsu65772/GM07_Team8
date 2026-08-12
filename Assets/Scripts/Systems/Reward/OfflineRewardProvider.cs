using System;
using UnityEngine;

public static class OfflineRewardProvider
{
    private static CurrencyReward[] offlineRewards;
    public static CurrencyReward[] OfflineRewards => offlineRewards;

    public static void ProvideOfflineReward()
    {
        TimeSpan offlineTime = CalculateOfflineTime(PlayerInfo.Instance.SaveData.LastSavedAtUtc);
        int playerMaxCleardStage = PlayerInfo.Instance.MaxClearedStage;

        offlineRewards = CalculateOfflineRewards(offlineTime, playerMaxCleardStage);

        if (offlineRewards.Length > 0)
        {
            for (int i = 0; i < offlineRewards.Length; i++)
            {
                CurrencyReward reward = offlineRewards[i];
                PlayerInfo.Instance.AddCurrency(reward.Type, reward.Amount, SavePolicy.Soon);
                Debug.Log($"{reward.Type}: {reward.Amount}");
            }
        }
        else
        {
            Debug.Log("지급할 보상이 없습니다.");
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

        TimeSpan offlineTime = DateTime.UtcNow - lastSavedAt;

        return offlineTime > TimeSpan.Zero ? offlineTime : TimeSpan.Zero;
    }

    private static CurrencyReward[] CalculateOfflineRewards(TimeSpan offlineTime, int playerMaxCleardStage)
    {
        int offlineMinutes = (int)offlineTime.TotalMinutes;

        if (offlineMinutes <= 0 || playerMaxCleardStage < 0)
        {
            return Array.Empty<CurrencyReward>();
        }

        int goldReward = offlineMinutes * (playerMaxCleardStage + 1);
        int gemReward = offlineMinutes * (playerMaxCleardStage + 1) / 10;

        if (goldReward <= 0 && gemReward <= 0)
        {
            return Array.Empty<CurrencyReward>();
        }

        return new CurrencyReward[]
        {
            new CurrencyReward { Type = CurrencyType.Gold, Amount = goldReward },
            new CurrencyReward { Type = CurrencyType.Gems, Amount = gemReward }
        };
    }
}

