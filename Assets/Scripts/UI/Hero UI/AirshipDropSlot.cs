using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AirshipDropSlot : MonoBehaviour
{
    [SerializeField, Range(0, 4)] private int slotIndex;//현재 비행선 위의 이 칸이 몇 번 슬롯인지 지정합니다.
    [SerializeField] private Image heroIcon;
    [SerializeField] private GameObject highlightObject;

    private Button btn;
    private bool isOccupied = false; // 현재 슬롯에 영웅이 들어있는지 여부

    public event Action<int> OnSlotClicked;
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
                if (isOccupied)
                {
                    OnSlotClearRequested?.Invoke(slotIndex);
                }
                else
                {
                    OnSlotClicked?.Invoke(slotIndex);
                }
            });
        }
        SetHighlight(false);
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
        if (entry == null) return false;
        if (heroIcon != null)
        {
            heroIcon.sprite = entry.HeroIcon;
            heroIcon.gameObject.SetActive(true);
        }
        isOccupied = true;
        return true;
    }

    public void ClearHero()
    {
        if (heroIcon != null)
        {
            heroIcon.sprite = null;
            heroIcon.gameObject.SetActive(false);
        }
        isOccupied = false;
        SetHighlight(false); // 비워질 때 강조도 함께 해제
    }
    public void SetHighlight(bool isActive)
    {
        if (highlightObject != null)
        {
            highlightObject.SetActive(isActive);
        }
    }
}