using System;

[Serializable]
public struct CurrencyReward
{
    public CurrencyType Type;
    public int Amount;

    public CurrencyReward(CurrencyType type, int amount)
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
public struct CurrencyCost
{
    public CurrencyType Type;
    public int Amount;
    public CurrencyCost(CurrencyType type, int amount)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException("소모 재화의 수량은 음수일 수 없습니다.", nameof(amount));
        }
        Type = type;
        Amount = amount;
    }
}