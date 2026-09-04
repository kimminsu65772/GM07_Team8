using System.Collections.Generic;
using UnityEngine;

public class StageMapUI : MonoBehaviour
{
    [Header("Data Reference")]
    [SerializeField] private StageCatalog stageCatalog;

    [Header("스테이지 변경자 참조")]
    [SerializeField] private StageTransitionController stageTransitionController;

    [Header("UI References")]
    [Tooltip("현재 Stage Panel의 MapTransform")]
    [SerializeField] private Transform mapTransform;

    [Tooltip("스테이지 버튼 프리팹")]
    [SerializeField] private GameObject stageSlotPrefab;

    [Header("Map Settings")]
    [Tooltip("현재 지역 번호")]
    [SerializeField] private int mapNumber = 1;

    [Tooltip("지역 하나당 스테이지 개수")]
    [SerializeField] private int stagesPerMap = 10;

    [Header("ZigZag Settings")]
    [Tooltip("좌우 간격")]
    [SerializeField] private float zigzagX = 130f;

    [Tooltip("스테이지 사이의 세로 간격")]
    [SerializeField] private float spacingY = 80f;

    private readonly List<StageMapSlot> spawnedSlots = new();
    private void OnEnable()
    {
        RefreshMap();
    }
    public void RefreshMap()
    {
        if (stageCatalog == null)
        {
            return;
        }
        if (mapTransform == null)
        {
            return;
        }
        if (stageSlotPrefab == null)
        {
            return;
        }
        int maxCleared = 0;

        if (PlayerInfo.Instance != null && PlayerInfo.Instance.IsInitialized)
        {
            maxCleared = PlayerInfo.Instance.MaxClearedStage;
        }
        GenerateStageButtonsIfNeeded(maxCleared);
    }
    private void GenerateStageButtonsIfNeeded(int maxCleared)
    {
        int totalStages = stageCatalog.StageCount;
        if (mapTransform == null) return;

        if (mapTransform.childCount > 0)
        {
            UpdateExistingStageButtons(mapTransform, maxCleared);
            return;
        }

        int startStage = GetStartStage();
        int endStage = Mathf.Min(startStage + stagesPerMap - 1,totalStages);
      

        for (int stageNumber = startStage; stageNumber <= endStage; stageNumber++)
        {
            if (!stageCatalog.TryGetStageData( stageNumber, out StageData stageData))
            {
                continue;
            }

            GameObject slotObj = Instantiate(stageSlotPrefab, mapTransform);

            slotObj.name = $"StageButton_{stageNumber:00}";

            StageMapSlot mapSlot = slotObj.GetComponent<StageMapSlot>();

            if (mapSlot == null)
            {
                Destroy(slotObj);
                continue;
            }
            SetZigZagPosition(slotObj, stageNumber - startStage);

            mapSlot.Init(this, stageNumber, maxCleared);
            spawnedSlots.Add(mapSlot);
        }
    }
    private int GetStartStage()
    {
        return (mapNumber - 1) * stagesPerMap + 1;
    }
    private void SetZigZagPosition(GameObject slotObj,int index)
    {
        RectTransform rect = slotObj.GetComponent<RectTransform>();
        if (rect == null) return;
        float x;

        if (index % 2 == 0)
        {
            x = -zigzagX;
        }
        else
        {
            x = zigzagX;
        }

        float y = index * spacingY;
        rect.anchoredPosition = new Vector2(x, y);
    }

    private void UpdateExistingStageButtons(Transform mapTransform, int maxCleared)
    {
        foreach (Transform child in mapTransform)
        {
            StageMapSlot mapSlot = child.GetComponent<StageMapSlot>();

            if (mapSlot == null) continue;

            mapSlot.Refresh(maxCleared);

            if (!spawnedSlots.Contains(mapSlot))
            {
                spawnedSlots.Add(mapSlot);
            }
        }
    }

    public void OnStageSelected(int stageNumber)
    {
        if (PlayerInfo.Instance == null) return;
        bool success = PlayerInfo.Instance.TrySetCurrentStage(stageNumber);

        if (success)
        {
            if (stageTransitionController != null)
            {
                stageTransitionController.StartTransition(stageNumber);
            }
        }
    }
}