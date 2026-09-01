
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class OfflineRewardUIController : MonoBehaviour
{
    [Header("오프라인 시간 세팅")]
    [SerializeField] private TMP_Text offlineTimeText;
    [SerializeField] private RectTransform timeTextRect;
    [SerializeField] private RectTransform timeIconRect;
    [SerializeField] private float iconSpacing = 8f;
    [SerializeField] private InventorySlot rewardSlotPrefab;

    [Header("오프라인 보상 스크롤")]
    [SerializeField] private Transform rewardContentParent;
    [SerializeField] private CurrencyTable currencyTable;
    [SerializeField] private ItemDBSO itemDBSO;
    [SerializeField] private float slotSpacing = 15f;

    private OfflineReward offlineRewards;

    private readonly List<InventorySlot> rewardSlots = new();


    private void Awake()
    {
        if (!HasOfflineRewards())
        {
            gameObject.SetActive(false);
            return;
        }

        SetTimeText();
        ArrangeIconAndText();
        ShowOfflineRewards();
    }

    private bool HasOfflineRewards()
    {
        offlineRewards = OfflineRewardProvider.OfflineRewards;

        if (offlineRewards.CurrencyRewards != null)
        {
            for (int i = 0; i < offlineRewards.CurrencyRewards.Length; i++)
            {
                if (offlineRewards.CurrencyRewards[i].Amount > 0)
                {
                    return true;
                }
            }
        }

        if (offlineRewards.ItemRewards != null)
        {
            for (int i = 0; i < offlineRewards.ItemRewards.Length; i++)
            {
                if (offlineRewards.ItemRewards[i].Amount > 0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void SetTimeText()
    {
        int offlineMinutes = OfflineRewardProvider.OfflineMinutes;

        int hours = offlineMinutes / 60;
        int minutes = offlineMinutes % 60;

        if (offlineMinutes == OfflineRewardProvider.MaxOfflineMinutes)
        {
            offlineTimeText.text = $"{hours}시간 (Max)";
            return;
        }

        if (hours > 0)
        {
            offlineTimeText.text = $"{hours}시간 {minutes}분";
        }
        else
        {
            offlineTimeText.text = $"{minutes}분";
        }
    }

    private void ArrangeIconAndText()
    {
        offlineTimeText.ForceMeshUpdate();

        float textWidth = offlineTimeText.preferredWidth;
        timeTextRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, textWidth);

        float textLeftX = timeTextRect.anchoredPosition.x - textWidth * timeTextRect.pivot.x;
        float iconHalfWidth = timeIconRect.rect.width * timeIconRect.pivot.x;

        timeIconRect.anchoredPosition = new Vector2(
            textLeftX - iconSpacing - iconHalfWidth,
            timeIconRect.anchoredPosition.y
        );
    }

    private void ShowOfflineRewards()
    {
        OfflineReward offlineRewards = OfflineRewardProvider.OfflineRewards;

        AddCurrencyRewardSlots(offlineRewards.CurrencyRewards);
        AddItemRewardSlots(offlineRewards.ItemRewards);

        ArrangeRewardSlots();
    }


    private void AddCurrencyRewardSlots(CurrencyReward[] currencyRewards)
    {
        if (currencyRewards == null) return;
        foreach (var reward in currencyRewards)
        {
            if (reward.Amount <= 0) continue;
            CurrencySO currency = currencyTable.GetCurrency(reward.Type);
            InventorySlot slot = Instantiate(rewardSlotPrefab, rewardContentParent);
            Sprite icon = currency != null ? currency.CurrencyIcon : null;
            slot.SetItem(icon, reward.Amount);
            rewardSlots.Add(slot);
        }
    }

    private void AddItemRewardSlots(ItemAmount[] itemRewards)
    {
        if (itemRewards == null) return;
        foreach (var reward in itemRewards)
        {
            if (reward.Amount <= 0) continue;
            ItemSO item = itemDBSO.GetItemById(reward.ItemId);
            InventorySlot slot = Instantiate(rewardSlotPrefab, rewardContentParent);
            Sprite icon = item != null ? item.ItemIcon : null;
            slot.SetItem(icon, reward.Amount);
            rewardSlots.Add(slot);
        }
    }

    private void ArrangeRewardSlots()
    {
        int slotCount = rewardSlots.Count;
        if (slotCount <= 0) return;

        RectTransform slotRect = rewardSlots[0].GetComponent<RectTransform>();
        Vector2 slotSize = slotRect.rect.size;
        float totalWidth = slotCount * slotSize.x + (slotCount - 1) * slotSpacing;

        float startX = -totalWidth * 0.5f + slotSize.x * 0.5f;

        for (int i = 0; i < slotCount; i++)
        {
            RectTransform rect = rewardSlots[i].GetComponent<RectTransform>();

            if (rect == null) continue;

            float x = startX + i * (slotSize.x + slotSpacing);
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(x, 0f);
        }
    }
}
