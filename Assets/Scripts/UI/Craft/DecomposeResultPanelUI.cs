using System.Collections.Generic;
using UnityEngine;

public class DecomposeResultPanelUI : MonoBehaviour
{
    [SerializeField] private ItemDBSO itemDB;
    [SerializeField] private DecomposeResultSlotUI[] resultSlots;
    [SerializeField] private float resultSlotSpacing = 120f;

    public void Show(Dictionary<int, int> rewards)
    {
        gameObject.SetActive(true);
        ClearSlots();

        if (rewards == null || rewards.Count == 0)
        {
            ArrangeSlots(0);
            return;
        }

        int index = 0;

        foreach ((int itemId, int amount) in rewards)
        {
            if (resultSlots == null || index >= resultSlots.Length)
            {
                break;
            }

            DecomposeResultSlotUI slot = resultSlots[index];

            if (slot == null)
            {
                index++;
                continue;
            }

            ItemSO item = itemDB != null ? itemDB.GetItemById(itemId) : null;
            Sprite icon = item != null ? item.ItemIcon : null;

            slot.SetSlot(icon, amount);
            index++;
        }

        ArrangeSlots(index);
    }

    public void Hide()
    {
        ClearSlots();
        gameObject.SetActive(false);
    }

    private void ClearSlots()
    {
        if (resultSlots == null)
        {
            return;
        }

        foreach (DecomposeResultSlotUI slot in resultSlots)
        {
            if (slot != null)
            {
                slot.ClearSlot();
            }
        }
    }

    private void ArrangeSlots(int activeCount)
    {
        if (resultSlots == null || activeCount <= 0)
        {
            return;
        }

        float startX = -resultSlotSpacing * (activeCount - 1) * 0.5f;

        for (int i = 0; i < activeCount; i++)
        {
            float x = startX + resultSlotSpacing * i;
            SetSlotX(i, x);
        }
    }

    private void SetSlotX(int index, float x)
    {
        if (resultSlots == null || index < 0 || index >= resultSlots.Length)
        {
            return;
        }

        if (resultSlots[index] == null)
        {
            return;
        }

        RectTransform rectTransform = resultSlots[index].GetComponent<RectTransform>();

        if (rectTransform == null)
        {
            return;
        }

        rectTransform.anchoredPosition = new Vector2(x, rectTransform.anchoredPosition.y);
    }
}
