using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("호버 시스템")]
    [SerializeField] private float hoverScale = 1.15f; //마우스 커지는 배율
    [SerializeField] private float duration = 0.2f; //애니메이션 진행되는 시간
    [SerializeField] private Ease easeType = Ease.OutBack;

    private Vector3 originalScale;
    private void Awake()
    {
        //버튼의 처음 크기 저장
        originalScale = transform.localScale;
    }
    //마우스가 버튼 위에 올라갔을 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(originalScale * hoverScale, duration).SetEase(easeType).SetUpdate(true);
    }
    //마우스가 버튼에서 나왔을 때
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(originalScale, duration).SetEase(Ease.OutQuad).SetUpdate(true);
    }
    private void OnDisable()
    {
        transform.DOKill();
        transform.localScale = originalScale;
    }
}
