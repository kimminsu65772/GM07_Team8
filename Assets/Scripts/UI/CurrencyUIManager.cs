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
        RefreshAllUI();
    }
    // 특정 재화의 UI만 갱신
    public void UpdateCurrencyUI(CurrencyType type)
    {
        if (WalletManager.Instance == null || !WalletManager.Instance.IsInitialized) return;

        if (uiDict.TryGetValue(type, out TextMeshProUGUI textMesh))
        {
            if (textMesh != null)
            {
                int currentAmount = WalletManager.Instance.GetAmount(type);
                textMesh.text = currentAmount.ToString("N0"); // 3자리마다 쉼표(,) 추가 (예: 10,000)
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
