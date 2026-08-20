using System;

[Serializable]
public class HeroUnlock
{
    public HeroNameEnum heroId;
    public CurrencyCost currencyCost;
    public HeroUnlockRequirement unlockRequirement;
}
