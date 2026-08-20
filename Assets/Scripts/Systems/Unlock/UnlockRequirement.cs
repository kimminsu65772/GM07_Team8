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