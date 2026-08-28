using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DecomposeResultSlotUI : MonoBehaviour
{
    [SerializeField] private Image matterIcon;
    [SerializeField] private TMP_Text numberText;

    public void SetSlot(Sprite icon, int amount)
    {
        gameObject.SetActive(true);

        if (matterIcon != null)
        {
            matterIcon.sprite = icon;
            matterIcon.enabled = icon != null;
        }

        if (numberText != null)
        {
            numberText.text = "x"+amount.ToString();
        }
    }

    public void ClearSlot()
    {
        gameObject.SetActive(false);

        if (matterIcon != null)
        {
            matterIcon.sprite = null;
            matterIcon.enabled = false;
        }

        if (numberText != null)
        {
            numberText.text = string.Empty;
        }
    }
}
