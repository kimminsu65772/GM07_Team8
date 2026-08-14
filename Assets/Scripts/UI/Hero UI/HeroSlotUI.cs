using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System;

public class HeroSlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image heroIcon;

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
    public void SetupSlot(HeroEntry entry, HeroSaveData saveData, bool isOwned, Action<HeroEntry, HeroSaveData> onClick)
    {
        currentEntry = entry;
        currentSaveData = saveData;
        heroName = entry.HeroName;
        onClickCallback = onClick;

        if (nameText != null) nameText.text = $"{heroName} (Lv.{saveData.Level})";

        // 클릭 이벤트 연결 
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(() => onClickCallback?.Invoke(currentEntry, currentSaveData));
        }
    }
    public string GetHeroName() => heroName;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentSaveData == null || !currentSaveData.IsOwned) return;

        originalParent = transform.parent;

        if (canvasTransform != null)
        {
            transform.SetParent(canvasTransform);
        }
        canvasGroup.blocksRaycasts = false;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (currentSaveData == null || !currentSaveData.IsOwned) return;

        RectTransform rect = GetComponent<RectTransform>();
        if (rect != null && canvasTransform != null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasTransform as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint
            );
            rect.anchoredPosition = localPoint;
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
}