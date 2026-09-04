using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("인벤토리 세팅")]
    public Transform slotContainer;
    public GameObject slotPrefab;       // 프리팹 연결
    public int inventoryCapacity = 16;  // 인벤토리 칸 수

    private List<InventorySlot> slots = new List<InventorySlot>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        // 게임 시작 시 지정한 칸수만큼 슬롯을 생성
        for (int i = 0; i < inventoryCapacity; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab, slotContainer);
            InventorySlot slotComponent = newSlot.GetComponent<InventorySlot>();
            slotComponent.ClearSlot();
            slots.Add(slotComponent);
        }
    }
    // 외부에서 아이템을 획득했을 때 호출할 수 있는 함수
    //public bool AddItem(Sprite itemSprite)
    //{
    //    foreach (var slot in slots)
    //    {
    //        // 비어있는 슬롯을 찾아 아이템을 채워 넣음
    //        if (slot.iconImage.sprite == null || !slot.iconImage.gameObject.activeSelf)
    //        {
    //            slot.SetItem(itemSprite);
    //            return true; 
    //        }
    //    }
    //    return false;
    //}
}
