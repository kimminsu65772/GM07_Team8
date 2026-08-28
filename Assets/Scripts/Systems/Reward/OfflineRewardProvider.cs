using System;
using UnityEngine;

public static class OfflineRewardProvider
{

    private static TimeSpan MaxTimeSpan = TimeSpan.FromHours(16);

    private static RewardBundle offlineRewards;
    private static TimeSpan offlineTime;
    public static RewardBundle OfflineRewards => offlineRewards;
    public static TimeSpan OfflineTime => offlineTime;



    public static void ProvideOfflineReward()
    {
        TimeSpan offlineTime = CalculateOfflineTime(PlayerInfo.Instance.SaveData.LastSavedAtUtc);
        int playerMaxCleardStage = PlayerInfo.Instance.MaxClearedStage;

        offlineRewards = CalculateOfflineRewards(offlineTime, playerMaxCleardStage);

        if (offlineRewards.Rewards.Length > 0)
        {
            for (int i = 0; i < offlineRewards.Rewards.Length; i++)
            {
                CurrencyReward reward = offlineRewards.Rewards[i];
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

        offlineTime = DateTime.UtcNow - lastSavedAt;

        if (offlineTime > MaxTimeSpan)
        {
            offlineTime = MaxTimeSpan;
        }

        return offlineTime > TimeSpan.Zero ? offlineTime : TimeSpan.Zero;
    }

    private static RewardBundle CalculateOfflineRewards(TimeSpan offlineTime, int playerMaxCleardStage)
    {
        int offlineMinutes = (int)offlineTime.TotalMinutes;

        if (offlineMinutes <= 0 || playerMaxCleardStage < 0)
        {
            return new RewardBundle(Array.Empty<CurrencyReward>());
        }

        long goldReward = (long)offlineMinutes * (playerMaxCleardStage + 1);
        long gemReward = (long)offlineMinutes * (playerMaxCleardStage + 1) / 10;

        if (goldReward <= 0 && gemReward <= 0)
        {
            return new RewardBundle(Array.Empty<CurrencyReward>());
        }

        return new RewardBundle(new CurrencyReward[]
        {
            new CurrencyReward(CurrencyType.Gold, goldReward),
            new CurrencyReward(CurrencyType.Gems, gemReward)
        });
    }
}

