using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroArrangeUIController : MonoBehaviour
{
    [Header("배치 슬롯 설정")]
    [SerializeField] private AirshipDropSlot[] heroArrangeSlots;

    [Header("영웅 리스트 관련 설정")]
    [SerializeField] private Transform heroListContent;
    [SerializeField] private GameObject heroSlotPrefab;
    [SerializeField] private HeroCatalog heroCatalog;
    [SerializeField] private HeroArrangeStatUI[] heroArrangeStatUIs;

    [Header("화면 어두워짐(Dim) 연출 설정")]
    [SerializeField] private GameObject dimPanelObject;

    private const int MaxHeroSlots = 5; // 최대 영웅 슬롯 수
    private int currentSelectedSlotIndex = -1;

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
                continue;
            }
            slot.OnSlotClicked -= HandleSlotClicked;
            slot.OnSlotClearRequested -= HandleSlotClearRequested;
            slot.OnSlotClicked += HandleSlotClicked;
            slot.OnSlotClearRequested += HandleSlotClearRequested;
        }
        currentSelectedSlotIndex = -1;
        RefreshUI();
    }

    private void OnDisable()
    {
        foreach (AirshipDropSlot slot in heroArrangeSlots)
        {
            if (slot == null) continue;
            slot.OnSlotClicked -= HandleSlotClicked;
            slot.OnSlotClearRequested -= HandleSlotClearRequested;
        }
    }

    private void HandleSlotClicked(int slotIndex)
    {
        currentSelectedSlotIndex = slotIndex;
        Debug.Log($"{slotIndex}번 슬롯이 선택되었습니다. 아래 스크롤에서 배치할 영웅을 선택하세요.");

        if (dimPanelObject != null)
        {
            dimPanelObject.SetActive(true);
        }

        // 모든 슬롯의 강조 효과를 끄고, 선택된 슬롯만 강조 켜기
        foreach (var slot in heroArrangeSlots)
        {
            if (slot == null) continue;
            bool isTarget = (slot.SlotIndex == slotIndex);
            slot.SetHighlight(isTarget);
        }
    }

    private void HandleSlotClearRequested(int slotIndex)
    {
        if (HeroFormationManager.Instance == null) return;

        bool result = HeroFormationManager.Instance.ClearSlot(slotIndex);
        if (!result) return;

        Debug.Log($"슬롯 {slotIndex}이 성공적으로 비워졌습니다.");
        ResetSelectionState();
        RefreshUI();
    }

    public void OnHeroClickedFromScroll(HeroNameEnum heroId)
    {
        if (currentSelectedSlotIndex == -1)
        {
            Debug.LogWarning("먼저 영웅을 배치할 비행선 슬롯을 선택해주세요!");
            return;
        }

        if (HeroFormationManager.Instance == null)
        {
            Debug.LogError("캐싱된 HeroFormationManager가 없습니다.");
            return;
        }

        bool result = HeroFormationManager.Instance.TrySetHeroToSlot(currentSelectedSlotIndex, heroId);

        if (!result)
        {
            Debug.LogWarning($"영웅 {heroId}를 슬롯 {currentSelectedSlotIndex}에 배치할 수 없습니다.");
            return;
        }

        Debug.Log($"슬롯 {currentSelectedSlotIndex}에 영웅 {heroId} 배치 완료!");

        ResetSelectionState();
        RefreshUI();
    }

    private void ResetSelectionState()
    {
        currentSelectedSlotIndex = -1;

        if (dimPanelObject != null)
        {
            dimPanelObject.SetActive(false);
        }

        if (heroArrangeSlots != null)
        {
            foreach (var slot in heroArrangeSlots)
            {
                slot?.SetHighlight(false);
            }
        }
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
            if (slot == null) continue;         

            int slotIndex = slot.SlotIndex;
            HeroArrangeStatUI statUI = GetArrangeStatUI(slotIndex);
            HeroSaveSlot saveSlot = FindFormationSlot(slotIndex);

            if (saveSlot == null || saveSlot.HeroId == HeroNameEnum.None)
            {
                slot.ClearHero();
                statUI.ClearHeroStatUIs();
                
                var powerDisplay = statUI?.GetComponent<HeroPowerDisplay>() ?? statUI?.GetComponentInChildren<HeroPowerDisplay>();
                powerDisplay?.ClearHero();
                continue;
            }
            if (heroCatalog == null || !heroCatalog.TryGetHeroEntry(saveSlot.HeroId, out HeroEntry heroEntry))
            {
                slot.ClearHero();
                statUI.ClearHeroStatUIs();
                continue;
            }

            if (slot.TrySetHero(heroEntry))
            {
                statUI?.SetHeroStatUIs(heroEntry);
                var powerDisplay = statUI?.GetComponent<HeroPowerDisplay>() ?? statUI?.GetComponentInChildren<HeroPowerDisplay>();
                powerDisplay?.SetHero(heroEntry);
            }
        }
    }

    private HeroArrangeStatUI GetArrangeStatUI(int slotIndex)
    {
        if (heroArrangeStatUIs == null || slotIndex < 0 || slotIndex >= heroArrangeStatUIs.Length) return null;
        return heroArrangeStatUIs[slotIndex];
    }

    // UI 영웅 배치 인덱스에 맞는 HeroSaveSlot을 가져오는 메서드
    private HeroSaveSlot FindFormationSlot(int slotIndex)
    {
        if (playerInfo?.HeroFormation?.Slots == null) return null;

        foreach (HeroSaveSlot slot in playerInfo.HeroFormation.Slots)
        {
            if (slot != null && slot.SlotIndex == slotIndex) return slot;
        }
        return null;
    }

    private void RefreshHeroList()
    {
        List<(HeroEntry entry, HeroSaveData heroSaveData)> ownedHeroes = GetOwnedHeroes();
        if (!PrepareHeroSlotPool(ownedHeroes.Count)) return;

        for (int i = 0; i < ownedHeroes.Count; i++)
        {
            HeroSlotUI slotUI = heroSlotPool[i];
            if (slotUI == null) continue;
            
            HeroEntry entry = ownedHeroes[i].entry;
            HeroSaveData saveData = ownedHeroes[i].heroSaveData;

            slotUI.SetupSlot(entry, saveData, true);

            bool isInFormation = HeroFormationManager.Instance != null && HeroFormationManager.Instance.IsHeroInFormation(entry.HeroId);

            slotUI.SetFormationState(isInFormation);
            slotUI.SetDragEnabled(false);

            Button slotButton = slotUI.GetComponent<Button>();
            if (slotButton == null)
            {
                slotButton = slotUI.gameObject.AddComponent<Button>();
            }

            slotButton.onClick.RemoveAllListeners();
            HeroNameEnum targetHeroId = entry.HeroId;
            slotButton.onClick.AddListener(() => OnHeroClickedFromScroll(targetHeroId));
        }
    }

    private List<(HeroEntry entry, HeroSaveData heroSaveData)> GetOwnedHeroes()
    {
        List<(HeroEntry entry, HeroSaveData heroSaveData)> ownedHeroes = new();

        foreach (HeroEntry entry in playerInfo.HeroEntries)
        {
            if (entry == null) continue;

            if (playerInfo.TryGetHeroData(entry.HeroId, out HeroSaveData heroSaveData) && heroSaveData.IsOwned)
            {
                ownedHeroes.Add((entry, heroSaveData));
            }
        }
        return ownedHeroes;
    }

    private bool PrepareHeroSlotPool(int requiredCount)
    {
        if (heroSlotPrefab == null || heroListContent == null) return false;

        // 필요한 슬롯 수만큼 풀을 준비
        while (heroSlotPool.Count < requiredCount)
        {
            GameObject newSlot = Instantiate(heroSlotPrefab, heroListContent);
            HeroSlotUI slotUI = newSlot.GetComponent<HeroSlotUI>();
            if (slotUI == null)
            {
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
