using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "Game/Item/ItemSO")]
public class ItemSO : ScriptableObject
{
    [SerializeField] private ItemType itemType;
    [SerializeField] private int itemId;
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemIcon;

    [TextArea]
    [SerializeField] private string itemDescription;
    public ItemType ItemType => itemType;
    public int ItemId => itemId;    
    public string ItemName => itemName;
    public Sprite ItemIcon => itemIcon;
    public string ItemDescription => itemDescription;

    private void OnValidate()
    {
        if (!ItemIdRules.IsValidItemId(itemId))
        {
            Debug.LogError($"잘못된 아이템 ID입니다. ({itemId}) 아이템 ID는 {ItemIdRules.ItemIdMin} ~ {ItemIdRules.ItemIdMax} 사이에서 설정해야 합니다.");
        }

        if (itemType == ItemType.Material && !ItemIdRules.IsMaterialId(itemId))
        {
            Debug.LogError($"잘못된 재료 아이템 ID 입니다. ({itemId}) 재료 아이템 ID는 {ItemIdRules.MaterialMin} ~ {ItemIdRules.MaterialMax} 사이에서 설정해야 합니다.");
        }
    }
}

