using TMPro;
using UnityEngine;

public class AirshipStatsDisplayView : MonoBehaviour
{
    [Header("UI 텍스트 연결")]
    [SerializeField] private TextMeshProUGUI attackLevelText;
    [SerializeField] private TextMeshProUGUI attackValueText;

    [SerializeField] private TextMeshProUGUI defenseLevelText;
    [SerializeField] private TextMeshProUGUI defenseValueText;

    [SerializeField] private TextMeshProUGUI maxHealthLevelText;
    [SerializeField] private TextMeshProUGUI maxHealthValueText;

    [SerializeField] private TextMeshProUGUI criticalLevelText;
    [SerializeField] private TextMeshProUGUI criticalValueText;

    [Header("참조 설정")]
    [SerializeField] private AirshipStatTable statTable; // 스탯 계산용 테이블 에셋

    private AirshipUpgradeManager upgradeManager; // 런타임에 찾아올 매니저 참조

    private void Start()
    {
        // 런타임에 씬 내부에서 AirshipUpgradeManager 컴포넌트 탐색
        upgradeManager = FindFirstObjectByType<AirshipUpgradeManager>();

        if (upgradeManager != null)
        {
            // 런타임 이벤트
            upgradeManager.OnUpgradeChanged += HandleUpgradeChanged;
            // 초기 뷰 갱신
            HandleUpgradeChanged(upgradeManager.UpgradeState);
        }
        else
        {
            Debug.LogWarning("씬 내에서 AirshipUpgradeManager를 찾을 수 없습니다!");
        }
    }
    private void OnDestroy()
    {
        if (upgradeManager != null)
        {
            upgradeManager.OnUpgradeChanged -= HandleUpgradeChanged;
        }
    }
    // 매니저로부터 최신 상태 데이터를 받아와 화면에 뿌려주는 함수
    private void HandleUpgradeChanged(AirshipUpgradeState state)
    {
        if (state == null) return;
        // 공격력 갱신
        UpdateStatUI(AirshipStatType.Attack, state.AttackLevel, attackLevelText, attackValueText);
        // 방어력 갱신
        UpdateStatUI(AirshipStatType.Defense, state.DefenseLevel, defenseLevelText, defenseValueText);
        // 최대 체력 갱신
        UpdateStatUI(AirshipStatType.MaxHealth, state.MaxHealthLevel, maxHealthLevelText, maxHealthValueText);
        // 치명타 확률 갱신
        UpdateStatUI(AirshipStatType.CriticalChance, state.CriticalLevel, criticalLevelText, criticalValueText);
    }
    private void UpdateStatUI(AirshipStatType statType, int level, TextMeshProUGUI levelText, TextMeshProUGUI valueText)
    {
        //레벨 텍스트 갱신
        if (levelText != null)
        {
            levelText.text = $"LV. {level}";
        }
        //실제 수치 텍스트 갱신 (테이블 공식 적용, 테이블이 없으면 기본 공식 사용)
        if (valueText != null)
        {
            float power = statTable != null ? statTable.GetStatValue(statType, level) : GetDefaultPower(statType, level);

            // 치명타 확률 같은 퍼센트형 스탯은 소수점 처리나 % 기호 붙이기 분기 가능
            if (statType == AirshipStatType.CriticalChance)
            {
                valueText.text = $"{power * 100f:F1}%";
            }
            else
            {
                valueText.text = $"{power:F1}";
            }
        }
    }
    // 테이블이 연결되지 않았을 때를 대비한 예비 수치 계산 공식
    private float GetDefaultPower(AirshipStatType statType, int level)
    {
        switch (statType)
        {
            case AirshipStatType.Attack: return 10f + (level - 1) * 2f;
            case AirshipStatType.Defense: return 1f + (level - 1) * 1f;
            case AirshipStatType.MaxHealth: return 100f + (level - 1) * 20f;
            case AirshipStatType.CriticalChance: return 0.05f + (level - 1) * 0.01f;
            default: return 0f;
        }
    }
}