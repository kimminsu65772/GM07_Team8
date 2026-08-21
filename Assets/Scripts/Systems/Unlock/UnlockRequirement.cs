using System;
using UnityEngine;

[Serializable]
public class AirshipUnlockRequirement
{
    public int requiredMaxClearedStage;
    public AirshipStatType requiredAirshipUpgradeStat;
    public int requiredAirshipUpgradeLevel;
}

[Serializable]
public class HeroUnlockRequirement
{
    public bool isNeedUnlockRequirement;
    public int requiredMaxClearedStage;
    // public HeroNameEnum requiredHeroId;
}

public readonly struct UnlockResult
{
    public readonly bool IsOwned;
    public readonly bool IsRequirementMet;
    public readonly bool HasEnoughCurrency;
    public readonly CurrencyCost Cost;
    public readonly string Reason;
    public bool CanUnlock => IsRequirementMet && HasEnoughCurrency && !IsOwned;

    public UnlockResult(bool isOwned,
        bool isRequirementMet,
        bool hasEnoughCurrency,
        CurrencyCost cost,
        string reason)
    {
        IsOwned = isOwned;
        IsRequirementMet = isRequirementMet;
        HasEnoughCurrency = hasEnoughCurrency;
        Cost = cost;
        Reason = reason;
    }
}