using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GachaResultUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private Image itemIconImage;

    public void SetUp(HeroNameEnum itemId)
    {
        if (itemNameText != null)
        {
            itemNameText.text = itemId.ToString();
        }
    }
}
