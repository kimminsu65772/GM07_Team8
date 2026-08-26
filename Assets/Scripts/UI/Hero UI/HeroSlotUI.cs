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

    private HeroNameEnum heroId;
    private string heroName;
    private HeroEntry currentEntry;
    private HeroSaveData currentSaveData;
    private Action<HeroEntry, HeroSaveData> onClickCallback;

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
        heroId = entry.HeroId;
        heroName = entry.HeroName;
        onClickCallback = onClick;

        if (nameText != null) nameText.text = heroName;

        if (heroIcon != null)
        {
            heroIcon.sprite = entry.HeroIcon;
            heroIcon.color = isOwned ? Color.white : Color.gray; 
        }
        Button btn = GetComponent<Button>();
   
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();

            if (onClickCallback != null)
            {
                btn.onClick.AddListener(() => onClickCallback.Invoke(currentEntry, currentSaveData));
            }
        }
    }
    public HeroNameEnum GetHeroId() => heroId;
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