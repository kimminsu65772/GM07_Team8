using UnityEngine;
using UnityEngine.UI;
public class ScrollReset : MonoBehaviour
{
    void Start()
    {
        // 스크롤 뷰 컴포넌트를 가져와서
        ScrollRect scrollRect = GetComponent<ScrollRect>();
        // 가로 스크롤 위치를 0(가장 왼쪽)으로 강제 설정합니다.
        scrollRect.horizontalNormalizedPosition = 0f;
    }
}