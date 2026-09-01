using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
public class HeroSlotUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image heroIcon;

    [Header("영웅 배치시 사용할 UI 요소 설정")]
    [SerializeField] private Image formationOverlayImage;
    [SerializeField] private TMP_Text formationStateText;

    [Header("화살표 효과 설정")]
    [SerializeField] private GameObject arrowEffectObject; // 화살표 UI 오브젝트 연결
    private Coroutine arrowAnimationCoroutine;

    private HeroNameEnum heroId;
    private string heroName;
    private HeroEntry currentEntry;
    private HeroSaveData currentSaveData;
    private Action<HeroEntry, HeroSaveData> onClickCallback;

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
    public void SetArrowEffect(bool isActive)
    {
        if (arrowEffectObject == null) return;

        arrowEffectObject.SetActive(isActive);

        if (isActive)
        {
            if (arrowAnimationCoroutine != null) StopCoroutine(arrowAnimationCoroutine);
            arrowAnimationCoroutine = StartCoroutine(AnimateArrow());
        }
        else
        {
            if (arrowAnimationCoroutine != null)
            {
                StopCoroutine(arrowAnimationCoroutine);
                arrowAnimationCoroutine = null;
            }
        }
    }
    private IEnumerator AnimateArrow()
    {
        RectTransform rectTrans = arrowEffectObject.GetComponent<RectTransform>();
        if (rectTrans == null) yield break;

        Vector2 originalPos = rectTrans.anchoredPosition;
        float speed = 6f;
        float distance = 8f; // 움직이는 범위 (픽셀)

        while (true)
        {
            float pingPong = Mathf.Sin(Time.time * speed) * distance;
            rectTrans.anchoredPosition = originalPos + new Vector2(0, pingPong);
            yield return null;
        }
    }
}