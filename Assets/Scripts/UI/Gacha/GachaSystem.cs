using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GachaSystem : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private TextMeshProUGUI resultText;   
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private GachaResultDisplay resultDisplay;
    [Header("뽑기 비용 설정")]
    [SerializeField] private CurrencyCost singlePullCost = new CurrencyCost(CurrencyType.Gold, 1000);
    [SerializeField] private CurrencyCost tenPullCost = new CurrencyCost(CurrencyType.Gold, 10000);

    // 뽑기 상품 데이터 구조체
    [System.Serializable]
    public class GachaItem
    {
        public string itemName;    
        [Range(0f, 100f)]
        public float dropRate;     
    }

    [Header("뽑기 아이템 리스트")]
    [SerializeField]
    private List<GachaItem> gachaPool = new List<GachaItem>()
    {
        new GachaItem { itemName = "검 (일반)", dropRate = 60f },
        new GachaItem { itemName = "임시 점검 (희귀)", dropRate = 30f },
        new GachaItem { itemName = "연장 점검 (영웅)", dropRate = 8f },
        new GachaItem { itemName = "서비스 점검 (전설)", dropRate = 2f }
    };

    private void Start()
    {
        UpdateUI();
    }

    //1회 뽑기 버튼에 연결할 함수
    public void PullGachaSingle()
    {
        if (!WalletManager.Instance.TrySpend(singlePullCost.Type, singlePullCost.Amount))
        {
            Debug.Log("재화가 부족합니다.");
            return;
        }
        string item = GetRandomItem();
        UpdateUI();
        if (resultDisplay != null)
            resultDisplay.ShowResults(new List<string> { item });
    }

    //10연차 뽑기 버튼에 연결할 함수
    public void PullGachaTen()
    {
        if (!WalletManager.Instance.TrySpend(tenPullCost.Type, tenPullCost.Amount))
        {
            Debug.Log("재화가 부족합니다.");
            return;
        }
        List<string> items = new List<string>();
        for (int i = 0; i < 10; i++) items.Add(GetRandomItem());
        UpdateUI();
        if (resultDisplay != null)
            resultDisplay.ShowResults(items);
    }
    // 확률에 따라 아이템을 추첨
    private string GetRandomItem()
    {
        //전체 확률의 합계 구하기
        float totalWeight = 0f;
        foreach (var item in gachaPool)
        {
            totalWeight += item.dropRate;
        }

        // 0부터 총 가중치 사이의 랜덤 값 뽑기
        float randomValue = Random.Range(0f, totalWeight);
        float currentWeightSum = 0f;

        // 당첨된 아이템 판정
        foreach (var item in gachaPool)
        {
            currentWeightSum += item.dropRate;
            if (randomValue <= currentWeightSum)
            {
                return item.itemName;
            }
        }
        return gachaPool[0].itemName; 
    }

    private void UpdateUI()
    {
        if (goldText != null && WalletManager.Instance != null)
        {
            // CurrencyType.Gold를 넣어 현재 보유한 골드 표시
            goldText.text = $"{WalletManager.Instance.GetAmount(CurrencyType.Gold)}";
        }
    }
}