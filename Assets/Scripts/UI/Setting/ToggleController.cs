using UnityEngine;

public class ToggleController : MonoBehaviour
{
    [Header("토글 UI")]
    [SerializeField] private GameObject targetUI;

    private void Awake()
    {
        if (targetUI == null)
        {
            targetUI = this.gameObject;
        }
    }
    private void Update()
    {
        if (targetUI.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
    }
    // UI 켜고 끄는 함수
    public void Toggle()
    {
        if (targetUI != null)
        {
            bool isActive = targetUI.activeSelf;
            targetUI.SetActive(!isActive);
        }
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
