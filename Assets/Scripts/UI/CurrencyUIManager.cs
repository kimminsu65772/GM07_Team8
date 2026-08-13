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

    private Dictionary<CurrencyType, TextMeshProUGUI> uiDict = new Dictionary<CurrencyType, TextMeshProUGUI>();

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
            if (!uiDict.ContainsKey(binding.currencyType))
            {
                uiDict.Add(binding.currencyType, binding.uiText);
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

        if (uiDict.TryGetValue(type, out TextMeshProUGUI textMesh))
        {
            if (textMesh != null)
            {
                // PlayerInfo를 통해 세이브 데이터 안의 월드 딕셔너리에서 직접 값 조회
                if (PlayerInfo.Instance.Wallet.Currencies.TryGetValue(type, out CurrencySaveData currencyData))
                {
                    textMesh.text = currencyData.Amount.ToString("N0"); // 3자리 콤마 포맷
                }
                else
                {
                    textMesh.text = "0";
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
