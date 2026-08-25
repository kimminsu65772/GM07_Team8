using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroLevelUpUIController : MonoBehaviour
{
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text atkText;
    [SerializeField] private TMP_Text defText;
    [SerializeField] private Image heroIcon;
    [SerializeField] private Button levelUpBtn;
    [SerializeField] private Image currencyIcon;
    [SerializeField] private TMP_Text currencyText;
    // TODO: 아예 레벨업 버튼을 따로 떼서 컴포넌트화 시키고 해당 버튼들을 배열로 저장해야 할듯.
    [SerializeField] private Button tenLevelUpBtn;
    [SerializeField] private Image tenCurrencyIcon;
    [SerializeField] private TMP_Text tenCurrencyText;
    [SerializeField] private HeroInventoryUIController heroInventoryUIController;
    [SerializeField] private HeroLevelUpCostTable heroLevelUpCostTable;
    [SerializeField] private CurrencyTable currencyTable;


    public event Action OnHeroLevelUp;


    private void OnEnable()
    {
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

        if (levelUpBtn != null && tenLevelUpBtn)
        {
            levelUpBtn.onClick.RemoveAllListeners();
            levelUpBtn.onClick.AddListener(OnLevelUpButtonClicked);

            tenLevelUpBtn.onClick.RemoveAllListeners();
            tenLevelUpBtn.onClick.AddListener(OnTenLevelUpButtonClicked);
        }
    }

    private void OnDisable()
    {
        if (levelUpBtn != null && tenLevelUpBtn)
        {
            levelUpBtn.onClick.RemoveAllListeners();
            tenLevelUpBtn.onClick.RemoveAllListeners();
        }

        heroInventoryUIController.OnHeroSelected -= SetLevelUpPanel;
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

        if (levelUpBtn != null)
        {
            levelUpBtn.interactable = false;
        }

        if (currencyIcon != null)
        {
            currencyIcon.enabled = false;
        }

        if (currencyText != null)
        {
            currencyText.text = string.Empty;
        }

        if (tenLevelUpBtn != null)
        {
            tenLevelUpBtn.interactable = false;
        }

        if (tenCurrencyIcon != null)
        {
            tenCurrencyIcon.enabled = false;
        }

        if (tenCurrencyText != null)
        {
            tenCurrencyText.text = string.Empty;
        }
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
        hpText.text = $"{currentStat.MaxHP} <color=#00FF66>+{hpIncrease}</color>";
        atkText.text = $"{currentStat.Atk} <color=#00FF66>+{atkIncrease}</color>";
        defText.text = $"{currentStat.Def} <color=#00FF66>+{defIncrease}</color>";
        CurrencySO currency = currencyTable.GetCurrency(heroLevelUpCostTable.CurrencyType);
        if (currency != null)
        {
            currencyIcon.sprite = currency.CurrencyIcon;
            tenCurrencyIcon.sprite = currency.CurrencyIcon; 
            currencyIcon.enabled = true;
            tenCurrencyIcon.enabled = true;
            currencyText.text = $"{heroLevelUpCostTable.GetCost(heroSaveData.Level)}";
            tenCurrencyText.text = $"{heroLevelUpCostTable.GetCostForNextTenLevels(heroSaveData.Level)}";
        }
        else
        {
            currencyIcon.enabled = false;
            tenCurrencyIcon.enabled = false;
            currencyText.text = string.Empty;
            tenCurrencyText.text = string.Empty;
        }

        if (heroIcon != null)
        {
            heroIcon.sprite = heroEntry.HeroIcon;
            heroIcon.enabled = true;
        }
        levelUpBtn.interactable = true;
        tenLevelUpBtn.interactable = true;
    }

    private void OnLevelUpButtonClicked()
    {
        if (heroInventoryUIController.SelectecHeroEntry == null || heroInventoryUIController.SelectedHeroSaveData == null)
        {
            Debug.LogWarning("레벨업 버튼 클릭 시 선택된 영웅 정보가 없습니다.");
            return;
        }

        // 나중에 PlayerInfo 관련 타입 변경 작업이 끝나면 형변환 삭제 필요.
        bool levelUpSuccess = PlayerInfo.Instance.TrySpendCurrency(heroLevelUpCostTable.CurrencyType, 
            (int)heroLevelUpCostTable.GetCost(heroInventoryUIController.SelectedHeroSaveData.Level));

        if (levelUpSuccess)
        {
            PlayerInfo.Instance.SetHeroLevel(heroInventoryUIController.SelectecHeroEntry.HeroId, 
                heroInventoryUIController.SelectedHeroSaveData.Level + 1);
        }

        SetLevelUpPanel(heroInventoryUIController.SelectecHeroEntry, 
            heroInventoryUIController.SelectedHeroSaveData);

        OnHeroLevelUp?.Invoke();
    }

    private void OnTenLevelUpButtonClicked()
    {
        if (heroInventoryUIController.SelectecHeroEntry == null || heroInventoryUIController.SelectedHeroSaveData == null)
        {
            Debug.LogWarning("레벨업 버튼 클릭 시 선택된 영웅 정보가 없습니다.");
            return;
        }

        // 나중에 PlayerInfo 관련 타입 변경 작업이 끝나면 형변환 삭제 필요.
        bool levelUpSuccess = PlayerInfo.Instance.TrySpendCurrency(heroLevelUpCostTable.CurrencyType,
            (int)heroLevelUpCostTable.GetCostForNextTenLevels(heroInventoryUIController.SelectedHeroSaveData.Level));

        if (levelUpSuccess)
        {
            PlayerInfo.Instance.SetHeroLevel(heroInventoryUIController.SelectecHeroEntry.HeroId,
                heroInventoryUIController.SelectedHeroSaveData.Level + 10);
        }

        SetLevelUpPanel(heroInventoryUIController.SelectecHeroEntry,
            heroInventoryUIController.SelectedHeroSaveData);

        OnHeroLevelUp?.Invoke();
    }
}
