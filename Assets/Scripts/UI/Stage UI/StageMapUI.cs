using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StageMapUI : MonoBehaviour
{
    [Header("Data Reference")]
    [SerializeField] private StageCatalog stageCatalog;

    [Header("스테이지 변경자 참조")]
    [SerializeField] private StageTransitionController stageTransitionController;

    [Header("UI References")]
    [SerializeField] private Transform mapContentTransform;
    [SerializeField] private GameObject stageSlotPrefab;

    [Header("Grid Layout Settings (일정 간격 설정)")]
    [SerializeField] private float spacingX = 200f; 
    [SerializeField] private float spacingY = 150f; 
    [SerializeField] private int columns = 4;      

    [Header("Random Offset Settings (위아래 좌우 흔들림 조절)")]
    [SerializeField] private float offsetXRange = 30f;
    [SerializeField] private float offsetYRange = 50f; 

    private readonly List<StageMapSlot> spawnedSlots = new List<StageMapSlot>();
    private void OnEnable()
    {
        RefreshMap();
    }
    private void GenerateMapSlots(int maxCleared)
    {
        int totalStages = stageCatalog.StageCount;

        for (int i = 1; i <= totalStages; i++)
        {
            if(!stageCatalog.TryGetStageData(i,out StageData stageData)) continue;
            GameObject slotObj = Instantiate(stageSlotPrefab,mapContentTransform);
            StageMapSlot mapSlot = slotObj.GetComponent<StageMapSlot>();

            if (mapSlot == null)
            {
                Destroy(slotObj);
                continue;
            }
            RectTransform rectTrans = slotObj.GetComponent<RectTransform>();

            if (rectTrans != null)
            {
                int index = i - 1;
                int row = index / columns;
                int col = index % columns;
                float baseX = col * spacingX;
                float baseY = -row * spacingY;

                float randomX = Random.Range(-offsetXRange,offsetXRange);

                float randomY = Random.Range( -offsetYRange,offsetYRange);

                rectTrans.anchoredPosition = new Vector2( baseX + randomX,baseY + randomY);
            }

            mapSlot.Init(this, i,maxCleared);
            spawnedSlots.Add(mapSlot);
        }
    }

    public void RefreshMap()
    {
        if (stageCatalog == null) return;
        if (mapContentTransform == null) return;
        if (stageSlotPrefab == null) return;
        ClearMapSlots();

        int maxCleared = 0;

        if (PlayerInfo.Instance != null &&
            PlayerInfo.Instance.IsInitialized)
        {
            maxCleared = PlayerInfo.Instance.MaxClearedStage;
        }
        GenerateMapSlots(maxCleared);
    }
    private void ClearMapSlots()
    {
        foreach (StageMapSlot slot in spawnedSlots)
        {
            if (slot != null)
            {
                Destroy(slot.gameObject);
            }
        }
        spawnedSlots.Clear();
        for (int i = mapContentTransform.childCount - 1; i >= 0; i--)
        {
            Destroy(mapContentTransform.GetChild(i).gameObject);
        }
    }
    public void OnStageSelected(int stageNumber)
    {
        if (PlayerInfo.Instance != null)
        {
            bool success = PlayerInfo.Instance.TrySetCurrentStage(stageNumber);
            if (success)
            {
                stageTransitionController.StartTransition(stageNumber);
            }
        }
    }
}