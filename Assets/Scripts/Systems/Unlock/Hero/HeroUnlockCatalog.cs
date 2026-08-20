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
}
