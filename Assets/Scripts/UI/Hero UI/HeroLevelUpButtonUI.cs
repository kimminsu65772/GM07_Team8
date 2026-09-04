using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroLevelUpButtonUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image currencyIcon;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Color affordableTextColor = Color.white;
    [SerializeField] private Color insufficientTextColor = Color.red;

    public void Initialize(Action onClicked)
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }

        if (button == null)
        {
            return;
        }

        button.onClick.RemoveAllListeners();
        if (onClicked != null)
        {
            button.onClick.AddListener(() =>
            {
                PlayClickSound();
                onClicked.Invoke();
            });
        }
    }

    private void PlayClickSound()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(SoundId.UIButtonClick);
        }
    }

    public void Clear()
    {
        SetVisible(true);

        if (button != null)
        {
            button.interactable = false;
        }

        if (currencyIcon != null)
        {
            currencyIcon.enabled = false;
            currencyIcon.sprite = null;
        }

        if (costText != null)
        {
            costText.text = string.Empty;
            costText.color = affordableTextColor;
        }
    }

    public void SetVisible(bool isVisible)
    {
        gameObject.SetActive(isVisible);
    }

    public void SetState(Sprite currencySprite, long cost, bool canAfford)
    {
        if (currencyIcon != null)
        {
            currencyIcon.sprite = currencySprite;
            currencyIcon.enabled = currencySprite != null;
        }

        if (costText != null)
        {
            costText.text = GameFormatUtils.ToIdleNumber(cost); 
            costText.color = canAfford ? affordableTextColor : insufficientTextColor;
        }

        if (button != null)
        {
            button.interactable = canAfford;
        }
    }

    public void SetInteractable(bool isInteractable)
    {
        if (button != null)
        {
            button.interactable = isInteractable;
        }
    }

    public void Dispose()
    {
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
        }
    }
}
