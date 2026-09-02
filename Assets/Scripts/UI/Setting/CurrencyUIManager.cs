using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrencyUIManager : MonoBehaviour
{
    public static CurrencyUIManager Instance { get; private set; }

    [System.Serializable]
    public struct CurrencyUIBinding
    {
        public CurrencyType currencyType;
        public TextMeshProUGUI uiText;
    }
    [Header("재화 UI 매핑 설정")]
    [SerializeField] private List<CurrencyUIBinding> uiBindings = new List<CurrencyUIBinding>();

    private Dictionary<CurrencyType, List<TextMeshProUGUI>> uiDict = new Dictionary<CurrencyType, List<TextMeshProUGUI>>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 딕셔너리로 빠르게 찾을 수 있도록 변환
        foreach (var binding in uiBindings)
        {
            if (binding.uiText == null) continue;

            if (!uiDict.ContainsKey(binding.currencyType))
            {
                uiDict[binding.currencyType] = new List<TextMeshProUGUI>();
            }
            // 중복 추가 방지
            if (!uiDict[binding.currencyType].Contains(binding.uiText))
            {
                uiDict[binding.currencyType].Add(binding.uiText);
            }
        }
    }
    private void Start()
    {
        // 게임 시작 시 모든 재화 UI 초기 갱신
        if (PlayerInfo.Instance != null && PlayerInfo.Instance.IsInitialized)
        {
            RefreshAllUI();
        }
    }
    private void OnEnable()
    {
        // PlayerInfo의 재화 변경 이벤트 구독
        if (PlayerInfo.Instance != null)
        {
            PlayerInfo.Instance.OnCurrencyChanged += OnCurrencyChangedHandler;
        }
    }
    private void OnDisable()
    {
        // 이벤트 해제 (메모리 누수 방지)
        if (PlayerInfo.Instance != null)
        {
            PlayerInfo.Instance.OnCurrencyChanged -= OnCurrencyChangedHandler;
        }
    }

    private void OnCurrencyChangedHandler(CurrencyType type)
    {
        // 값이 바뀐 특정 재화의 UI만 갱신
        UpdateCurrencyUI(type);
    }
    // 특정 재화의 UI만 갱신
    public void UpdateCurrencyUI(CurrencyType type)
    {
        // PlayerInfo가 초기화되었고 지갑 데이터가 있는지 확인
        if (PlayerInfo.Instance == null || !PlayerInfo.Instance.IsInitialized) return;

        if (uiDict.TryGetValue(type, out List<TextMeshProUGUI> textMeshList))
        {
            string formattedAmount = "0";
            if (PlayerInfo.Instance.Wallet.Currencies.TryGetValue(type, out CurrencySaveData currencyData))
            {
                formattedAmount = GameFormatUtils.ToIdleNumber(currencyData.Amount);
            }

            // 해당 재화에 연결된 모든 텍스트에 값 적용
            foreach (var textMesh in textMeshList)
            {
                if (textMesh != null)
                {
                    textMesh.text = formattedAmount;
                }
            }
        }
    }
    // 전체 재화 UI 갱신
    public void RefreshAllUI()
    {
        foreach (var kvp in uiDict)
        {
            UpdateCurrencyUI(kvp.Key);
        }
    }
}
