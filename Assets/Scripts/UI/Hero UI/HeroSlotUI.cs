using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System;

public class HeroSlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image heroIcon;

    [Header("영웅 배치시 사용할 UI 요소 설정")]
    [SerializeField] private Image formationOverlayImage;
    [SerializeField] private TMP_Text formationStateText;

    [Header("영웅 배치 슬롯 사용 구분")]
    [SerializeField] private bool canDrag = true;

    private string heroName;
    private HeroEntry currentEntry;
    private HeroSaveData currentSaveData;
    private Action<HeroEntry, HeroSaveData> onClickCallback;

    // 드래그 관련 변수
    private Transform originalParent;
    private CanvasGroup canvasGroup;
    private Transform canvasTransform;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        Canvas rootCanvas = GetComponentInParent<Canvas>();
        if (rootCanvas != null)
        {
            canvasTransform = rootCanvas.rootCanvas.transform;
        }
    }
    public void SetupSlot(HeroEntry entry, HeroSaveData saveData, bool isOwned, Action<HeroEntry, HeroSaveData> onClick = null)
    {
        currentEntry = entry;
        currentSaveData = saveData;
        heroName = entry.HeroName;
        onClickCallback = onClick;

        if (nameText != null) nameText.text = $"{heroName} (Lv.{saveData.Level})";

        if (heroIcon != null)
        {
            heroIcon.sprite = entry.HeroIcon;
            heroIcon.color = isOwned ? Color.white : Color.gray; // 소유 여부에 따라 색상 변경
        }

        // 클릭 이벤트 연결 
        Button btn = GetComponent<Button>();
   
        if (btn != null)
        {
            // 기존의 클릭 이벤트를 먼저 정리한다.
            btn.onClick.RemoveAllListeners();

            // 만약에 전달해야할 콜백이 있다면 연결한다.
            if (onClickCallback != null)
            {
                btn.onClick.AddListener(() => onClickCallback.Invoke(currentEntry, currentSaveData));
            }
        }
    }
    public string GetHeroName() => heroName;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentSaveData == null || !currentSaveData.IsOwned || !canDrag) return;

        originalParent = transform.parent;

        if (canvasTransform != null)
        {
            transform.SetParent(canvasTransform);
        }
        canvasGroup.blocksRaycasts = false;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (currentSaveData == null || !currentSaveData.IsOwned || !canDrag) return;

        RectTransform rect = GetComponent<RectTransform>();
        if (rect != null && canvasTransform != null)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                canvasTransform as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector3 worldPoint
            );
            rect.position = worldPoint;
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (currentSaveData == null || !currentSaveData.IsOwned) return;
        canvasGroup.blocksRaycasts = true;

        if (transform.parent == canvasTransform)
        {
            transform.SetParent(originalParent);
            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
    }

    public void SetFormationState(bool isInFormation)
    {
        if (formationOverlayImage != null)
        {
            formationOverlayImage.gameObject.SetActive(isInFormation);
        }

        if (formationStateText != null)
        {
            formationStateText.gameObject.SetActive(isInFormation);
            formationStateText.text = isInFormation ? "배치중" : string.Empty;
        }
    }

    public void SetDragEnabled(bool enabled)
    {
        canDrag = enabled;
    }
}