using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HeroUnlockCatalog", menuName = "Unlock Table/HeroUnlockCatalog")]
public class HeroUnlockCatalog : ScriptableObject
{
    [SerializeField] private HeroUnlock[] heroUnlocks;

    public IReadOnlyList<HeroUnlock> HeroUnlocks => heroUnlocks;

    public bool TryGetHeroUnlock(HeroNameEnum heroId, out HeroUnlock heroUnlock)
    {
        heroUnlock = null;
        if (heroUnlocks == null || heroUnlocks.Length == 0)
        {
            Debug.LogWarning($"HeroUnlockCatalog: 카탈로그에 해금 정보가 등록되지 않았습니다.");
            return false;
        }

        foreach (HeroUnlock unlock in heroUnlocks)
        {
            if (unlock == null)
            {
                Debug.LogWarning("HeroUnlockCatalog: heroUnlocks 배열에 null 값이 존재합니다.");
                continue;
            }

            if (unlock.heroId == heroId)
            {
                heroUnlock = unlock;
                return true;
            }
        }
        
        Debug.LogWarning($"HeroUnlockCatalog: {heroId}의 해금 정보가 존재하지 않습니다.");
        return false;
    }

    public UnlockResult CheckHeroUnlock(HeroNameEnum heroId)
    {
        if (PlayerInfo.Instance == null)
        {
            Debug.LogError("플레이어의 정보가 없습니다.");
            return new UnlockResult(
                false,
                false,
                false,
                new CurrencyCost(CurrencyType.Gems, 0),
                "플레이어 정보 없음");
        }

        bool isOwned = PlayerInfo.Instance.IsHeroOwned(heroId);
        if (!TryGetHeroUnlock(heroId, out HeroUnlock unlock))
        {
            Debug.LogError($"영웅 {heroId}에 대한 해금 정보를 찾을 수 없습니다.");
            return new UnlockResult(
                isOwned,
                false,
                false,
                new CurrencyCost(CurrencyType.Gems, 0),
                "해금 정보 없음");
        }

        if (isOwned)
        {
            return new UnlockResult(
                isOwned,
                true,
                true,
                unlock.currencyCost,
                "이미 소유 중");
        }

        bool isRequirementMet =
            IsHeroRequirementMet(
                unlock.unlockRequirement,
                out string reason);

        bool hasEnoughCurrency =
            PlayerInfo.Instance.Wallet != null &&
            PlayerInfo.Instance.Wallet.Currencies != null &&
            PlayerInfo.Instance.Wallet.Currencies.TryGetValue(
                unlock.currencyCost.Type,
                out CurrencySaveData currency) &&
            currency.Amount >= unlock.currencyCost.Amount;

        if (isRequirementMet && !hasEnoughCurrency)
        {
            reason =
                $"필요한 재화가 부족합니다. " +
                $"{unlock.currencyCost.Type} {unlock.currencyCost.Amount} 필요";
        }

        return new UnlockResult(
            isOwned,
            isRequirementMet,
            hasEnoughCurrency,
            unlock.currencyCost,
            reason);
    }

    private bool IsHeroRequirementMet(
        HeroUnlockRequirement requirement,
        out string reason)
    {
        reason = string.Empty;

        if (requirement == null ||
            !requirement.isNeedUnlockRequirement)
        {
            return true;
        }

        if (PlayerInfo.Instance.MaxClearedStage <
            requirement.requiredMaxClearedStage)
        {
            reason =
                $"최대 클리어 스테이지 " +
                $"{requirement.requiredMaxClearedStage} 이상 필요";
            return false;
        }

        return true;
    }
}
