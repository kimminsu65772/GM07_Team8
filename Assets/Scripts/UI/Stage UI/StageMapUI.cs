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
    [SerializeField] private GameObject mapTemplate;
    [SerializeField] private GameObject stageSlotPrefab;

    [Header("Grid Layout Settings (일정 간격 설정)")]
    [SerializeField] private float spacingX = 280f; 
    [SerializeField] private float spacingY = 60f; 
    [SerializeField] private int columns = 10;      

    [Header("Random Offset Settings (위아래 좌우 흔들림 조절)")]
    [SerializeField] private float offsetXRange = 50f;
    [SerializeField] private float offsetYRange = 50f; 

    private readonly List<StageMapSlot> spawnedSlots = new List<StageMapSlot>();
    private readonly List<GameObject> spawnedMaps = new List<GameObject>();

    private bool isMapGenerated = false;
    private void OnEnable()
    {
        RefreshMap();
    }
    private void GenerateMapSlots(int maxCleared)
    {
        int totalStages = stageCatalog.StageCount;
        int mapCount = Mathf.CeilToInt(totalStages / 10f);

        for (int mapIndex = 0; mapIndex < mapCount; mapIndex++)
        {
            GameObject mapObj;

            if (mapIndex == 0)
            {
                mapObj = mapTemplate;
            }
            else
            {
                mapObj = Instantiate(mapTemplate,mapContentTransform);
            }
            mapObj.name = $"Map_{mapIndex + 1:00}";

            spawnedMaps.Add(mapObj);

            Transform mapTransform = mapObj.transform.Find("Map Transform");

            if (mapTransform == null) continue;

            int startStage = mapIndex * 10 + 1;
            int endStage = Mathf.Min(startStage + 9,totalStages);

            for (int stageNumber = startStage; stageNumber <= endStage; stageNumber++)
            {
                if (!stageCatalog.TryGetStageData(stageNumber, out StageData stageData)) continue;
                GameObject slotObj = Instantiate(stageSlotPrefab,mapTransform);
                StageMapSlot mapSlot = slotObj.GetComponent<StageMapSlot>();

                if (mapSlot == null)
                {
                    Destroy(slotObj);
                    continue;
                }

                RectTransform rectTrans = slotObj.GetComponent<RectTransform>();

                if (rectTrans != null)
                {
                    int index = stageNumber - startStage;
                    int row = index / columns;
                    int col = index % columns;
                    float baseX = col * spacingX;
                    float baseY = -row * spacingY;

                    float randomX = Random.Range(-offsetXRange,offsetXRange);

                    float randomY = Random.Range( -offsetYRange,offsetYRange);

                    rectTrans.anchoredPosition = new Vector2( baseX + randomX,baseY + randomY);
                }
                mapSlot.Init(this, stageNumber, maxCleared);
                spawnedSlots.Add(mapSlot);
            }
        }
    }

    public void RefreshMap()
    {
        if (stageCatalog == null) return;
        if (mapContentTransform == null) return;
        if (stageSlotPrefab == null) return;

        int maxCleared = 0;

        if (PlayerInfo.Instance != null &&
            PlayerInfo.Instance.IsInitialized)
        {
            maxCleared = PlayerInfo.Instance.MaxClearedStage;
        }

        if (!isMapGenerated)
        {
            GenerateMapSlots(maxCleared);

            isMapGenerated = true;
        }
        UpdateMapVisibility(maxCleared);
    }
    private void UpdateMapVisibility(int maxCleared)
    {
        if (spawnedMaps.Count == 0) return;
        int currentMapIndex = maxCleared / 10;

        currentMapIndex = Mathf.Clamp(currentMapIndex,0,spawnedMaps.Count - 1);

        for (int i = 0; i < spawnedMaps.Count; i++)
        {
            if (spawnedMaps[i] == null) continue;

            spawnedMaps[i].SetActive(i == currentMapIndex);
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