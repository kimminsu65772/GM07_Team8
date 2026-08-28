using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaterialCostSlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] private Color enoughColor = Color.white;
    [SerializeField] private Color notEnoughColor = Color.red;

    public void Bind(ItemAmount cost, ItemSO item)
    {
        PlayerInfo playerInfo = PlayerInfo.Instance;

        if (cost == null)
        {
            Clear();
            return;
        }

        if (iconImage != null)
        {
            iconImage.sprite = item != null ? item.ItemIcon : null;
            iconImage.enabled = iconImage.sprite != null;
        }

        if (amountText != null)
        {
            int ownedAmount = playerInfo != null ? playerInfo.GetItemAmount(cost.itemId) : 0;
            bool hasEnough = playerInfo != null && playerInfo.HasEnoughItem(cost.itemId, cost.amount);
            amountText.text = $"{ownedAmount}/{cost.amount}";
            amountText.color = hasEnough ? enoughColor : notEnoughColor;
        }

        gameObject.SetActive(true);
    }

    public void Clear()
    {
        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }

        if (amountText != null)
        {
            amountText.text = string.Empty;
            amountText.color = enoughColor;
        }
    }
}
