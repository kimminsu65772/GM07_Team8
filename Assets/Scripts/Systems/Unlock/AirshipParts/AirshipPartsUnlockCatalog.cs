using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AirshipPartsUnlockCatalog", menuName = "Unlock Table/PartsUnlockCatalog")]
public class AirshipPartsUnlockCatalog : ScriptableObject
{
    [SerializeField] private List<AirshipCannonUnlock> cannonUnlocks = new();
    [SerializeField] private List<AirshipGearUnlock> gearUnlocks = new();

    public IReadOnlyList<AirshipCannonUnlock> CannonUnlocks => cannonUnlocks;
    public IReadOnlyList<AirshipGearUnlock> GearUnlocks => gearUnlocks;

    // 해당 캐논 타입의 해금 조건을 가져오는 메서드
    // 지금 당장 해금 조건으로는 스테이지 클리어랑 업그레이드 레벨 달성 여부 두 가지만 존재
    public bool TryGetCannonUnlock(AirshipCannonType cannonType, out AirshipCannonUnlock unlock)
    {
        unlock = null;
        if (cannonUnlocks == null || cannonUnlocks.Count == 0)
        {
            Debug.LogWarning("AirshipPartsUnlockCatalog: 캐논 해금 데이터가 비어있습니다.");
            return false;
        }

        foreach (AirshipCannonUnlock cannonUnlock in cannonUnlocks)
        {
            if (cannonUnlock == null)
            {
                continue;
            }

            if (cannonUnlock.cannonType == cannonType)
            {
                unlock = cannonUnlock;
                return true;
            }
        }

        
        return false;
    }

    // 해당 기어 타입의 해금 조건을 가져오는 메서드
    public bool TryGetGearUnlock(AirshipGearType gearType, out AirshipGearUnlock unlock)
    {
        unlock = null;

        if (gearUnlocks == null || gearUnlocks.Count == 0)
        {
            Debug.LogWarning("AirshipPartsUnlockCatalog: 기어 해금 데이터가 비어있습니다.");
            return false;
        }
        foreach (AirshipGearUnlock gearUnlock in gearUnlocks)
        {
            if (gearUnlock == null)
            {
                continue;
            }

            if (gearUnlock.gearType == gearType)
            {
                unlock = gearUnlock;
                return true;
            }
        }
        
        return false;
    }

    public UnlockResult CheckCannonUnlock(AirshipCannonType gearType)
    {
        if (PlayerInfo.Instance == null)
        {
            Debug.LogError("플레이어의 정보가 없습니다.");
            return new UnlockResult(false, false, false, new CurrencyCost(CurrencyType.Gear, 0), "플레이어 정보 없음");
        }

        bool isOwned = PlayerInfo.Instance.IsCannonOwned(gearType);
        if (!TryGetCannonUnlock(gearType, out AirshipCannonUnlock unlock))
        {
            Debug.LogError($"캐논 타입 {gearType}에 대한 해금 정보를 찾을 수 없습니다.");
            return new UnlockResult(isOwned, false, false, new CurrencyCost(CurrencyType.Gear, 0), "해금 정보 없음");
        }

        if (isOwned)
        {
            return new UnlockResult(isOwned, true, true, unlock.currencyCost, "이미 소유 중");
        }

        bool isRequirementMet = IsAirshipPartsRequirementMet(unlock.unlockRequirement, out string reason);

        bool hasEnoughCurrency = 
            PlayerInfo.Instance.Wallet.Currencies.TryGetValue(unlock.currencyCost.Type, out CurrencySaveData currency) && currency.Amount >= unlock.currencyCost.Amount;

        if (isRequirementMet && !hasEnoughCurrency)
        {
            reason = $"필요한 재화가 부족합니다. {unlock.currencyCost.Type} {unlock.currencyCost.Amount} 필요";
        }

        return new UnlockResult(isOwned, isRequirementMet, hasEnoughCurrency, unlock.currencyCost, reason);
    }

    public UnlockResult CheckGearUnlock(AirshipGearType gearType)
    {
        if (PlayerInfo.Instance == null)
        {
            Debug.LogError("플레이어의 정보가 없습니다.");
            return new UnlockResult(false, false, false, new CurrencyCost(CurrencyType.Gear, 0), "플레이어 정보 없음");
        }

        bool isOwned = PlayerInfo.Instance.IsGearOwned(gearType);
        if (!TryGetGearUnlock(gearType, out AirshipGearUnlock unlock))
        {
            Debug.LogError($"기어 타입 {gearType}에 대한 해금 정보를 찾을 수 없습니다.");
            return new UnlockResult(isOwned, false, false, new CurrencyCost(CurrencyType.Gear, 0), "해금 정보 없음");
        }

        if (isOwned)
        {
            return new UnlockResult(isOwned, true, true, unlock.currencyCost, "이미 소유 중");
        }

        bool isRequirementMet = IsAirshipPartsRequirementMet(unlock.unlockRequirement, out string reason);

        bool hasEnoughCurrency =
            PlayerInfo.Instance.Wallet != null &&
            PlayerInfo.Instance.Wallet.Currencies != null &&
            PlayerInfo.Instance.Wallet.Currencies.TryGetValue(
                unlock.currencyCost.Type,
                out CurrencySaveData currency) &&
            currency.Amount >= unlock.currencyCost.Amount;

        if (isRequirementMet && !hasEnoughCurrency)
        {
            reason = $"필요한 재화가 부족합니다. {unlock.currencyCost.Type} {unlock.currencyCost.Amount} 필요";
        }

        return new UnlockResult(isOwned, isRequirementMet, hasEnoughCurrency, unlock.currencyCost, reason);
    }

    private bool IsAirshipPartsRequirementMet(AirshipUnlockRequirement requirement, out string reason)
    {
        reason = string.Empty;
        if (requirement == null)
        {
            Debug.LogError("해금 조건이 없습니다.");
            return true;
        }

        if (PlayerInfo.Instance.MaxClearedStage < requirement.requiredMaxClearedStage)
        {
            reason = $"최대 클리어 스테이지 {requirement.requiredMaxClearedStage} 이상 필요";
            return false;
        }

        int currentUpgradeLevel = PlayerInfo.Instance.GetAirshipUpgradeLevel(requirement.requiredAirshipUpgradeStat);
        if (currentUpgradeLevel < requirement.requiredAirshipUpgradeLevel)
        {
            reason = $"{requirement.requiredAirshipUpgradeStat} 업그레이드 레벨 {requirement.requiredAirshipUpgradeLevel} 이상 필요";
            return false;
        }
        return true;
    }
}
