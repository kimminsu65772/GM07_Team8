using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    [Header("버튼")]
    [SerializeField] private Button startButton; //탐험 시작 버튼
    [SerializeField] private Button settingsButton; //설정 버튼
    [SerializeField] private Button quitButton; //게임 종료 버튼

    [Header("설정창")]
    [SerializeField] private GameObject settingsPanel; //설정창 오브젝트

    [Header("씬 이름")]
    [SerializeField] private string inGameSceneName = "InGameScene";//이동할 게임씬

    private void Start()
    {
        //버튼 클릭 이벤트 연결
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnClickStart);
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OnClickSettings);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnClickQuit);
        }

        //시작시 설정창 꺼두기
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    //탐험 시작 버튼 클릭시
    private void OnClickStart()
    {
        //사운드 재생 페이드 아웃 추후 추가*****

        //인게임 씬으로 전환
        SceneManager.LoadScene(inGameSceneName);
    }

    //설정 버튼 클릭시
    private void OnClickSettings()
    {
        if (settingsPanel != null)
        {
            bool isActive = settingsPanel.activeSelf;
            settingsPanel.SetActive(!isActive);
        }
    }

    //게임 종료 버튼 클릭시
    private void OnClickQuit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
