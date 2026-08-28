using UnityEngine;
using System.Collections.Generic;

public class InventoryUIController : MonoBehaviour
{
    [SerializeField] private ItemDBSO itemDB;
    [SerializeField] RectTransform contentRect;
    [SerializeField] InventorySlot inventorySlotPrefab;

    [Header("슬롯 크기 및 간격 설정")]
    [SerializeField, Min(1)] private int columnCount = 4;
    [SerializeField, Min(0)] private int prewarmSlotCount = 0;
    [SerializeField] private Vector2 cellSize = new Vector2(100f, 100f);
    [SerializeField] private Vector2 spacing = new Vector2(12f, 12f);
    [SerializeField] private Vector2 padding = new Vector2(16f, 16f);

    private List<InventorySlot> inventorySlots = new();
    private List<ItemStruct> inventoryItems = new();
    private struct ItemStruct
    {
        public int itemId;
        public ItemStackSaveData itemStack;

        public ItemStruct(int itemId, ItemStackSaveData itemStack)
        {
            this.itemId = itemId;
            this.itemStack = itemStack;
        }
    }

    private void OnEnable()
    {
        PlayerInfo.Instance.OnItemAmountChanged -= RefreshInventory;
        PlayerInfo.Instance.OnItemAmountChanged += RefreshInventory;

        RefreshInventory();
    }

    private void OnDisable()
    {
        PlayerInfo.Instance.OnItemAmountChanged -= RefreshInventory;
    }

    private void RefreshInventory()
    {
        inventoryItems.Clear();

        Dictionary<int, ItemStackSaveData> items = PlayerInfo.Instance.SaveData.Inventory.Items;

        foreach ((int itemId, ItemStackSaveData itemStack) in items)
        {
            if (itemStack == null) continue;
            if (itemStack.Amount <= 0) continue;

            ItemSO itemSO = itemDB.GetItemById(itemId);
            if (itemSO == null) continue;
            if (itemSO.ItemType != ItemType.Material) continue;
            
            inventoryItems.Add(new ItemStruct(itemId, itemStack));
        }

        int displaySlotCount = Mathf.Max(inventoryItems.Count, prewarmSlotCount);
        ResizeContent(displaySlotCount);


        for (int i = 0; i < displaySlotCount; i++)
        {
            InventorySlot slot = GetOrCreateSlot(i);

            if (slot == null) continue;

            slot.gameObject.SetActive(true);
            SetSlotPosition(slot, i);

            if (i < inventoryItems.Count)
            {
                ItemStruct item = inventoryItems[i];
                ItemSO itemSO = itemDB.GetItemById(item.itemId);

                if (itemSO != null)
                {
                    slot.SetItem(itemSO.ItemIcon, item.itemStack.Amount);
                }
                else
                {
                    slot.ClearSlot();
                }
            }
            else
            {
                slot.ClearSlot();
            }
        }

        for (int i = displaySlotCount; i < inventorySlots.Count; i++)
        {
            inventorySlots[i].ClearSlot();
            inventorySlots[i].gameObject.SetActive(false);
        }
    }

    private InventorySlot GetOrCreateSlot(int index)
    {
        if (index < 0) return null;

        if (index < inventorySlots.Count)
        {
            return inventorySlots[index];
        }
        else
        {
            InventorySlot newSlot = Instantiate(inventorySlotPrefab, contentRect);
            inventorySlots.Add(newSlot);
            return newSlot;
        }
    }

    private void ResizeContent(int displaySlotCount)
    {
        if (contentRect == null) return;

        int rowCount = Mathf.CeilToInt((float)displaySlotCount / columnCount);

        float height = padding.y * 2
            + cellSize.y * rowCount
            + Mathf.Max(0, rowCount - 1) * spacing.y;

        RectTransform viewportRect = contentRect.parent as RectTransform;

        if (viewportRect != null)
        {
            float viewportHeight = viewportRect.rect.height - Mathf.Abs(contentRect.anchoredPosition.y);
            height = Mathf.Max(height, viewportHeight);
        }

        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, height);
    }

    private void SetSlotPosition(InventorySlot slot, int index)
    {
        if (slot == null) return;

        slot.TryGetComponent<RectTransform>(out RectTransform slotRect);
        if (slotRect == null) return;

        int row = index / columnCount;
        int column = index % columnCount;

        float x = padding.x + (cellSize.x + spacing.x) * column;
        float y = -padding.y - (cellSize.y + spacing.y) * row;

        slotRect.anchorMin = new Vector2(0, 1);
        slotRect.anchorMax = new Vector2(0, 1);
        slotRect.pivot = new Vector2(0, 1);
        slotRect.sizeDelta = cellSize;
        slotRect.anchoredPosition = new Vector2(x, y);
    }
}
