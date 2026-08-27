using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EquimentInventoryController : MonoBehaviour
{
    private enum EquipmentInventoryMode
    {
        Equip,
        Decompose
    }

    [Header("장비 데이터")]
    [SerializeField] private EquipmentDB equipmentDB;

    [Header("히어로 선택 관련 UI")]
    [SerializeField] private Transform heroSlotRoot;
    [SerializeField] private HeroSlotUI heroSlotPrefab;
    [SerializeField] private Image selectedHeroIcon;

    [Header("장비 슬롯 설정")]
    [SerializeField] private HeroEquipmentSlot weaponSlot;
    [SerializeField] private HeroEquipmentSlot bodySlot;
    [SerializeField] private HeroEquipmentSlot accSlot;

    [Header("인벤토리 슬롯 설정")]
    [SerializeField] private RectTransform contentRect;
    [SerializeField] private HeroEquipmentSlot slotPrefab;

    [Header("인벤토리 레이아웃")]
    [SerializeField, Min(1)] private int columnCount = 4;
    [SerializeField] private Vector2 cellSize = new Vector2(100f, 100f);
    [SerializeField] private Vector2 spacing = new Vector2(12f, 12f);
    [SerializeField] private Vector2 padding = new Vector2(16f, 16f);

    [Header("분해 UI")]
    [SerializeField] private EquipmentDecomposeUIController decomposeUIController;

    private EquipmentInventoryMode currentMode = EquipmentInventoryMode.Equip;

    [Header("성능 비교용 플래그")]
    [SerializeField] private bool useLoopCheckForEquipped = false;

    // 영웅 슬롯과 장비 슬롯 캐싱을 위한 리스트
    private readonly List<HeroSlotUI> heroSlots = new();
    private readonly List<HeroEquipmentSlot> inventorySlots = new();

    // 장착하지 않은 장비만 인벤토리 슬롯에 표시하기 위한 리스트
    private readonly List<EquipmentSaveData> visibleEquipments = new();

    private readonly HashSet<int> equippedEquipmentIds = new();



    private HeroEntry selectedHeroEntry;
    private HeroSaveData selectedHeroSaveData;

    private void OnEnable()
    {
        PlayerInfo.Instance.OnEquipmentInventoryChanged -= OnEquipmentInventoryChanged;
        PlayerInfo.Instance.OnEquipmentInventoryChanged += OnEquipmentInventoryChanged;

        InitializeEquippedSlots();
        RefreshHeroSlots();
        ClearSelectedHero();
        RefreshInventorySlots();
    }

    private void OnDisable()
    {
        PlayerInfo.Instance.OnEquipmentInventoryChanged -= OnEquipmentInventoryChanged;
    }

    public void SetEquipMode()
    {
        currentMode = EquipmentInventoryMode.Equip;

        if (decomposeUIController != null)
        {
            decomposeUIController.ClearSelection();
        }
    }

    public void SetDecomposeMode()
    {
        currentMode = EquipmentInventoryMode.Decompose;
    }


    private void InitializeEquippedSlots()
    {
        if (weaponSlot != null)
        {
            weaponSlot.SetClickAction(OnEquippedSlotClicked);
        }

        if (bodySlot != null)
        {
            bodySlot.SetClickAction(OnEquippedSlotClicked);
        }

        if (accSlot != null)
        {
            accSlot.SetClickAction(OnEquippedSlotClicked);
        }
    }

    private void RefreshHeroSlots()
    {
        IReadOnlyList<HeroEntry> heroEntries = PlayerInfo.Instance.HeroEntries;

        int slotIndex = 0;

        for (int i = 0; i < heroEntries.Count; i++)
        {
            HeroEntry heroEntry = heroEntries[i];

            if (heroEntry == null) continue;
            if (!PlayerInfo.Instance.TryGetHeroData(heroEntry.HeroId, out HeroSaveData heroSaveData)) continue;
            if (heroSaveData == null || !heroSaveData.IsOwned) continue;

            HeroSlotUI heroSlot = GetOrCreateHeroSlot(slotIndex);
            heroSlot.gameObject.SetActive(true);
            heroSlot.SetDragEnabled(false);
            heroSlot.SetupSlot(heroEntry, heroSaveData, true, OnHeroSlotClicked);

            slotIndex++;
        }

        // 캐싱한 슬롯이 오히려 
        for (int i = slotIndex; i < heroSlots.Count; i++)
        {
            heroSlots[i].gameObject.SetActive(false);
        }
    }

    private HeroSlotUI GetOrCreateHeroSlot(int index)
    {
        while (heroSlots.Count <= index)
        {
            HeroSlotUI newSlot = Instantiate(heroSlotPrefab, heroSlotRoot);
            heroSlots.Add(newSlot);
        }

        return heroSlots[index];
    }

    private void ClearSelectedHero()
    {
        selectedHeroEntry = null;
        selectedHeroSaveData = null;

        RefreshSelectedHeroIcon();
        RefreshStatView();
        RefreshEquippedSlots();
        RefreshInventorySlots();
    }

    private void RefreshInventorySlots()
    {
        if (!useLoopCheckForEquipped) RebuildEquippedEquipmentIdSet();

        visibleEquipments.Clear();

        IReadOnlyList<EquipmentSaveData> ownedEquiments = PlayerInfo.Instance.GetOwnedEquipments();

        for (int i = 0; i < ownedEquiments.Count; i++)
        {
            EquipmentSaveData equipData = ownedEquiments[i];
            if (equipData == null) continue;
            if (!IsEquippedByAnyHero(equipData.EquipId))
            {
                visibleEquipments.Add(equipData);
            }
        }

        ResizeContent(visibleEquipments.Count);

        for (int i = 0; i < visibleEquipments.Count; i++)
        {
            EquipmentSaveData equipData = visibleEquipments[i];
            
            if (equipmentDB == null || equipData == null) 
                continue;

            EquipmentSO equipmentSO = equipmentDB.GetEquipmentSO(equipData.EquipDataId);
            HeroEquipmentSlot slot = GetOrCreateInventorySlot(i);
            slot.gameObject.SetActive(true);
            SetSlotPosition(slot, i);
            slot.SetSlot(equipData, equipmentSO, false);
            slot.SetClickAction(OnInventorySlotClicked);
        }

        for (int i = visibleEquipments.Count; i < inventorySlots.Count; i++)
        {
            inventorySlots[i].ClearSlot();
            inventorySlots[i].gameObject.SetActive(false);
        }
    }

    private void OnEquipmentInventoryChanged()
    {
        RefreshInventorySlots();
    }

    private HeroEquipmentSlot GetOrCreateInventorySlot(int index)
    {
        while (inventorySlots.Count <= index)
        {
            HeroEquipmentSlot newSlot = Instantiate(slotPrefab, contentRect);
            inventorySlots.Add(newSlot);
        }
        return inventorySlots[index];
    }

    private bool IsEquippedByAnyHero(int equipId)
    {
        if (useLoopCheckForEquipped)
        {
            return IsEquippedByAnyHeroLoop(equipId);
        }
        else
        {
            return IsEquippedByAnyHeroCached(equipId);
        }
    }

    private void SetSlotPosition(HeroEquipmentSlot slot, int index)
    {
        if (slot == null) return;

        RectTransform slotRect = slot.GetComponent<RectTransform>();

        if (slotRect == null) return;

        int row = index / columnCount;
        int column = index % columnCount;

        // 왼쪽 패딩과 column 간격을 고려하여 x 좌표를 계산
        float x = padding.x + (cellSize.x + spacing.x) * column;

        // 위쪽 패딩과 row 간격을 고려하여 y 좌표를 계산. 이때 UI 좌표계는 위쪽이 0이므로 음수로 계산한다.
        float y = -padding.y - (cellSize.y + spacing.y) * row;

        // content와 슬롯의 기준을 왼쪽 위로 맞추기 위해 anchor와 pivot을 (0, 1)로 설정하고, sizeDelta와 anchoredPosition을 적용한다.
        slotRect.anchorMin = new Vector2(0, 1);
        slotRect.anchorMax = new Vector2(0, 1);
        slotRect.pivot = new Vector2(0, 1);
        slotRect.sizeDelta = cellSize;
        slotRect.anchoredPosition = new Vector2(x, y);
    }

    private void ResizeContent(int itemCount)
    {
        if (contentRect == null) return;

        // 장비 슬롯을 배치할 때 필요한 행의 수를 계산한다.
        int rowCount = Mathf.CeilToInt((float)itemCount / columnCount);

        // contentRect의 높이를 계산한다. 위 아래의 패딩과 슬롯 크기, 행 간격을 고려하여 계산한다.
        float height = padding.y * 2 
            + (cellSize.y * rowCount) 
            + (Mathf.Max(0, rowCount - 1) * spacing.y);

        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, height);
    }

    

    // 영웅 데이터를 순회하면서 해당 장비가 장착되어 있는지 확인하는 메서드
    private bool IsEquippedByAnyHeroLoop(int equipId)
    {

        if (equipId <= 0)
        {
            return false;
        }

        foreach (KeyValuePair<HeroNameEnum, HeroSaveData> pair in PlayerInfo.Instance.Heroes)
        {
            HeroSaveData heroData = pair.Value;

            if (heroData == null)
            {
                continue;
            }

            if (heroData.EquippedWeaponId == equipId)
            {
                return true;
            }

            if (heroData.EquippedBodyId == equipId)
            {
                return true;
            }

            if (heroData.EquippedAccId == equipId)
            {
                return true;
            }
        }

        return false;
    }

    // 인벤토리 갱신 때마다 hashset을 재구성하여 장착 여부를 캐싱한다.
    private void RebuildEquippedEquipmentIdSet()
    {
        equippedEquipmentIds.Clear();

        foreach (KeyValuePair<HeroNameEnum, HeroSaveData> pair in PlayerInfo.Instance.Heroes)
        {
            HeroSaveData heroData = pair.Value;

            if (heroData == null)
            {
                continue;
            }

            AddEquippedId(heroData.EquippedWeaponId);
            AddEquippedId(heroData.EquippedBodyId);
            AddEquippedId(heroData.EquippedAccId);
        }
    }

    private void AddEquippedId(int equipId)
    {
        if (equipId > 0)
        {
            equippedEquipmentIds.Add(equipId);
        }
    }

    private bool IsEquippedByAnyHeroCached(int equipId)
    {
        return equipId > 0 && equippedEquipmentIds.Contains(equipId);
    }

    private void RefreshEquippedSlots()
    {
        if (selectedHeroEntry == null || selectedHeroSaveData == null || equipmentDB == null)
        {
            ClearEquippedSlots();
            return;
        }

        PlayerInfo.Instance.GetHeroEquippedEquipments(selectedHeroEntry.HeroId, 
            out EquipmentSaveData weaponData, 
            out EquipmentSaveData bodyData, 
            out EquipmentSaveData accData);

        EquipmentSO weaponSO = (weaponData != null) ? equipmentDB.GetEquipmentSO(weaponData.EquipDataId) : null;
        EquipmentSO bodySO = (bodyData != null) ? equipmentDB.GetEquipmentSO(bodyData.EquipDataId) : null;
        EquipmentSO accSO = (accData != null) ? equipmentDB.GetEquipmentSO(accData.EquipDataId) : null;

        SetEquippedSlot(weaponSlot, weaponData, weaponSO);
        SetEquippedSlot(bodySlot, bodyData, bodySO);
        SetEquippedSlot(accSlot, accData, accSO);
    }

    private void SetEquippedSlot(HeroEquipmentSlot slot, EquipmentSaveData saveData, EquipmentSO equipmentSO)
    {
        if (slot == null)
        {
            return;
        }

        slot.SetSlot(saveData, equipmentSO, saveData != null);
    }

    private void ClearEquippedSlots()
    {
        if (weaponSlot != null)
        {
            weaponSlot.ClearSlot();
        }

        if (bodySlot != null)
        {
            bodySlot.ClearSlot();
        }

        if (accSlot != null)
        {
            accSlot.ClearSlot();
        }
    }

    private void RefreshStatView()
    {
    }
    private void RefreshSelectedHeroIcon()
    {
        if (selectedHeroIcon == null)
        {
            return;
        }

        if (selectedHeroEntry == null)
        {
            selectedHeroIcon.sprite = null;
            selectedHeroIcon.gameObject.SetActive(false);
            return;
        }

        selectedHeroIcon.sprite = selectedHeroEntry.HeroIcon;
        selectedHeroIcon.gameObject.SetActive(true);
    }

    // 장착된 장비 슬롯을 클릭하면 해당 장비를 해제하도록 처리한다.
    private void OnEquippedSlotClicked(HeroEquipmentSlot slot)
    { 
        if (currentMode == EquipmentInventoryMode.Decompose)
        {
            return;
        }

        if (slot == null) return;
        if (slot.EquipmentSaveData == null) return;

        bool result = PlayerInfo.Instance.ClearHeroEquippedEquipmentId(selectedHeroEntry.HeroId, slot.EquipPart);

        if (!result) return;

        RefreshEquippedSlots();
        RefreshInventorySlots();
        RefreshStatView();
    }

    private void OnHeroSlotClicked(HeroEntry entry, HeroSaveData saveData)
    {
        if (entry == null || saveData == null) return;
        selectedHeroEntry = entry;
        selectedHeroSaveData = saveData;

        RefreshSelectedHeroIcon();
        RefreshEquippedSlots();
        RefreshInventorySlots();
        RefreshStatView();
    }

    private void OnInventorySlotClicked(HeroEquipmentSlot slot)
    {
        if (slot == null) return;
        
        EquipmentSaveData equipData = slot.EquipmentSaveData;

        if (equipData == null) return;

        if (currentMode == EquipmentInventoryMode.Decompose)
        {
            if (decomposeUIController == null)
            {
                Debug.LogWarning("분해 UI 컨트롤러가 연결되지 않았습니다.");
                return;
            }

            decomposeUIController.ToggleEquipment(equipData);
            return;
        }

        if (selectedHeroEntry == null || selectedHeroSaveData == null) return;

        bool result = PlayerInfo.Instance.SetHeroEquippedEquipmentId(selectedHeroEntry.HeroId, equipData.EquipPart, equipData.EquipId);

        if (!result) return;

        RefreshEquippedSlots();
        RefreshInventorySlots();
        RefreshStatView();
    }
}
