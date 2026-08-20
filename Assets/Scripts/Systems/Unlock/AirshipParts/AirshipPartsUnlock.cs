using System;
using UnityEngine;

[Serializable]
public class AirshipCannonUnlock
{
    public AirshipCannonType cannonType;
    public CurrencyType currencyType;
    public int unlockCost;
    public UnlockRequirement unlockRequirement;
}

[Serializable]
public class AirshipGearUnlock
{
    public AirshipGearType gearType;
    public CurrencyType currencyType;
    public int unlockCost;
    public UnlockRequirement unlockRequirement;
}

[Serializable]
public class UnlockRequirement
{
    public int requiredMaxClearedStage;
    public AirshipStatType requiredAirshipUpgradeStat;
    public int requiredAirshipUpgradeLevel;
}
