using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GachaSystem : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private TextMeshProUGUI resultText;   
    [SerializeField] private TextMeshProUGUI goldText;   
    [Header("재화 설정")]
    private int playerGold = 10000;                        // 보유 골드 (테스트용)
    private int gachaCost = 1000;                          // 1회 뽑기 비용

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
        Debug.Log("뽑기 버튼 클릭됨! 함수 진입 성공!");
        if (playerGold < gachaCost)
        {
            if (resultText != null) resultText.text = "재화(골드)가 부족합니다!";
            return;
        }

        playerGold -= gachaCost;
        string pulledItem = GetRandomItem();

        if (resultText != null)
        {
            resultText.text = $"🎉 획득 아이템:\n<color=yellow>{pulledItem}</color>";
        }

        UpdateUI();
    }

    //10연차 뽑기 버튼에 연결할 함수
    public void PullGachaTen()
    {
        int totalCost = gachaCost * 10;
        if (playerGold < totalCost)
        {
            if (resultText != null) resultText.text = "재화가 부족하여 10연차를 할 수 없습니다!";
            return;
        }

        playerGold -= totalCost;
        List<string> pulledItems = new List<string>();

        for (int i = 0; i < 10; i++)
        {
            pulledItems.Add(GetRandomItem());
        }

        if (resultText != null)
        {
            resultText.text = "🎉 10연차 결과:\n" + string.Join("\n", pulledItems);
        }

        UpdateUI();
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
        if (goldText != null) goldText.text = $"보유 골드: {playerGold} G";
    }
}