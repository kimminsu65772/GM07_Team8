using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GachaSystem : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private GachaResultDisplay resultDisplay;
    [Header("시스템 연결")]
    [SerializeField] private HeroCatalog heroCatalog;
    [Header("뽑기 비용")]
    [SerializeField] private long singlePullCost = 1000;
    [SerializeField] private long tenPullCost = 10000;

    [System.Serializable]
    public class HeroGachaItem
    {
        public HeroNameEnum heroId; 
        [Range(0f, 100f)] public float dropRate;
    }

    [Header("영웅 뽑기 풀")]
    [SerializeField] private List<HeroGachaItem> gachaPool = new List<HeroGachaItem>();

    private void Start()
    {
        UpdateUI();
    }

    public void PullGachaSingle() => ProcessPull(1, singlePullCost);
    public void PullGachaTen() => ProcessPull(10, tenPullCost);

    private void ProcessPull(int count, long cost)
    {
        if (PlayerInfo.Instance == null)
        {
            Debug.LogError("씬에 PlayerInfo가 존재하지 않습니다!");
            return;
        }

        bool success = PlayerInfo.Instance.TrySpendCurrency(CurrencyType.Gold, cost);
        if (!success)
        {
            Debug.Log("재화가 부족합니다.");
            return;
        }

        if (heroCatalog == null)
        {
            Debug.LogError("GachaSystem에 HeroCatalog가 연결되지 않았습니다!");
            return;
        }

        List<HeroNameEnum> pulledNames = new();

        for (int i = 0; i < count; i++)
        {
            HeroNameEnum pickedHeroId = GetRandomHeroId();
            pulledNames.Add(pickedHeroId);
            if (heroCatalog.TryGetHeroEntry(pickedHeroId, out HeroEntry entry))
            {
                PlayerInfo.Instance.SetHeroOwned(pickedHeroId, true);
            }
            else
            {
                Debug.LogWarning($"카탈로그에서 영웅을 찾을 수 없습니다: {pickedHeroId}");
            }
        }
        UpdateUI();
        if (resultDisplay != null)
        {
            resultDisplay.ShowResults(pulledNames);
        }
    }
    private HeroNameEnum GetRandomHeroId()
    {
        float totalWeight = 0f;
        foreach (var item in gachaPool) totalWeight += item.dropRate;

        float randomValue = Random.Range(0f, totalWeight);
        float currentWeightSum = 0f;

        foreach (var item in gachaPool)
        {
            currentWeightSum += item.dropRate;
            if (randomValue <= currentWeightSum) return item.heroId;
        }
        return gachaPool[0].heroId;
    }
    private void UpdateUI()
    {
        if (goldText != null && PlayerInfo.Instance != null && PlayerInfo.Instance.Wallet != null)
        {
            if (PlayerInfo.Instance.Wallet.Currencies.TryGetValue(CurrencyType.Gold, out var goldData))
            {
                goldText.text = goldData.Amount.ToString();
            }
            else
            {
                goldText.text = "0";
            }
        }
    }
}