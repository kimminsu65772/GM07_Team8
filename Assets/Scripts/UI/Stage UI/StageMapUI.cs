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

    private void Start()
    {
        GenerateMapSlots();
    }
    private void GenerateMapSlots()
    {
        if (stageCatalog == null)
        {
            Debug.LogError("StageMapUI: StageCatalog가 연결되지 않았습니다.");
            return;
        }
        foreach (var slot in spawnedSlots)
        {
            if (slot != null) Destroy(slot.gameObject);
        }
        spawnedSlots.Clear();
        int totalStages = stageCatalog.StageCount;
        for (int i = 1; i <= totalStages; i++)
        {
            if (stageCatalog.TryGetStageData(i, out StageData stageData))
            {
                GameObject slotObj = Instantiate(stageSlotPrefab, mapContentTransform);
                StageMapSlot mapSlot = slotObj.GetComponent<StageMapSlot>();

                if (mapSlot != null)
                {
                    RectTransform rectTrans = slotObj.GetComponent<RectTransform>();
                    if (rectTrans != null)
                    {
                        int index = i - 1;
                        int row = index / columns;
                        int col = index % columns;

                        float baseX = col * spacingX;
                        float baseY = -row * spacingY; 

                        float randomX = Random.Range(-offsetXRange, offsetXRange);
                        float randomY = Random.Range(-offsetYRange, offsetYRange);

                        rectTrans.anchoredPosition = new Vector2(baseX + randomX, baseY + randomY);
                    }

                    int maxCleared = (PlayerInfo.Instance != null && PlayerInfo.Instance.IsInitialized)
                        ? PlayerInfo.Instance.MaxClearedStage
                        : 0;

                    mapSlot.Init(this, i, maxCleared);
                    spawnedSlots.Add(mapSlot);
                }
            }
        }
    }
    public void RefreshMap()
    {
        int maxCleared = (PlayerInfo.Instance != null && PlayerInfo.Instance.IsInitialized)
            ? PlayerInfo.Instance.MaxClearedStage
            : 0;

        foreach (var slot in spawnedSlots)
        {
            slot.RefreshState(maxCleared);
        }
    }
    public void OnStageSelected(int stageNumber)
    {
        if (PlayerInfo.Instance != null)
        {
            bool success = PlayerInfo.Instance.TrySetCurrentStage(stageNumber);
            if (success)
            {
                Debug.Log($"스테이지 {stageNumber} 선택 완료!");
                stageTransitionController.StartTransition(stageNumber);
            }
            else
            {
                Debug.LogWarning($"스테이지 {stageNumber} 선택 실패: 아직 도전할 수 없는 스테이지이거나 잘못된 요청입니다.");
            }
        }
    }
}