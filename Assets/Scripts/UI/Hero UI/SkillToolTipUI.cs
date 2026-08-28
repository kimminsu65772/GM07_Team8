using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillToolTipUI : MonoBehaviour
{
    [Header("UI 컴포넌트 연결")]
    [SerializeField] private Image skillIconImage;
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI skillCooldownText;
    [SerializeField] private TextMeshProUGUI skillDescText;

    public void SetToolTipData(Sprite icon, string skillName, float cooldown, string description)
    {
        if (icon == null || string.IsNullOrEmpty(skillName) || skillName == "New Text")
        {
            HideToolTip();
            return;
        }
        if (skillIconImage != null) skillIconImage.sprite = icon;
        if (skillNameText != null) skillNameText.text = skillName;
        if (skillCooldownText != null) skillCooldownText.text = $"{cooldown}초";
        if (skillDescText != null) skillDescText.text = description;
        gameObject.SetActive(true);
    }
    public void ShowToolTip(Sprite icon, string skillName, float cooldown, string description)
    {
        if (skillIconImage != null)
        {
            skillIconImage.sprite = icon;
            skillIconImage.gameObject.SetActive(icon != null);
        }

        if (skillNameText != null)
        {
            skillNameText.text = skillName;
        }

        if (skillCooldownText != null)
        {
            skillCooldownText.text = $"쿨타임 : {cooldown}초";
        }

        if (skillDescText != null)
        {
            skillDescText.text = description;
        }
    }

    public void HideToolTip()
    {
        gameObject.SetActive(false);
    }
}