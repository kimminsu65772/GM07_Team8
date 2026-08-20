using UnityEngine;
using System.Collections.Generic;
using System;

public class HeroFormationManager : MonoBehaviour
{
    [SerializeField] private HeroCatalog heroCatalog;

    private static HeroFormationManager instance;
    public static HeroFormationManager Instance => instance;
    private HeroFormationSaveData formationData;
    private Dictionary<HeroNameEnum, HeroSaveData> heroDataDictionary;


    private readonly Dictionary<HeroNameEnum, HeroFormationRuntimeSlot> runtimeSlotDictionary = new();
    // 세이브 데이터 내에는 따로 진형에 대한 구분없이 슬롯의 인덱스만 존재하므로,
    // HeroFormationRuntimeSlot을 통해 런타임에서 전방/후방 진형을 구분하여 필요한 컴포넌트에게 전달할 수 있도록 함.
    private readonly List<HeroFormationRuntimeSlot> FrontLineSlots = new();
    private readonly List<HeroFormationRuntimeSlot> BackLineSlots = new();

    public event Action OnFormationChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        instance.Initialize();
        DontDestroyOnLoad(this.gameObject);
    }

    public void Initialize()
    {
        if (heroCatalog == null)
        {
            Debug.LogError("HeroCatalog가 할당되지 않았습니다.");
            return;
        }

        if (PlayerInfo.Instance.Heroes == null)
        {
            Debug.LogError("저장된 영웅 데이터가 비어있습니다.");
            return;
        }

        if (PlayerInfo.Instance.HeroFormation == null)
        {
            Debug.LogError("저장된 진형 데이터가 없습니다.");
            return;
        }

        if (PlayerInfo.Instance.HeroFormation.Slots == null)
        {
            Debug.LogError("저장된 진형 슬롯 데이터가 비어있습니다.");
            return;
        }
        heroDataDictionary = PlayerInfo.Instance.Heroes;
        formationData = PlayerInfo.Instance.HeroFormation;
        RefreshRuntimeFormation();
    }

    public void RefreshRuntimeFormation()
    {
        FrontLineSlots.Clear();
        BackLineSlots.Clear();
        runtimeSlotDictionary.Clear();

        foreach (HeroSaveSlot slot in formationData.Slots)
        {
            if (slot == null || slot.HeroId == HeroNameEnum.None)
            {
                continue;
            }
            if (!heroDataDictionary.TryGetValue(slot.HeroId, out HeroSaveData heroData))
            {
                Debug.LogError($"저장 목록에 없는 영웅입니다.");
                continue;
            }
            if (!heroCatalog.TryGetHeroEntry(slot.HeroId, out HeroEntry heroEntry))
            {
                Debug.LogError($"HeroCatalog에서 영웅 정보를 찾을 수 없습니다: {slot.HeroId}.");
                continue;
            }
            if (heroData.IsOwned == false)
            {
                Debug.LogError($"미보유 영웅입니다.: {slot.HeroId}.");
                continue;
            }
            HeroFormationRuntimeSlot runtimeSlot = new HeroFormationRuntimeSlot(slot.SlotIndex, slot.HeroId, heroData, heroEntry);
            if (runtimeSlot != null)
            {
                runtimeSlotDictionary[slot.HeroId] = runtimeSlot;
            }
            if (heroEntry.HeroLocation == HeroLocationEnum.Front)
            {
                FrontLineSlots.Add(runtimeSlot);
            }
            else if (heroEntry.HeroLocation == HeroLocationEnum.Back)
            {
                BackLineSlots.Add(runtimeSlot);
            }
        }
    }

    public IReadOnlyList<HeroFormationRuntimeSlot> GetFrontLineSlots()
    {
        return FrontLineSlots;
    }

    public IReadOnlyList<HeroFormationRuntimeSlot> GetBackLineSlots()
    {
        return BackLineSlots;
    }

    // 슬롯 인덱스와 영웅의 Id를 받아 해당 슬롯에 영웅을 배치한다.
    // 만약 다른 슬롯에 이미 배치되어 있는 영웅이라면, 그 슬롯을 비우고 새로운 슬롯에 배치한다.
    public bool TrySetHeroToSlot(int slotIndex, HeroNameEnum heroId)
    {
        if (formationData == null || formationData.Slots == null)
        {
            Debug.LogError("진형 데이터가 초기화되지 않았습니다.");
            return false;
        }
        if (!TryGetSlotByIndex(slotIndex, out HeroSaveSlot targetSlot))
        {
            Debug.LogError($"해당 슬롯 인덱스를 찾을 수 없습니다: {slotIndex}.");
            return false;
        }
        if (heroId == HeroNameEnum.None)
        {
            Debug.LogError("영웅 Id가 비어있습니다.");
            return false;
        }
        if (!heroDataDictionary.TryGetValue(heroId, out HeroSaveData heroData))
        {
            return false;
        }

        if (!heroData.IsOwned)
        {
            return false;
        }

        // 배치하려는 영웅이 이미 다른 슬롯에 배치되어 있는 경우, 배치 슬롯에 있는 HeroName을 서로 교환한다.
        if (TryGetSlotByHeroId(heroId, out HeroSaveSlot heroSlot) && heroSlot.HeroId == heroId)
        {
            HeroNameEnum tempHeroId = targetSlot.HeroId;
            targetSlot.HeroId = heroId;
            heroSlot.HeroId = tempHeroId;
        }
        else
        {
            targetSlot.HeroId = heroId;
        }

        RefreshRuntimeFormation();
        OnFormationChanged?.Invoke();
        SaveScheduler.Instance.RequestSave(SavePolicy.Soon);
        return true;
    }

    public bool ClearSlot(int slotIndex)
    {
        if (formationData == null || formationData.Slots == null)
        {
            Debug.LogError("진형 데이터가 초기화되지 않았습니다.");
            return false;
        }
        foreach (HeroSaveSlot slot in formationData.Slots)
        {
            if (slot != null && slot.SlotIndex == slotIndex)
            {
                slot.HeroId = HeroNameEnum.None;
                RefreshRuntimeFormation();
                OnFormationChanged?.Invoke();
                SaveScheduler.Instance.RequestSave(SavePolicy.Soon);
                return true;
            }
        }
        Debug.LogWarning($"해당 슬롯 인덱스에 배치된 영웅이 없습니다: {slotIndex}.");
        return true;
    }

    public bool IsHeroInFormation(HeroNameEnum heroId)
    {
        if (heroId == HeroNameEnum.None)
        {
            Debug.LogError("영웅 Id가 비어있습니다.");
            return false;
        }
        return runtimeSlotDictionary.ContainsKey(heroId);
    }

    // 슬롯 인덱스의 유효성을 확인하고, 해당 슬롯이 담고 있는 HeroSaveSlot을 반환한다. 슬롯이 존재하지 않으면 false를 반환한다.
    private bool TryGetSlotByIndex(int slotIndex, out HeroSaveSlot slot)
    {
        slot = null;

        if (formationData == null || formationData.Slots == null)
        {
            Debug.LogError("진형 데이터가 초기화되지 않았습니다.");
            return false;
        }

        foreach (HeroSaveSlot candidate in formationData.Slots)
        {
            if (candidate != null && candidate.SlotIndex == slotIndex)
            {
                slot = candidate;
                return true;
            }
            else
            {
                continue;
            }
        }

        return false;
    }

    // 영웅의 이름을 받아 해당 영웅이 배치되어 있는 슬롯을 반환한다. 슬롯이 존재하지 않으면 false를 반환한다.
    private bool TryGetSlotByHeroId(HeroNameEnum heroId, out HeroSaveSlot slot)
    {
        slot = null;
        if (formationData == null || formationData.Slots == null)
        {
            Debug.LogError("진형 데이터가 초기화되지 않았습니다.");
            return false;
        }
        if (heroId == HeroNameEnum.None)
        {
            Debug.LogError("영웅 Id가 비어있습니다.");
            return false;
        }
        foreach (HeroSaveSlot candidate in formationData.Slots)
        {
            if (candidate != null && candidate.HeroId == heroId)
            {
                slot = candidate;
                return true;
            }
            else
            {
                continue;
            }
        }
        return false;
    }
}

public class HeroFormationRuntimeSlot
{
    public int SlotIndex { get; }
    public HeroNameEnum HeroId { get; }
    public HeroSaveData HeroData { get; }
    public HeroEntry HeroEntry { get; }

    public HeroFormationRuntimeSlot(int slotIndex, HeroNameEnum heroId, HeroSaveData heroData, HeroEntry heroEntry)
    {
        SlotIndex = slotIndex;
        HeroId = heroId;
        HeroData = heroData;
        HeroEntry = heroEntry;
    }
}
