using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    [Header("버튼")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button settingsButton;

    [Header("설정창")]
    [SerializeField] private GameObject settingsPanel;

    [Header("씬 이름")]
    [SerializeField] private string inGameSceneName = "InGameScene";
    private void Start()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnClickStart);
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OnClickSettings);
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }
    private void OnClickStart()
    {
        SceneManager.LoadScene(inGameSceneName);
    }
    private void OnClickSettings()
    {
        if (settingsPanel != null)
        {
            bool isActive = settingsPanel.activeSelf;
            settingsPanel.SetActive(!isActive);
        }
    }
}