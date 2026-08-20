using System;

[Serializable]
public class AirshipCannonUnlock
{
    public AirshipCannonType cannonType;
    public CurrencyCost currencyCost;
    public AirshipUnlockRequirement unlockRequirement;
}

[Serializable]
public class AirshipGearUnlock
{
    public AirshipGearType gearType;
    public CurrencyCost currencyCost;
    public AirshipUnlockRequirement unlockRequirement;
}
