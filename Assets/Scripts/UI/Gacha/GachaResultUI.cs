using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GachaResultUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private Image itemIconImage;

    public void SetUp(string itemName)
    {
        if (itemNameText != null)
        {
            itemNameText.text = itemName;
        }
    }
}
