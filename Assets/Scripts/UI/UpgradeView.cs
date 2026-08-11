using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeView : MonoBehaviour
{
    [Header("UI 텍스트 연결")]
    public TextMeshProUGUI statNameLevelText;
    public TextMeshProUGUI currentValueText;
    public TextMeshProUGUI costText;

    [Header("설정")]
    [SerializeField] private AirshipStatType targetStatType = AirshipStatType.Attack; // 어떤 스탯 창인지 지정
    [SerializeField] private AirshipStatTable statTable; // 스탯 테이블 에셋 연결 (선택사항)

    private Button upgradeButton;
    private AirshipUpgradeManager upgradeManager; // 런타임에 찾아올 매니저 참조

    private void Awake()
    {
        // 런타임에 내 오브젝트(또는 자식)의 Button 컴포넌트 찾기
        upgradeButton = GetComponent<Button>();
        if (upgradeButton == null)
        {
            upgradeButton = GetComponentInChildren<Button>();
        }
    }
    private void Start()
    {
        gameObject.SetActive(false);

        // 런타임에 씬 내부에서 AirshipUpgradeManager 컴포넌트를 코드로 탐색
        upgradeManager = FindFirstObjectByType<AirshipUpgradeManager>();
        if (upgradeManager != null)
        {
            // 런타임 이벤트 구독 (+=)
            upgradeManager.OnUpgradeChanged += HandleUpgradeChanged;
            // 초기 뷰 설정
            HandleUpgradeChanged(upgradeManager.UpgradeState);
        }
        else
        {
            Debug.LogWarning("씬 내에서 AirshipUpgradeManager를 찾을 수 없습니다!");
        }
        // 런타임 버튼 클릭 리스너 추가
        if (upgradeButton != null)
        {
            upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        }
    }
    private void OnDestroy()
    {
        // 런타임 등록했던 이벤트들 안전하게 해제 (-=, RemoveListener)
        if (upgradeManager != null)
        {
            upgradeManager.OnUpgradeChanged -= HandleUpgradeChanged;
        }

        if (upgradeButton != null)
        {
            upgradeButton.onClick.RemoveListener(OnUpgradeButtonClicked);
        }
    }
    private void HandleUpgradeChanged(AirshipUpgradeState state)
    {
        
    }
    private int GetLevelFromState(AirshipUpgradeState state, AirshipStatType statType)
    {
        switch (statType)
        {
            case AirshipStatType.Attack: return state.AttackLevel;
            case AirshipStatType.Defense: return state.DefenseLevel;
            case AirshipStatType.MaxHealth: return state.MaxHealthLevel;
            case AirshipStatType.CriticalChance: return state.CriticalLevel;
            default: return 1;
        }
    }
    private void UpdateView(int level, float power, int cost)
    {
        if (statNameLevelText != null) statNameLevelText.text = $"{targetStatType} LV.{level}";
        if (currentValueText != null) currentValueText.text = $"현재 수치: {power:F1}";
        if (costText != null) costText.text = $"비용 {cost}";
    }
    private void OnUpgradeButtonClicked()
    {
        Debug.Log($"{targetStatType} 강화 버튼 클릭 (런타임 연결)");

        if (upgradeManager == null)
        {
            Debug.LogWarning("AirshipUpgradeManager가 연결되어 있지 않습니다!");
            return;
        }
        // 런타임으로 찾은 매니저를 통해 강화 시도
        bool success = upgradeManager.TryUpgrade(targetStatType);
        if (!success)
        {
            Debug.Log("재화가 부족하거나 강화할 수 없습니다!");
            return;
        }

        Debug.Log($"{targetStatType} 강화 성공!");
    }
}