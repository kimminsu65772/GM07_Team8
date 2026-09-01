using UnityEngine;
using System.Collections.Generic;

public static class EquipmentDecomposeCalculator
{
    // 장비 분해 후 획득할 아이템과 최소/최대 수량, 획득 확률을 담는 구조체
    private readonly struct DecomposeRewardRange
    {
        public readonly int itemId;
        public readonly int minAmount;
        public readonly int maxAmount;
        public readonly float chance;

        public DecomposeRewardRange(int itemId, int minAmount, int maxAmount, float chance)
        {
            this.itemId = itemId;
            this.minAmount = minAmount;
            this.maxAmount = maxAmount;
            this.chance = Mathf.Clamp01(chance);
        }
    }

    // 장비 하나만 분해하는 메서드
    public static List<ItemAmount> GenerateRewards(EquipmentSaveData equipment)
    {
        if (equipment == null) return new List<ItemAmount>();

        return RollRewardsByGrade(equipment.EquipGrade);
    }

    // 타입의 중복없이 정돈된 분해 결과를 전달할 수 있도록 Dictionary 형태의 분해 결과를 반환하는 메서드
    public static Dictionary<int, int> GenerateTotalRewards(List<EquipmentSaveData> equipments)
    {
        Dictionary<int, int> totalRewards = new();

        if (equipments == null) return totalRewards;

        foreach (var equipment in equipments)
        {
            if (equipment == null) continue;

            List<ItemAmount> rewards = GenerateRewards(equipment);

            foreach (var reward in rewards)
            {
                if (!totalRewards.TryAdd(reward.ItemId, reward.Amount))
                {
                    totalRewards[reward.ItemId] += reward.Amount;
                }
            }
        }
        return totalRewards;
    }

    private static List<ItemAmount> RollRewardsByGrade(EquipGradeEnum grade)
    {
        List<ItemAmount> rewards = new();
        List<DecomposeRewardRange> rewardRanges = GetRewardRanges(grade);

        foreach (var range in rewardRanges)
        {
            if (Random.value <= range.chance)
            {
                int amount = Random.Range(range.minAmount, range.maxAmount + 1);

                if (amount <= 0)
                {
                    continue;
                }

                rewards.Add(new ItemAmount { ItemId = range.itemId, Amount = amount });
            }
        }

        return rewards;
    }

    private static List<DecomposeRewardRange> GetRewardRanges(EquipGradeEnum grade)
    {
        return grade switch
        {
            EquipGradeEnum.Common => new List<DecomposeRewardRange>
        {
            new DecomposeRewardRange(10000, 1, 2, 1f),
            new DecomposeRewardRange(10001, 1, 1, 0.1f)
        },

            EquipGradeEnum.Rare => new List<DecomposeRewardRange>
        {
            new DecomposeRewardRange(10000, 2, 4, 1f),
            new DecomposeRewardRange(10001, 1, 2, 0.35f)
        },

            EquipGradeEnum.Epic => new List<DecomposeRewardRange>
        {
            new DecomposeRewardRange(10000, 3, 6, 1f),
            new DecomposeRewardRange(10001, 1, 3, 0.75f),
            new DecomposeRewardRange(10002, 1, 1, 0.15f)
        },

            EquipGradeEnum.Legendary => new List<DecomposeRewardRange>
        {
            new DecomposeRewardRange(10000, 5, 10, 1f),
            new DecomposeRewardRange(10001, 2, 5, 1f),
            new DecomposeRewardRange(10002, 1, 3, 0.5f)
        },

            _ => new List<DecomposeRewardRange>()
        };
    }
}
