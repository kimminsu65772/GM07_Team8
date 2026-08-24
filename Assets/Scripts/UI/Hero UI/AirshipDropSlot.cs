using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AirshipDropSlot : MonoBehaviour, IDropHandler
{
    [SerializeField, Range(0, 4)] private int slotIndex;//현재 비행선 위의 이 칸이 몇 번 슬롯인지 지정합니다.
    [SerializeField] private Image heroIcon;

    private Button btn;

    // 영웅이 슬롯에 드롭되었을 때, 슬롯의 인덱스와 드롭된 영웅의 이름을 전달하여 배치를 처리할 수 있도록 함.
    public event Action<int, HeroNameEnum> OnHeroDropped;
    // 슬롯 비우기 요청 이벤트
    public event Action<int> OnSlotClearRequested;

    public int SlotIndex => slotIndex;

    private void Awake()
    {
        if (heroIcon == null)
        {
            Debug.LogError("AirshipDropSlot: heroIcon이 할당되지 않았습니다.");
        }

        btn = GetComponent<Button>();

        if (btn != null)
        {
            btn.onClick.AddListener(() =>
            {
                OnSlotClearRequested?.Invoke(slotIndex);
            });
        }
    }

    //드롭 이벤트 감지
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;

        if (droppedObject == null)
        {
            Debug.LogError("드롭된 오브젝트가 없습니다.");
            return;
        }

        HeroSlotUI heroSlotUI = droppedObject.GetComponent<HeroSlotUI>();

        if (heroSlotUI == null)
        {
            Debug.LogError("드롭된 오브젝트가 HeroSlotUI 컴포넌트를 가지고 있지 않습니다.");
            return;
        }

        HeroNameEnum heroId = heroSlotUI.GetHeroId();
        
        if (heroId == HeroNameEnum.None)
        {
            Debug.LogError("배치하려는 영웅의 Id가 없습니다.");
            return;
        }

        OnHeroDropped?.Invoke(slotIndex, heroId); 
    }

    // 슬롯 채우기
    public void SetHero(HeroEntry entry)
    {
        if (entry == null)
        {
            Debug.LogError("SetHero 호출 시 entry가 null입니다.");
            return;
        }
        if (heroIcon == null)
        {
            Debug.LogError("heroIcon이 할당되지 않았습니다.");
            return;
        }

        heroIcon.sprite = entry.HeroIcon;
        heroIcon.gameObject.SetActive(true);
    }

    public bool TrySetHero(HeroEntry entry)
    {
        if (entry == null)
        {
            Debug.LogError("SetHero 호출 시 entry가 null입니다.");
            return false;
        }
        if (heroIcon == null)
        {
            Debug.LogError("heroIcon이 할당되지 않았습니다.");
            return false;
        }

        heroIcon.sprite = entry.HeroIcon;
        heroIcon.gameObject.SetActive(true);
        return true;
    }

    public void ClearHero()
    {
        if (heroIcon != null)
        {
            heroIcon.sprite = null;
            heroIcon.gameObject.SetActive(false);
        }
    }
}