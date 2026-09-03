using UnityEngine;

public class ToggleController : MonoBehaviour
{
    [Header("토글 UI")]
    [SerializeField] private GameObject targetUI;

    [Header("최초 1회 제한 설정")]
    [SerializeField] private bool runOnlyOnce = false;

    private void Awake()
    {
        if (targetUI == null)
        {
            targetUI = this.gameObject;
        }
    }
    // UI 켜고 끄는 함수
    public void Toggle()
    {
        if (targetUI == null) return;

        bool isActive = targetUI.activeSelf;

        if (runOnlyOnce && !isActive)
        {
            if (PlayerInfo.Instance != null)
            {
                if (PlayerInfo.Instance.AlreadyShowedArrangeHeroTutorial) return;
                PlayerInfo.Instance.SetAlreadyShowedArrangeHeroTutorial();
            }
        }
        targetUI.SetActive(!isActive);
    }
    // 켜기
    public void Open()
    {
        if (targetUI != null)
        {
            targetUI.SetActive(true);
        }
    }
    // 끄기
    public void Close()
    {
        if (targetUI != null)
        {
            targetUI.SetActive(false);
        }
    }
}
