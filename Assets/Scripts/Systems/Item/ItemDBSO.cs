using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDBSO", menuName = "Game/Item/ItemDB")]
public class ItemDBSO : ScriptableObject
{
    [SerializeField] private List<ItemSO> items;

    public IReadOnlyList<ItemSO> Items => items;

    private Dictionary<int, ItemSO> itemById;

    private void OnEnable()
    {
        BuildItemDict();
    }

    public bool ContainsItemId(int itemId)
    {
        if (itemById == null)
        {
            BuildItemDict();
        }

        return itemById.ContainsKey(itemId);
    }

    public ItemSO GetItemById(int itemId)
    {
        if (itemById == null)
        {
            BuildItemDict();
        }

        if (itemById.TryGetValue(itemId, out ItemSO item))
        {
            return item;
        }

        Debug.LogError($"아이템 ID {itemId}에 해당하는 아이템이 존재하지 않습니다.", this);
        return null;
    }

    private void BuildItemDict()
    {
        itemById = new Dictionary<int, ItemSO>();

        if (items == null)
            return;

        foreach (ItemSO item in items)
        {
            if (item == null)
            {
                Debug.LogError("ItemDBSO에 null 아이템이 포함되어 있습니다.", this);
                continue;
            }

            if (itemById.ContainsKey(item.ItemId))
            {
                Debug.LogError($"중복된 아이템 ID가 발견되었습니다: {item.ItemId} ({item.ItemName})", this);
                continue;
            }

            itemById.Add(item.ItemId, item);
        }
    }
}
