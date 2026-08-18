using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private FollowCam mainCamera;
    [SerializeField] private StageManager stageManager;
    [SerializeField] private MapController mapController;

    [Header("Start Settings")]
    [SerializeField] private Transform airshipStartPoint;
    [SerializeField] private AirshipController airship;
    
    private static BattleManager instance;

    public static BattleManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<BattleManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("BattleManager");
                    instance = obj.AddComponent<BattleManager>();
                }
            }
            return instance;
        }
    }

    public AirshipController Airship => airship;

    private readonly List<GameObject> spawnedHeroes = new List<GameObject>();
    private readonly Dictionary<string, GameObject> heroCache = new Dictionary<string, GameObject>();
    private int currentStage;
    private bool isInitialized;

    private HeroFormationManager heroFormationManager;

    private void OnEnable()
    {
        heroFormationManager = HeroFormationManager.Instance;

        if (heroFormationManager != null)
        {
            heroFormationManager.OnFormationChanged -= HandleFormationChanged;
            heroFormationManager.OnFormationChanged += HandleFormationChanged;
        }
    }

    private void OnDisable()
    {
        if (heroFormationManager != null)
        {
            heroFormationManager.OnFormationChanged -= HandleFormationChanged;
        }
    }

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
        if (stageNumber < 1 || stageNumber > stageManager.LastStage)
        {
            Debug.LogError($"유효하지 않은 스테이지입니다: {stageNumber}");
            return;
        }

        if (!PlayerInfo.Instance.TrySetCurrentStage(stageNumber))
        {
            Debug.LogError($"설정할 수 없는 스테이지입니다: {stageNumber}");
            return;
        }

        StopStage();
        mainCamera.ResetCameraPosition();
        currentStage = PlayerInfo.Instance.CurrentStage;
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
        Profiler.BeginSample("BattleManager.PlaceFormationHeroes");

        try
        {
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

            if (heroFormationManager == null)
            {
                Debug.LogWarning("영웅 배치 매니저를 찾을 수 없습니다.");
                return;
            }

            DeactivateSpawnedHeroes();

            PlaceHeroes(heroFormationManager.GetFrontLineSlots(), placementPoints, true);
            PlaceHeroes(heroFormationManager.GetBackLineSlots(), placementPoints, false);
        }
        finally
        {
            Profiler.EndSample();
        }
    }

    // 현재 영웅 배치 정보를 받아와서 배치 포인트에 따라 영웅을 배치하는 메서드
    private void PlaceHeroes(
        IReadOnlyList<HeroFormationRuntimeSlot> slots, 
        AirshipHeroPlacementPoints placementPoints, 
        bool isFront)
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

            GameObject spawnedHero = GetCachedHeroOrCreate(slot);

            spawnedHero.transform.SetParent(startPoint, false);
            spawnedHero.transform.SetPositionAndRotation(startPoint.position, startPoint.rotation);
            spawnedHero.SetActive(true);

            Hero hero = spawnedHero.GetComponent<Hero>();

            spawnedHeroes.Add(spawnedHero);
        }
    }
    private void DeactivateSpawnedHeroes()
    {
        for (int i = 0; i < spawnedHeroes.Count; i++)
        {
            if (spawnedHeroes[i] != null)
            {
                Hero hero = spawnedHeroes[i].GetComponent<Hero>();
                spawnedHeroes[i].SetActive(false);
            }
        }
        spawnedHeroes.Clear();
    }

    private GameObject GetCachedHeroOrCreate(HeroFormationRuntimeSlot slot)
    {
        if (heroCache.TryGetValue(slot.HeroName, out GameObject cachedHero))
        {
            return cachedHero;
        }

        Profiler.BeginSample("BattleManager.CacheMiss.InstantiateHero");

        try
        {
            GameObject newHero = Instantiate(slot.HeroEntry.HeroPrefab);
            heroCache.Add(slot.HeroName, newHero);
            return newHero;
        }
        finally
        {
            Profiler.EndSample();
        }
    }

    private void HandleFormationChanged()
    {
        PlaceFormationHeroes();
    }
}
