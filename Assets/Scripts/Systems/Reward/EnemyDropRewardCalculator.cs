using UnityEngine;
using System.Collections.Generic;

public static class EnemyDropRewardCalculator
{
    private const int LowMaterialId = 10000;
    private const int MidMaterialId = 10001;
    private const int HighMaterialId = 10002;

    private const float LowMaterialDropChance = 0.025f;

    private const float MidBaseDropChance = 0.005f;
    private const float MidChancePerFiveStage = 0.002f;
    private const float MaxMidDropChance = 0.015f;

    private const float HighBaseDropChance = 0.0015f;
    private const float HighChancePerTenStage = 0.001f;
    private const float MaxHighDropChance = 0.006f;

    public static List<ItemAmount> RollEnemyKillDrops(int currentStage)
    {
        List<ItemAmount> rewards = new();

        TryAddDrop(rewards, LowMaterialId, LowMaterialDropChance);

        float midMaterialChance = GetMidMaterialDropChance(currentStage);
        TryAddDrop(rewards, MidMaterialId, midMaterialChance);

        float hightMaterialChance = GetHighMaterialDropChance(currentStage);
        TryAddDrop(rewards, HighMaterialId, hightMaterialChance);

        return rewards;
    }

    private static void TryAddDrop(List<ItemAmount> rewards, int itemId, float dropChance)
    {
        if (dropChance <= 0) return;

        if (UnityEngine.Random.value < dropChance)
        {
            rewards.Add(new ItemAmount
            {
                ItemId = itemId,
                Amount = 1
            });
        }
    }

    private static float GetMidMaterialDropChance(int currentStage)
    {
        if (currentStage < 5) return 0;

        int stageStep = currentStage / 5;

        float chance = MidBaseDropChance + stageStep * MidChancePerFiveStage;

        return Mathf.Clamp(chance, 0, MaxMidDropChance);
    }

    private static float GetHighMaterialDropChance(int currentStage)
    {
        if (currentStage < 10) return 0;

        int stageStep = currentStage / 10;

        float chance = HighBaseDropChance + stageStep * HighChancePerTenStage;

        return Mathf.Clamp(chance, 0, MaxHighDropChance);
    }
}
