using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private FollowCam mainCamera;
    [SerializeField] private StageManager stageManager;
    [SerializeField] private MapController mapController;

    [Header("Start Settings")]
    [SerializeField] private Transform airshipStartPoint;
    [SerializeField] private AirshipController airship;

    private readonly List<GameObject> spawnedHeroes = new List<GameObject>();
    private int currentStage;
    private bool isInitialized;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (isInitialized)
        {
            return;
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main.GetComponent<FollowCam>();
        }

        currentStage = PlayerInfo.Instance.CurrentStage;
        airship.Init();
        SetUpStage(currentStage);
        StartStage();

        isInitialized = true;
    }

    public void SetUpStage(int stageNumber)
    {
        StopStage();
        mainCamera.ResetCameraPosition();
        currentStage = stageNumber;
        mapController.LoadMap(currentStage);
        ResetPlayerPosition();
        PlaceFormationHeroes();
    }

    public void StartStage()
    {
        stageManager.StartStage(currentStage);
    }

    public void StopStage()
    {
        stageManager.StopStage();
    }

    private void ResetPlayerPosition()
    {
        if (airshipStartPoint == null || airship == null)
        {
            Debug.LogError("비행선 시작 위치가 설정되지 않았습니다.");
            return;
        }

        airship.transform.position = airshipStartPoint.position;
        airship.Respawn();
    }

    private void PlaceFormationHeroes()
    {
        ClearSpawnedHeroes();

        if (airship == null)
        {
            Debug.LogError("비행선이 설정되지 않았습니다.");
            return;
        }

        // 비행선 영웅 배치 클래스에서 배치 포인트 가져오기
        AirshipHeroPlacementPoints placementPoints = airship.GetComponent<AirshipHeroPlacementPoints>();

        if (placementPoints == null)
        {
            Debug.LogError("배치 지점이 설정되지 않았습니다.");
            return;
        }

        HeroFormationManager formationManager = HeroFormationManager.Instance;
        if (formationManager == null)
        {
            Debug.LogWarning("영웅 배치 매니저를 찾을 수 없습니다.");
            return;
        }

        PlayerInfo playerInfo = PlayerInfo.Instance;
        formationManager.Initialize();

        PlaceHeroes(formationManager.GetFrontLineSlots(), placementPoints, true);
        PlaceHeroes(formationManager.GetBackLineSlots(), placementPoints, false);
    }

    // 현재 영웅 배치 정보를 받아와서 배치 포인트에 따라 영웅을 배치하는 메서드
    private void PlaceHeroes(IReadOnlyList<HeroFormationRuntimeSlot> slots, AirshipHeroPlacementPoints placementPoints, bool isFront)
    {
        if (slots == null || slots.Count == 0)
        {
            return;
        }

        // 진형 배치에 필요한 수만큼 시작포인트를 배열로 가져옴
        Transform[] startPoints = placementPoints.GetPlacementTransforms(slots.Count, isFront);

        // 실제 배치 가능한 슬롯 수와  시작 포인트 수 중 최소값을 사용하여 배치
        int placeCount = Mathf.Min(slots.Count, startPoints.Length);
        for (int i = 0; i < placeCount; i++)
        {
            HeroFormationRuntimeSlot slot = slots[i];
            Transform startPoint = startPoints[i];

            // 슬롯이 유효하지 않거나 매핑되는 영웅 데이터가 없거나 영웅 프리팹이 없는 경우 건너뜀
            if (slot == null ||
                slot.HeroEntry == null ||
                slot.HeroEntry.HeroPrefab == null ||
                startPoint == null)
            {
                continue;
            }

            GameObject spawnedHero = Instantiate(
                slot.HeroEntry.HeroPrefab,
                startPoint.position,
                startPoint.rotation,
                startPoint
            );

            spawnedHeroes.Add(spawnedHero);
        }
    }

    private void ClearSpawnedHeroes()
    {
        for (int i = spawnedHeroes.Count - 1; i >= 0; i--)
        {
            if (spawnedHeroes[i] != null)
            {
                Destroy(spawnedHeroes[i]);
            }
        }

        spawnedHeroes.Clear();
    }
}
