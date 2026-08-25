using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class HeroLevelUpUIController : MonoBehaviour
{
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text atkText;
    [SerializeField] private TMP_Text defText;
    [SerializeField] private Image heroIcon;
    [SerializeField] private HeroLevelUpButtonUI levelUpButton;
    [SerializeField] private HeroLevelUpButtonUI tenLevelUpButton;
    [SerializeField] private RectTransform levelUpButtonRect;
    [SerializeField] private HeroInventoryUIController heroInventoryUIController;
    [SerializeField] private HeroLevelUpCostTable heroLevelUpCostTable;
    [SerializeField] private CurrencyTable currencyTable;
    [SerializeField] private GameObject unownedHeroOverlay;

    private Vector2 pairedAnchorMin;
    private Vector2 pairedAnchorMax;
    private Vector2 pairedPivot;
    private Vector2 pairedAnchoredPosition;
    private bool hasPairedLayout;

    public event Action OnHeroLevelUp;

    private void Awake()
    {
        CachePairedButtonLayout();
    }

    private void OnEnable()
    {
        CachePairedButtonLayout();

        if (heroInventoryUIController == null)
        {
            Clear();
            return;
        }

        heroInventoryUIController.OnHeroSelected -= SetLevelUpPanel;
        heroInventoryUIController.OnHeroSelected += SetLevelUpPanel;

        if (heroInventoryUIController.SelectecHeroEntry == null || heroInventoryUIController.SelectedHeroSaveData == null)
        {
            Clear();
            return;
        }

        if (heroLevelUpCostTable == null)
        {
            Debug.LogError("HeroLevelUpCostTable이 할당되지 않았습니다.");
            Clear();
            return;
        }

        if (currencyTable == null)
        {
            Debug.LogError("CurrencyTable이 할당되지 않았습니다.");
            Clear();
            return;
        }

        SetLevelUpPanel(heroInventoryUIController.SelectecHeroEntry, heroInventoryUIController.SelectedHeroSaveData);

        if (levelUpButton != null)
        {
            levelUpButton.Initialize(() => OnLevelUpButtonClicked(1));
        }

        if (tenLevelUpButton != null)
        {
            tenLevelUpButton.Initialize(() => OnLevelUpButtonClicked(10));
        }

        if (PlayerInfo.Instance != null)
        {
            PlayerInfo.Instance.OnCurrencyChanged -= HandleCurrencyChanged;
            PlayerInfo.Instance.OnCurrencyChanged += HandleCurrencyChanged;
        }
    }

    private void OnDisable()
    {
        if (levelUpButton != null)
        {
            levelUpButton.Dispose();
        }

        if (tenLevelUpButton != null)
        {
            tenLevelUpButton.Dispose();
        }

        if (heroInventoryUIController != null)
        {
            heroInventoryUIController.OnHeroSelected -= SetLevelUpPanel;
        }

        if (PlayerInfo.Instance != null)
        {
            PlayerInfo.Instance.OnCurrencyChanged -= HandleCurrencyChanged;
        }
    }

    private void Clear()
    {
        if (levelText != null) levelText.text = string.Empty;
        if (hpText != null) hpText.text = string.Empty;
        if (atkText != null) atkText.text = string.Empty;
        if (defText != null) defText.text = string.Empty;

        if (heroIcon != null)
        {
            heroIcon.sprite = null;
            heroIcon.enabled = false;
        }

        if (levelUpButton != null)
        {
            levelUpButton.Clear();
        }

        if (tenLevelUpButton != null)
        {
            tenLevelUpButton.Clear();
        }

        SetTenLevelUpVisible(false);
        SetUnownedHeroOverlayVisible(false);
    }

    private void SetLevelUpPanel(HeroEntry heroEntry, HeroSaveData heroSaveData)
    {
        if (heroEntry == null || heroSaveData == null)
        {
            Clear();
            return;
        }

        HeroStat currentStat = HeroStats
            .GetStatTable((int)heroEntry.HeroId)
            .GetStat(heroSaveData.Level);

        HeroStat nextStat = HeroStats
            .GetStatTable((int)heroEntry.HeroId)
            .GetStat(heroSaveData.Level + 1);

        double hpIncrease = nextStat.MaxHP - currentStat.MaxHP;
        double atkIncrease = nextStat.Atk - currentStat.Atk;
        double defIncrease = nextStat.Def - currentStat.Def;

        levelText.text = $"{heroSaveData.Level}";
        hpText.text = $"{GameFormatUtils.ToIdleNumber(currentStat.MaxHP)} <color=#00FF66>+{GameFormatUtils.ToIdleNumber(hpIncrease)}</color>";
        atkText.text = $"{GameFormatUtils.ToIdleNumber(currentStat.Atk)} <color=#00FF66>+{GameFormatUtils.ToIdleNumber(atkIncrease)}</color>";
        defText.text = $"{GameFormatUtils.ToIdleNumber(currentStat.Def)} <color=#00FF66>+{GameFormatUtils.ToIdleNumber(defIncrease)}</color>";

        CurrencySO currency = currencyTable.GetCurrency(heroLevelUpCostTable.CurrencyType);
        Sprite currencySprite = currency != null ? currency.CurrencyIcon : null;
        long currentCurrencyAmount = GetCurrencyAmount(heroLevelUpCostTable.CurrencyType);
        long oneLevelCost = heroLevelUpCostTable.GetCost(heroSaveData.Level);
        bool canUseCurrency = currency != null;
        bool canAffordOneLevel = canUseCurrency && currentCurrencyAmount >= oneLevelCost;

        if (levelUpButton != null)
        {
            levelUpButton.SetState(currencySprite, oneLevelCost, canAffordOneLevel);
        }

        long tenLevelCost = heroLevelUpCostTable.GetCostForNextTenLevels(heroSaveData.Level);

        // 10레벨을 올릴 수 없는 상태라면 버튼을 숨기기 위해 조건을 체크한다.
        bool canShowTenLevelButton =
            canUseCurrency &&
            CanLevelUp(heroEntry, heroSaveData.Level, 10) &&
            currentCurrencyAmount >= tenLevelCost;
        SetTenLevelUpVisible(canShowTenLevelButton);

        if (canShowTenLevelButton)
        {
            if (tenLevelUpButton != null)
            {
                tenLevelUpButton.SetState(currencySprite, tenLevelCost, true);
            }
        }

        if (heroIcon != null)
        {
            heroIcon.sprite = heroEntry.HeroIcon;
            heroIcon.enabled = true;
        }

        SetUnownedHeroOverlayVisible(!heroSaveData.IsOwned);

        if (!heroSaveData.IsOwned)
        {
            SetLevelUpButtonsInteractable(false);
        }
    }

    private void OnLevelUpButtonClicked(int levelAmount)
    {
        if (heroInventoryUIController.SelectecHeroEntry == null || heroInventoryUIController.SelectedHeroSaveData == null)
        {
            Debug.LogWarning("레벨업 버튼 클릭 시 선택된 영웅 정보가 없습니다.");
            return;
        }

        HeroSaveData heroSaveData = heroInventoryUIController.SelectedHeroSaveData;

        if (!CanLevelUp(heroInventoryUIController.SelectecHeroEntry, heroSaveData.Level, levelAmount))
        {
            return;
        }

        long cost = levelAmount == 10
            ? heroLevelUpCostTable.GetCostForNextTenLevels(heroSaveData.Level)
            : heroLevelUpCostTable.GetCost(heroSaveData.Level);

        bool levelUpSuccess = PlayerInfo.Instance.TrySpendCurrency(heroLevelUpCostTable.CurrencyType, cost);

        if (levelUpSuccess)
        {
            PlayerInfo.Instance.SetHeroLevel(heroInventoryUIController.SelectecHeroEntry.HeroId, 
                heroSaveData.Level + levelAmount);

            SetLevelUpPanel(heroInventoryUIController.SelectecHeroEntry,
                heroInventoryUIController.SelectedHeroSaveData);

            OnHeroLevelUp?.Invoke();
            return;
        }

        SetLevelUpPanel(heroInventoryUIController.SelectecHeroEntry,
            heroInventoryUIController.SelectedHeroSaveData);
    }

    private long GetCurrencyAmount(CurrencyType currencyType)
    {
        PlayerInfo playerInfo = PlayerInfo.Instance;
        if (playerInfo == null ||
            playerInfo.Wallet == null ||
            playerInfo.Wallet.Currencies == null ||
            !playerInfo.Wallet.Currencies.TryGetValue(currencyType, out CurrencySaveData currency))
        {
            return 0;
        }

        return currency.Amount;
    }

    private bool CanLevelUp(HeroEntry heroEntry, int currentLevel, int levelAmount)
    {
        if (heroEntry == null || levelAmount <= 0)
        {
            return false;
        }

        try
        {
            HeroStats
                .GetStatTable((int)heroEntry.HeroId)
                .GetStat(currentLevel + levelAmount);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private void SetTenLevelUpVisible(bool isVisible)
    {
        if (tenLevelUpButton != null)
        {
            tenLevelUpButton.SetVisible(isVisible);
        }

        if (levelUpButtonRect == null)
        {
            return;
        }

        if (isVisible)
        {
            RestorePairedButtonLayout();
            return;
        }

        CenterLevelUpButton();
    }

    private void SetLevelUpButtonsInteractable(bool isInteractable)
    {
        if (levelUpButton != null)
        {
            levelUpButton.SetInteractable(isInteractable);
        }

        if (tenLevelUpButton != null)
        {
            tenLevelUpButton.SetInteractable(isInteractable);
        }
    }

    private void SetUnownedHeroOverlayVisible(bool isVisible)
    {
        if (unownedHeroOverlay == null)
        {
            return;
        }

        unownedHeroOverlay.SetActive(isVisible);
    }

    private void CachePairedButtonLayout()
    {
        if (levelUpButtonRect == null || hasPairedLayout)
        {
            return;
        }

        pairedAnchorMin = levelUpButtonRect.anchorMin;
        pairedAnchorMax = levelUpButtonRect.anchorMax;
        pairedPivot = levelUpButtonRect.pivot;
        pairedAnchoredPosition = levelUpButtonRect.anchoredPosition;
        hasPairedLayout = true;
    }

    private void RestorePairedButtonLayout()
    {
        if (!hasPairedLayout)
        {
            return;
        }

        levelUpButtonRect.anchorMin = pairedAnchorMin;
        levelUpButtonRect.anchorMax = pairedAnchorMax;
        levelUpButtonRect.pivot = pairedPivot;
        levelUpButtonRect.anchoredPosition = pairedAnchoredPosition;
    }

    private void CenterLevelUpButton()
    {
        Vector2 anchorMin = levelUpButtonRect.anchorMin;
        Vector2 anchorMax = levelUpButtonRect.anchorMax;
        Vector2 pivot = levelUpButtonRect.pivot;
        Vector2 anchoredPosition = levelUpButtonRect.anchoredPosition;

        anchorMin.x = 0.5f;
        anchorMax.x = 0.5f;
        pivot.x = 0.5f;
        anchoredPosition.x = 0f;

        levelUpButtonRect.anchorMin = anchorMin;
        levelUpButtonRect.anchorMax = anchorMax;
        levelUpButtonRect.pivot = pivot;
        levelUpButtonRect.anchoredPosition = anchoredPosition;
    }

    private void HandleCurrencyChanged(CurrencyType currencyTypet)
    {
        if (currencyTypet != heroLevelUpCostTable.CurrencyType)
        {
            return;
        }
        if (heroInventoryUIController.SelectecHeroEntry == null || heroInventoryUIController.SelectedHeroSaveData == null)
        {
            return;
        }
        Clear();
        SetLevelUpPanel(heroInventoryUIController.SelectecHeroEntry, heroInventoryUIController.SelectedHeroSaveData);
    }
}
