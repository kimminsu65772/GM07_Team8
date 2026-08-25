using System;

[Serializable]
public struct CurrencyReward
{
    public CurrencyType Type;
    public long Amount;

    public CurrencyReward(CurrencyType type, long amount)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException("지급 재화의 수량은 음수일 수 없습니다.", nameof(amount));
        }

        Type = type;
        Amount = amount;
    }
}

[Serializable]
public struct RewardBundle
{
    public CurrencyReward[] Rewards;
    public RewardBundle(CurrencyReward[] rewards)
    {
        Rewards = rewards;
    }
}

[Serializable]
public struct CurrencyCost
{
    public CurrencyType Type;
    public long  Amount;
    public CurrencyCost(CurrencyType type, long amount)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException("소모 재화의 수량은 음수일 수 없습니다.", nameof(amount));
        }
        Type = type;
        Amount = amount;
    }
}