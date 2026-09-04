using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ItemLogUIController : MonoBehaviour
{
    [Header("패널")]
    [SerializeField] private GameObject panelRoot;

    [Header("버튼")]
    [SerializeField] private GameObject ItemLogBtn;

    [Header("획득 목록")]
    [SerializeField] private RectTransform contentRect;
    [SerializeField] private InventorySlot slotPrefab;
    [SerializeField] private CurrencyTable currencyTable;
    [SerializeField] private ItemDBSO itemDBSO;

    [Header("슬롯 배치")]
    [SerializeField] private Vector2 slotSize = new Vector2(64f, 64f);
    [SerializeField] private Vector2 startPadding = new Vector2(16f, 0f);
    [SerializeField] private float slotSpacing = 12f;

    [Header("진행 정보")]
    [SerializeField] private TMP_Text playTimeText;
    [SerializeField] private TMP_Text killCountText;

    private readonly List<InventorySlot> rewardSlots = new();
    // 획득 보상을 순서대로 기록하기 위한 리스트
    private readonly List<LogRewardKey> rewardOrder = new();

    // 획득 수량은 dictionary로 관리하여 빠르게 조회할 수 있도록 한다.
    private readonly Dictionary<LogRewardKey, long> rewardAmounts = new();

    private float playTime;
    private int killCount;

    private enum LogRewardType
    {
        Currency,
        Item
    }
    private struct LogRewardKey
    {
        public LogRewardType Type;
        public int Id;

        public LogRewardKey(LogRewardType type, int id)
        {
            Type = type;
            Id = id;
        }
    }

    private void Awake()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }
    }

    private void Update()
    {
        playTime += Time.deltaTime;

        if (panelRoot != null && panelRoot.activeSelf)
        {
            RefreshPlayTimeText();
        }
    }

    public void ShowPanel()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(true);
        }

        if (ItemLogBtn != null)
        {
            ItemLogBtn.SetActive(false);
        }

        RefreshPlayTimeText();
        RefreshKillCountText();
        RefreshRewardSlots();
    }
    public void HidePanel()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }

        if (ItemLogBtn != null)
        {
            ItemLogBtn.SetActive(true);
        }
    }

    public void AddKillCount()
    {
        killCount++;

        if (panelRoot != null && panelRoot.activeSelf)
        {
            RefreshKillCountText();
        }
    }

    // 적을 처치할 때 일부 재화나 아이템은 확률적으로 드랍될 수 있다.
    // 이 경우 AddRewards를 호출하여 획득한 재화와 아이템을 기록하고
    // 내부에서는 수량이 0보다 큰 경우에만 UI에 표시하도록 한다.
    public void AddRewards(CurrencyReward[] currencyRewards, List<ItemAmount> itemRewards)
    {
        AddCurrencyRewards(currencyRewards);
        AddItemRewards(itemRewards);

        if (panelRoot != null && panelRoot.activeSelf)
        {
            RefreshRewardSlots();
        }
    }

    // 재화만 지급되는 경우에 호출하는 메서드
    public void AddCurrencyReward(CurrencyType currencyType, long amount)
    {
        if (amount <= 0)
        {
            return;
        }

        AddRewardAmount(
            new LogRewardKey(LogRewardType.Currency, (int)currencyType),
            amount
        );

        if (panelRoot != null && panelRoot.activeSelf)
        {
            RefreshRewardSlots();
        }
    }

    // 아이템만 지급되는 경우에 호출하는 메서드
    public void AddItemReward(int itemId, int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        AddRewardAmount(
            new LogRewardKey(LogRewardType.Item, itemId),
            amount
        );

        if (panelRoot != null && panelRoot.activeSelf)
        {
            RefreshRewardSlots();
        }
    }

    // 기록 요청 받은 재화 보상을 내부에서 확인하고, 수량이 0보다 큰 경우에만 rewardAmounts에 추가.
    private void AddCurrencyRewards(CurrencyReward[] currencyRewards)
    {
        if (currencyRewards == null)
        {
            return;
        }

        for (int i = 0; i < currencyRewards.Length; i++)
        {
            CurrencyReward reward = currencyRewards[i];

            if (reward.Amount <= 0)
            {
                continue;
            }

            AddRewardAmount(
                new LogRewardKey(LogRewardType.Currency, (int)reward.Type),
                reward.Amount
            );
        }
    }

    // 기록 요청 받은 아이템 보상을 내부에서 확인하고, 수량이 0보다 큰 경우에만 rewardAmounts에 추가.
    private void AddItemRewards(List<ItemAmount> itemRewards)
    {
        if (itemRewards == null)
        {
            return;
        }

        for (int i = 0; i < itemRewards.Count; i++)
        {
            ItemAmount reward = itemRewards[i];

            if (reward.Amount <= 0)
            {
                continue;
            }

            AddRewardAmount(
                new LogRewardKey(LogRewardType.Item, reward.ItemId),
                reward.Amount
            );
        }
    }

    // rewardAmounts에 보상 수량을 추가하는 내부 메서드
    private void AddRewardAmount(LogRewardKey key, long amount)
    {
        // 딕셔너리에 해당 키가 없을 때 order 리스트에 추가하고, 수량을 기록한다.
        if (rewardAmounts.TryAdd(key, amount))
        {
            rewardOrder.Add(key);
            return;
        }

        // 이미 존재하는 키라면 수량을 누적한다.
        rewardAmounts[key] += amount;
    }

    private void RefreshRewardSlots()
    {
        for (int i = 0; i < rewardOrder.Count; i++)
        {
            LogRewardKey key = rewardOrder[i];
            long amount = rewardAmounts[key];

            InventorySlot slot = GetOrCreateSlot(i);
            slot.gameObject.SetActive(true);
            SetSlotPosition(slot, i);
            Sprite icon = GetRewardIcon(key);
            slot.SetItem(icon, amount);
        }

        for (int i = rewardOrder.Count; i < rewardSlots.Count; i++)
        {
            rewardSlots[i].ClearSlot();
            rewardSlots[i].gameObject.SetActive(false);
        }

        ResizeContent();
    }

    private InventorySlot GetOrCreateSlot(int index)
    {
        while (rewardSlots.Count <= index)
        {
            InventorySlot slot = Instantiate(slotPrefab, contentRect);
            rewardSlots.Add(slot);
        }

        return rewardSlots[index];
    }

    private Sprite GetRewardIcon(LogRewardKey key)
    {
        switch (key.Type)
        {
            case LogRewardType.Currency:
                CurrencySO currency = 
                    currencyTable != null 
                    ? currencyTable.GetCurrency((CurrencyType)key.Id) 
                    : null;
                return currency != null ? currency.CurrencyIcon : null;

            case LogRewardType.Item:
                ItemSO item =
                    itemDBSO != null
                    ? itemDBSO.GetItemById(key.Id)
                    : null;
                return item != null ? item.ItemIcon : null;
        }

        return null;
    }

    private void SetSlotPosition(InventorySlot slot, int index)
    {
        if (slot == null) return;

        RectTransform slotRect = slot.GetComponent<RectTransform>();

        if (slotRect == null) return;

        slotRect.anchorMin = new Vector2(0f, 0.5f);
        slotRect.anchorMax = new Vector2(0f, 0.5f);
        slotRect.pivot = new Vector2(0f, 0.5f);

        slotRect.sizeDelta = slotSize;

        float x = startPadding.x + index * (slotSize.x + slotSpacing);
        slotRect.anchoredPosition = new Vector2(x, startPadding.y);
    }

    private void ResizeContent()
    {
        if (contentRect == null) return;

        int activeSlot = rewardOrder.Count;

        if (activeSlot <= 0)
        {
            contentRect.sizeDelta = new Vector2(0f, contentRect.sizeDelta.y);
            return;
        }

        float width = startPadding.x * 2f + activeSlot * slotSize.x + (activeSlot - 1) * slotSpacing;

        contentRect.sizeDelta = new Vector2(width, contentRect.sizeDelta.y);
    }

    private void RefreshPlayTimeText()
    {
        if (playTimeText == null) return;

        int totalSeconds = Mathf.FloorToInt(playTime);

        playTimeText.text = FormatPlayTime(totalSeconds);
    }

    private string FormatPlayTime(float seconds)
    {
        int totalSeconds = Mathf.FloorToInt(seconds);

        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int remainSeconds = totalSeconds % 60;

        if (hours >= 100)
        {
            return "99시간+";
        }

        if (hours > 0)
        {
            return $"{hours}시간 {minutes}분";
        }

        if (minutes > 0)
        {
            return $"{minutes}분 {remainSeconds}초";
        }

        return $"{remainSeconds}초";
    }

    private void RefreshKillCountText()
    {
        if (killCountText == null)
        {
            return;
        }

        killCountText.text = FormatKillCount(killCount);
    }

    private string FormatKillCount(int count)
    {
        if (count >= 100000000)
        {
            return "99999999+마리";
        }

        return $"{count}마리";
    }
}

