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
}
