using TMPro;
using UnityEngine;

public class HeroPowerDisplay : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI attackPowerText;  // 통합 공격력 UI 텍스트
    [SerializeField] private TextMeshProUGUI defensePowerText; // 통합 수비력 UI 텍스트

    [Header("Weights (가중치 설정)")]
    [SerializeField] private float attackWeight = 10f;        
    [SerializeField] private float critRateWeight = 200f;    
    [SerializeField] private float defenseWeight = 10f;      
    [SerializeField] private float healthWeight = 2f;        

    private HeroEntry currentHeroEntry;

    public int TotalAttackPower { get; private set; }
    public int TotalDefensePower { get; private set; }

    public void SetHero(HeroEntry heroEntry)
    {
        currentHeroEntry = heroEntry;

        if (currentHeroEntry != null && currentHeroEntry.HeroId != HeroNameEnum.None) // 프로젝트에 맞는 빈 영웅 조건 체크
        {
            CalculatePowers();
            UpdateUI();
        }
        else
        {
            ClearHero(); // 영웅이 없으면 깔끔하게 초기화 및 숨김/빈 칸 처리
        }
    }
    public void ClearHero()
    {
        currentHeroEntry = null;
        ClearUI();
    }
    public void CalculatePowers()
    {
        if (currentHeroEntry == null) return;

        HeroStat stat = currentHeroEntry.GetHeroStat();

        float atk = (float)stat.Atk;
        float def = (float)stat.Def;
        float hp = (float)stat.MaxHP;

        // 장비에서 크리티컬 확률 가져와 합산
        float critRate = GetEquippedCritRate(currentHeroEntry.HeroId); 

        TotalAttackPower = Mathf.FloorToInt((atk * attackWeight) + (critRate * critRateWeight));
        TotalDefensePower = Mathf.FloorToInt((def * defenseWeight) + (hp * healthWeight));
    }
    private float GetEquippedCritRate(HeroNameEnum heroId)
    {
        float totalCritChance = 0f;

        if (PlayerInfo.Instance != null)
        {
            PlayerInfo.Instance.GetHeroEquippedEquipments(heroId, out var weapon, out var armor, out var acc);

            if (weapon != null) totalCritChance += weapon.BonusCriChance;
            if (armor != null) totalCritChance += armor.BonusCriChance;
            if (acc != null) totalCritChance += acc.BonusCriChance;
        }

        return totalCritChance;
    }
    private void UpdateUI()
    {
        if (attackPowerText != null) attackPowerText.text = TotalAttackPower.ToString("N0");
        if (defensePowerText != null) defensePowerText.text = TotalDefensePower.ToString("N0");
    }

    private void ClearUI()
    {
        TotalAttackPower = 0;
        TotalDefensePower = 0;
        if (attackPowerText != null) attackPowerText.text = "";
        if (defensePowerText != null) defensePowerText.text = "";
    }
}