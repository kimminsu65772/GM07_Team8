using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class HeroInventoryUIController : MonoBehaviour
{
    [Header("UI 연결 (아래쪽 리스트)")]
    [SerializeField] private Transform contentGrid;
    [SerializeField] private GameObject heroSlotPrefab;
    [SerializeField] private HeroCatalog heroCatalog;

    //[Header("상세 정보 UI 연결 (위쪽 패널)")]
    //[SerializeField] private GameObject detailPanelRoot;     
    //[SerializeField] private TextMeshProUGUI detailNameText;   
    //[SerializeField] private TextMeshProUGUI detailDescText;

    [Header("영웅 상세 정보 UI 연결")]
    [SerializeField] private Image heroIcon;
    [SerializeField] private Image skillIcon;
    [SerializeField] private TMP_Text heroName;
    [SerializeField] private TMP_Text heroLevel;
    [SerializeField] private TMP_Text heroLocation;


    private readonly List<HeroSlotUI> heroSlotPool = new();

    private PlayerInfo playerInfo;

    private void Awake()
    {
        if (contentGrid == null)
        {
            Debug.LogError("Content Grid가 설정되지 않았습니다. 인스펙터에서 연결해주세요.");
            return;
        }
        if (heroSlotPrefab == null)
        {
            Debug.LogError("Hero Slot Prefab이 설정되지 않았습니다. 인스펙터에서 연결해주세요.");
            return;
        }
        if (heroCatalog == null)
        {
            Debug.LogError("Hero Catalog가 설정되지 않았습니다. 인스펙터에서 연결해주세요.");
            return;
        }

        playerInfo = PlayerInfo.Instance;
    }

    private void Start()
    {
        RefreshInventory();
        ClearDetailInfo();
    }
    //인벤토리 목록 생성 및 갱신
    public void RefreshInventory()
    {
        if (playerInfo == null)
        {
            playerInfo = PlayerInfo.Instance;
        }

        if (playerInfo == null)
        {
            Debug.LogError("PlayerInfo 인스턴스를 찾을 수 없습니다.");
            return;
        }

        List<(HeroEntry entry, HeroSaveData heroSaveData)> IngameHeroList = GetIngameHeroes();

        IngameHeroList.Sort((a, b) => b.heroSaveData.IsOwned.CompareTo(a.heroSaveData.IsOwned));

        if (!PrepareHeroSlotPool(IngameHeroList.Count))
        {
            Debug.LogError("영웅 슬롯 풀을 준비하는 데 실패했습니다.");
            return;
        }

        for (int i = 0; i < IngameHeroList.Count; i++)
        {
            (HeroEntry entry, HeroSaveData heroSaveData) hero = IngameHeroList[i];
            heroSlotPool[i].SetupSlot(hero.entry, hero.heroSaveData, hero.heroSaveData.IsOwned, ShowDetailInfo);
            heroSlotPool[i].SetFormationState(false);
            heroSlotPool[i].SetDragEnabled(false);
        }
    }
    //영웅 상세 정보 표시
    //private void ShowDetailInfo(HeroEntry entry, HeroSaveData saveData)
    //{
    //    if (detailPanelRoot != null) detailPanelRoot.SetActive(true);
    //    if (detailNameText != null)
    //    {
    //        detailNameText.text = $"{entry.HeroName} (Lv.{saveData.Level})";
    //    }
    //    if (detailDescText != null)
    //    {      
    //        detailDescText.text = $"영웅 위치: {entry.HeroLocation}\n소유 여부: {(saveData.IsOwned ? "보유 중" : "미보유")}";
    //    }
    //}
    ////상세 정보 초기화
    //private void ClearDetailInfo()
    //{
    //    if (detailNameText != null) detailNameText.text = "영웅을 선택해주세요.";
    //    if (detailDescText != null) detailDescText.text = "";
    //}

    private void ShowDetailInfo(HeroEntry entry, HeroSaveData saveData)
    {
        if (heroIcon != null)
        {
            heroIcon.gameObject.SetActive(true);
            heroIcon.sprite = entry.HeroIcon;
        }
        if (heroName != null)
        {
            heroName.text = entry.HeroName;
        }
        if (heroLevel != null)
        {
            heroLevel.text = $"Lv. {saveData.Level}";
        }
        if (heroLocation != null)
        {
            heroLocation.text = $"위치: {entry.HeroLocation}";
        }
        if (skillIcon != null)
        {
            skillIcon.gameObject.SetActive(true);
        }
    }

    private void ClearDetailInfo()
    {
        if (heroIcon != null)
        {
            heroIcon.gameObject.SetActive(false);
            heroIcon.sprite = null;
        }
        if (skillIcon != null)
        {
            skillIcon.gameObject.SetActive(false);
            skillIcon.sprite = null;
        }
        if (heroName != null)
        {
            heroName.text = "영웅을 선택해주세요.";
        }
        if (heroLevel != null)
        {
            heroLevel.text = "";
        }
        if (heroLocation != null)
        {
            heroLocation.text = "";
        }
    }

    private List<(HeroEntry entry, HeroSaveData heroSaveData)> GetIngameHeroes()
    {
        if (playerInfo == null)
        {
            playerInfo = PlayerInfo.Instance;
        }

        List<(HeroEntry entry, HeroSaveData heroSaveData)> Heroes = new();

        foreach (HeroEntry entry in playerInfo.HeroEntries)
        {
            if (entry == null)
            {
                Debug.LogWarning("HeroEntry가 null입니다. PlayerInfo의 HeroEntries를 확인하세요.");
                continue;
            }

            if (playerInfo.TryGetHeroData(entry.HeroId, out HeroSaveData heroSaveData))
            {
                Heroes.Add((entry, heroSaveData));
            }
        }

        return Heroes;
    }

    private bool PrepareHeroSlotPool(int requiredCount)
    {

        if (heroSlotPrefab == null || contentGrid == null)
        {
            Debug.LogError("영웅 슬롯 프리팹 또는 영웅 리스트 Content가 설정되지 않았습니다.");
            return false;
        }

        // 필요한 슬롯 수만큼 풀을 준비
        while (heroSlotPool.Count < requiredCount)
        {
            GameObject newSlot = Instantiate(heroSlotPrefab, contentGrid);
            HeroSlotUI slotUI = newSlot.GetComponent<HeroSlotUI>();

            if (slotUI == null)
            {
                Debug.LogError("HeroSlotPrefab에 HeroSlotUI 컴포넌트가 없습니다.");
                Destroy(newSlot);
                return false;
            }

            heroSlotPool.Add(slotUI);
        }
        // 사용하지 않는 슬롯은 비활성화
        for (int i = 0; i < heroSlotPool.Count; i++)
        {
            heroSlotPool[i].gameObject.SetActive(i < requiredCount);
        }

        return true;
    }
}