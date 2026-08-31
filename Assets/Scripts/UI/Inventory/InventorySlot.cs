using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text itemCountText;

    public void SetItem(Sprite itemSprite, long amount)
    {
        if (itemSprite == null || amount <= 0)
        {
            ClearSlot();
            return;
        }

        iconImage.sprite = itemSprite;
        iconImage.gameObject.SetActive(true);

        if (itemCountText != null)
        {
            string formattedAmount = GameFormatUtils.ToIdleNumber(amount);
            itemCountText.text = "x" + formattedAmount;
            itemCountText.gameObject.SetActive(true);
        }
    }

    public void ClearSlot()
    {
        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.gameObject.SetActive(false);
        }

        if (itemCountText != null)
        {
            itemCountText.text = string.Empty;
            itemCountText.gameObject.SetActive(false);
        }
    }
}
