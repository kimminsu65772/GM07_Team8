using UnityEngine;

[CreateAssetMenu(fileName = "HeroLevelUpCostTable", menuName = "Game Data/Hero/HeroLevelCost")]
public class HeroLevelUpCostTable : ScriptableObject
{
    public enum CostGrowthType
    {
        Linear,
        Exponential
    }

    [SerializeField] private CurrencyType currencyType = CurrencyType.Gold;
    [SerializeField] private CostGrowthType growthType = CostGrowthType.Linear;

    [Header("Linear")]
    [SerializeField, Min(0)] private int baseCost = 100;
    [SerializeField, Min(0)] private int costPerLevel = 50;

    [Header("Exponential")]
    [SerializeField, Min(0)] private int exponentialBaseCost = 100;
    [SerializeField, Min(1f)] private float growthRate = 1.15f;

    public CurrencyType CurrencyType => currencyType;

    public long GetCost(int currentLevel)
    {
        currentLevel = Mathf.Max(1, currentLevel);

        return growthType switch
        {
            CostGrowthType.Linear => GetLinearCost(currentLevel),
            CostGrowthType.Exponential => GetExponentialCost(currentLevel),
            _ => baseCost
        };
    }

    public long GetCostForNextTenLevels(int currentLevel)
    {
        currentLevel = Mathf.Max(1, currentLevel);
        return growthType switch
        {
            CostGrowthType.Linear => GetLinearPlusTenLevelsCost(currentLevel),
            CostGrowthType.Exponential => GetExponentialPlusTenLevelsCost(currentLevel),
            _ => baseCost
        };
    }

    private long GetLinearCost(int currentLevel)
    {
        return baseCost + costPerLevel * (currentLevel - 1);
        
    }

    private long GetLinearPlusTenLevelsCost(int currentLevel)
    {
        long totalCost = 0;
        for (int i = 0; i < 10; i++)
        {
            totalCost += GetLinearCost(currentLevel + i);
        }
        return totalCost;
    }

    private long GetExponentialCost(int currentLevel)
    {
        float cost = exponentialBaseCost * Mathf.Pow(growthRate, currentLevel - 1);
        return (long)Mathf.CeilToInt(cost);
    }

    private long GetExponentialPlusTenLevelsCost(int currentLevel)
    {
        float totalCost = 0;
        for (int i = 0; i < 10; i++)
        {
            totalCost += GetExponentialCost(currentLevel + i);
        }
        return (long)Mathf.CeilToInt(totalCost);
    }
}