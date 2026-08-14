using UnityEngine;
using TMPro;
public class AirshipSlotUI : MonoBehaviour
{
    [SerializeField] private int slotIndex;
    [SerializeField] private TMP_Text slotInfoText;
    // 비행선 진형 상태가 갱신될 때 호출하여 슬롯 UI를 그려줌
    public void UpdateSlotUI(string heroName)
    {
        if (slotInfoText != null)
        {
            if (string.IsNullOrEmpty(heroName))
            {
                slotInfoText.text = $"비어있음 ({slotIndex})";
            }
            else
            {
                slotInfoText.text = $"{heroName}\n({slotIndex})";
            }
        }
    }
}