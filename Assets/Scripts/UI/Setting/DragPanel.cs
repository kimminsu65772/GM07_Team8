using UnityEngine;
using UnityEngine.EventSystems;

public class DragPanel : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    public RectTransform targetPanel;
    public void OnPointerDown(PointerEventData eventData)
    {
        targetPanel.SetAsLastSibling();
    }
    public void OnDrag(PointerEventData eventData)
    {
        targetPanel.anchoredPosition += eventData.delta;
    }
}