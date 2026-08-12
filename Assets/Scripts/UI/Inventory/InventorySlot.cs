using UnityEngine;
using UnityEngine.UI;
public class InventorySlot : MonoBehaviour
{
    public Image iconImage;
    public void SetItem(Sprite itemSprite)
    {
        if (itemSprite != null)
        {
            iconImage.sprite = itemSprite;
            iconImage.gameObject.SetActive(true);
        }
        else
        {
            ClearSlot();
        }
    }
    public void ClearSlot()
    {
        iconImage.sprite = null;
        iconImage.gameObject.SetActive(false);
    }
}