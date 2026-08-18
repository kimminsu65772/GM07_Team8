using UnityEngine;
using UnityEngine.UI;
public class QuitButton : MonoBehaviour
{
    [SerializeField] private Button quitButton;

    private void Start()
    {
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnClickQuit);
        }
    }

    private void OnClickQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}