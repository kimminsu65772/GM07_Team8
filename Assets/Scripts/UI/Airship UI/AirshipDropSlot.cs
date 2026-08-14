using UnityEngine;
using UnityEngine.EventSystems;

public class AirshipDropSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private int slotIndex;//현재 비행선 위의 이 칸이 몇 번 슬롯인지 지정합니다.

    //드롭 이벤트 감지
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;
        if (droppedObject == null) return;
        HeroSlotUI draggedHeroSlot = droppedObject.GetComponent<HeroSlotUI>();
        if (draggedHeroSlot != null)
        {
            string heroName = draggedHeroSlot.GetHeroName();
            if (string.IsNullOrEmpty(heroName)) return;
            HeroFormationManager formationManager = HeroFormationManager.Instance;//영웅 이름 추출 및 진형 매니저 연동
            if (formationManager != null)
            {
                bool success = formationManager.TrySetHeroToSlot(slotIndex, heroName);
                if (success)
                {
                    Debug.Log($"비행선 {slotIndex}번 슬롯에 영웅 [{heroName}] 배치 성공!");
                    droppedObject.transform.SetParent(transform);//배치 처리
                    RectTransform rect = droppedObject.GetComponent<RectTransform>();
                    rect.anchoredPosition = Vector2.zero;
                }
                else
                {
                    Debug.LogWarning($"영웅 [{heroName}]을(를) {slotIndex}번 슬롯에 배치하는 데 실패했습니다.");
                }
            }
        }
    }
}