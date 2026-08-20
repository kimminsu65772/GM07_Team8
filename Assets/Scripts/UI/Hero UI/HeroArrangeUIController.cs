using System.Collections.Generic;
using UnityEngine;

public class HeroArrangeUIController : MonoBehaviour
{
    [Header("배치 슬롯 설정")]
    [SerializeField] private AirshipDropSlot[] heroArrangeSlots;

    [Header("영웅 리스트 관련 설정")]
    [SerializeField] private Transform heroListContent;
    [SerializeField] private GameObject heroSlotPrefab;
    [SerializeField] private HeroCatalog heroCatalog;

    private const int MaxHeroSlots = 5; // 최대 영웅 슬롯 수

    private PlayerInfo playerInfo;

    // 영웅 슬롯 재사용 전용
    private readonly List<HeroSlotUI> heroSlotPool = new();

    private void OnValidate()
    {
        if (heroArrangeSlots != null && heroArrangeSlots.Length > MaxHeroSlots)
        {
            Debug.LogWarning($"영웅 배치 슬롯은 최대 {MaxHeroSlots}개까지만 설정할 수 있습니다. 현재 설정된 슬롯 수: {heroArrangeSlots.Length}");
        }
    }

    private void OnEnable()
    {

        playerInfo = PlayerInfo.Instance;

        foreach (AirshipDropSlot slot in heroArrangeSlots)
        {
            if (slot == null)
            {
                Debug.LogWarning("배치 슬롯 중 null이 있습니다. HeroArrangeSlots 배열을 확인하세요.");
                continue;
            }
            slot.OnHeroDropped -= HandleHeroDropped;
            slot.OnSlotClearRequested -= HandleSlotClearRequested;
            slot.OnHeroDropped += HandleHeroDropped;
            slot.OnSlotClearRequested += HandleSlotClearRequested;
        }

        RefreshUI();
    }

    private void OnDisable()
    {
        foreach (AirshipDropSlot slot in heroArrangeSlots)
        {
            if (slot == null)
            {
                Debug.LogWarning("배치 슬롯 중 null이 있습니다. HeroArrangeSlots 배열을 확인하세요.");
                continue;
            }

            slot.OnHeroDropped -= HandleHeroDropped;
            slot.OnSlotClearRequested -= HandleSlotClearRequested;
        }
    }

    private void HandleHeroDropped(int slotIndex, HeroNameEnum heroId)
    {
        if (HeroFormationManager.Instance == null)
        {
            Debug.LogError("캐싱된 HeroFormationManager가 없습니다.");
            return;
        }

        bool result = HeroFormationManager.Instance.TrySetHeroToSlot(slotIndex, heroId);

        if (!result)
        {
            Debug.LogWarning($"영웅 {heroId}를 슬롯 {slotIndex}에 배치할 수 없습니다.");
            return;
        }

        Debug.Log($"영웅 {heroId}가 슬롯 {slotIndex}에 성공적으로 배치되었습니다.");
        //  UI 갱신
        RefreshUI();
    }

    private void HandleSlotClearRequested(int slotIndex)
    {
        if (HeroFormationManager.Instance == null)
        {
            Debug.LogError("캐싱된 HeroFormationManager가 없습니다.");
            return;
        }
        bool result = HeroFormationManager.Instance.ClearSlot(slotIndex);
        if (!result)
        {
            Debug.LogWarning($"슬롯 {slotIndex}을 비울 수 없습니다.");
            return;
        }
        Debug.Log($"슬롯 {slotIndex}이 성공적으로 비워졌습니다.");
        //  UI 갱신
        RefreshUI();
    }

    private void RefreshUI()
    {
        RefreshArrangeSlot();
        RefreshHeroList();
    }

    private void RefreshArrangeSlot()
    {
        foreach (AirshipDropSlot slot in heroArrangeSlots)
        {
            if (slot == null)
            {
                Debug.LogWarning("배치 슬롯 중 null이 있습니다. HeroArrangeSlots 배열을 확인하세요.");
                continue;
            }

            HeroSaveSlot saveSlot = FindFormationSlot(slot.SlotIndex);

            if (saveSlot == null || saveSlot.HeroId == HeroNameEnum.None)
            {
                slot.ClearHero();
                continue;
            }
            if (heroCatalog == null || !heroCatalog.TryGetHeroEntry(saveSlot.HeroId, out HeroEntry heroEntry))
            {
                slot.ClearHero();
                continue;
            }

            slot.SetHero(heroEntry);
        }
    }

    // UI 영웅 배치 인덱스에 맞는 HeroSaveSlot을 가져오는 메서드
    private HeroSaveSlot FindFormationSlot(int slotIndex)
    {
        if (playerInfo == null)
        {
            Debug.LogError("PlayerInfo 인스턴스가 없습니다.");
            return null;
        }

        if (playerInfo.HeroFormation == null)
        {
            Debug.LogError("PlayerInfo의 HeroFormation이 null입니다.");
            return null;
        }

        if (playerInfo.HeroFormation.Slots == null)
        {
            Debug.LogError("PlayerInfo의 HeroFormation.Slots가 null입니다.");
            return null;
        }

        foreach (HeroSaveSlot slot in playerInfo.HeroFormation.Slots)
        {
            if (slot == null)
            {
                Debug.LogWarning("HeroFormation.Slots 배열에 null 슬롯이 있습니다.");
                continue;
            }
            if (slot.SlotIndex == slotIndex)
            {
                return slot;
            }
        }

        return null;
    }

    private void RefreshHeroList()
    {
        List<(HeroEntry entry, HeroSaveData heroSaveData)> ownedHeroes = GetOwnedHeroes();

        bool result = PrepareHeroSlotPool(ownedHeroes.Count);

        if (!result)
        {
            Debug.LogError("영웅 슬롯 풀을 준비하는 데 실패했습니다.");
            return;
        }

        for (int i = 0; i < ownedHeroes.Count; i++)
        {
            HeroSlotUI slotUI = heroSlotPool[i];
            if (slotUI == null)
            {
                Debug.LogError($"HeroSlotUI가 null입니다. 인덱스: {i}");
                continue;
            }
            HeroEntry entry = ownedHeroes[i].entry;
            HeroSaveData saveData = ownedHeroes[i].heroSaveData;

            slotUI.SetupSlot(entry, saveData, true);

            if (HeroFormationManager.Instance == null)
            {
                Debug.LogError("캐싱된 HeroFormationManager가 없습니다.");
                continue;
            }
            bool isInFormation = HeroFormationManager.Instance.IsHeroInFormation(entry.HeroId);

            slotUI.SetFormationState(isInFormation);
            slotUI.SetDragEnabled(true);
        }
    }

    private List<(HeroEntry entry, HeroSaveData heroSaveData)> GetOwnedHeroes()
    {
        List<(HeroEntry entry, HeroSaveData heroSaveData)> ownedHeroes = new();

        foreach (HeroEntry entry in playerInfo.HeroEntries)
        {
            if (entry == null)
            {
                Debug.LogWarning("HeroEntry가 null입니다. PlayerInfo의 HeroEntries를 확인하세요.");
                continue;
            }

            if (playerInfo.TryGetHeroData(entry.HeroId, out HeroSaveData heroSaveData) && heroSaveData.IsOwned)
            {
                ownedHeroes.Add((entry, heroSaveData));
            }
        }

        return ownedHeroes;
    }

    private bool PrepareHeroSlotPool(int requiredCount)
    {

        if (heroSlotPrefab == null || heroListContent == null)
        {
            Debug.LogError("영웅 슬롯 프리팹 또는 영웅 리스트 Content가 설정되지 않았습니다.");
            return false;
        }

        // 필요한 슬롯 수만큼 풀을 준비
        while (heroSlotPool.Count < requiredCount)
        {
            GameObject newSlot = Instantiate(heroSlotPrefab, heroListContent);
            HeroSlotUI slotUI = newSlot.GetComponent<HeroSlotUI>();

            if (slotUI == null)
            {
                Debug.LogError("HeroSlotPrefab에 HeroSlotUI 컴포넌트가 없습니다.");
                Destroy(newSlot);
                return false;
            }

            heroSlotPool.Add(slotUI);
        }
        // 사용하지 않는 슬롯은 비활성화
        for (int i = 0; i < heroSlotPool.Count; i++)
        {
            heroSlotPool[i].gameObject.SetActive(i < requiredCount);
        }

        return true;
    }
}
