using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class HeroInventoryUI : MonoBehaviour
{
    [Header("UI 연결 (아래쪽 리스트)")]
    [SerializeField] private Transform contentGrid;
    [SerializeField] private GameObject heroSlotPrefab;
    [SerializeField] private HeroCatalog heroCatalog;

    [Header("상세 정보 UI 연결 (위쪽 패널)")]
    [SerializeField] private GameObject detailPanelRoot;     
    [SerializeField] private TextMeshProUGUI detailNameText;   
    [SerializeField] private TextMeshProUGUI detailDescText;  

    private void Start()
    {
        RefreshInventory();
        ClearDetailInfo();
    }
    //인벤토리 목록 생성 및 갱신
    public void RefreshInventory()
    {
        foreach (Transform child in contentGrid)
        {
            Destroy(child.gameObject);
        }
        if (PlayerInfo.Instance == null || heroCatalog == null) return;

        Dictionary<string, HeroSaveData> savedHeroes = PlayerInfo.Instance.Heroes;

        foreach (var pair in savedHeroes)
        {
            string heroName = pair.Key;
            HeroSaveData heroData = pair.Value;

            if (heroCatalog.TryGetHeroEntry(heroName, out HeroEntry entry))
            {
                GameObject slotObj = Instantiate(heroSlotPrefab, contentGrid);
                HeroSlotUI slotUI = slotObj.GetComponent<HeroSlotUI>();

                if (slotUI != null)
                {
                    bool isOwned = heroData.IsOwned;

                    slotUI.SetupSlot(entry, heroData, isOwned, (clickedEntry, clickedSaveData) => {
                        ShowDetailInfo(clickedEntry, clickedSaveData);
                    });
                }
            }
        }
    }
    //영웅 상세 정보 표시
    private void ShowDetailInfo(HeroEntry entry, HeroSaveData saveData)
    {
        if (detailPanelRoot != null) detailPanelRoot.SetActive(true);
        if (detailNameText != null)
        {
            detailNameText.text = $"{entry.HeroName} (Lv.{saveData.Level})";
        }
        if (detailDescText != null)
        {      
            detailDescText.text = $"영웅 위치: {entry.HeroLocation}\n소유 여부: {(saveData.IsOwned ? "보유 중" : "미보유")}";
        }
    }
    //상세 정보 초기화
    private void ClearDetailInfo()
    {
        if (detailNameText != null) detailNameText.text = "영웅을 선택해주세요.";
        if (detailDescText != null) detailDescText.text = "";
    }
}