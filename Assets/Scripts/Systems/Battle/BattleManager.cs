using UnityEngine;

public class BattleManager : MonoBehaviour
{    
    [SerializeField] private MapCatalog mapCatalog;
    [SerializeField] private Transform mapRoot;
    [SerializeField] private StageTestManager stageTestManager;

    [Header("시작점 설정")]
    [SerializeField] private Transform airshipStartPoint;
    [SerializeField] private Transform[] meleeHeroStartPoint;
    [SerializeField] private AirshipController airship;

    private GameObject currentMap;

    private int currentStage;

    public void Initialize()
    {
        if (StageProgressManager.Instance == null)
        {
            Debug.LogError("StageProgressManager 인스턴스가 존재하지 않습니다.");
            return;
        }
        currentStage = StageProgressManager.Instance.CurrentStage;
        SetUpStage(currentStage);
        StartStage();
    }

    public void SetUpStage(int stageNumber)
    {
        currentStage = stageNumber;
        LoadMap(currentStage);
        ResetPlayerPosition();
        
    }

    public void StartStage()
    {
        stageTestManager.StartStage(currentStage);
    }

    private void ResetPlayerPosition()
    {
        if (airshipStartPoint == null || meleeHeroStartPoint == null || meleeHeroStartPoint.Length == 0)
        {
            Debug.LogError("시작점이 설정되지 않았습니다.");
            return;
        }

        // 비행선 위치 초기화
        airship.transform.position = airshipStartPoint.position;
        airship.Respawn();

        // 근접 영웅 위치 초기화
        // TODO: 배치 리스트를 보고 적절한 위치에 배치할 수 있도록 해야함.

    }

    private void LoadMap(int currentStage)
    {
        if (mapCatalog == null)
        {
            Debug.LogError("MapCatalog가 할당되지 않았습니다.");
            return;
        }
        GameObject mapPrefab = mapCatalog.GetMapPrefab(currentStage);
        if (mapPrefab == null)
        {
            Debug.LogError($"Stage {currentStage}에 대한 MapPrefab이 존재하지 않습니다.");
            return;
        }
        if (currentMap != null)
        {
            Destroy(currentMap);
        }
        currentMap = Instantiate(mapPrefab, mapRoot, false);
    }
}
