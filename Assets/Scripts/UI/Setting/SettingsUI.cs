using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [Header("컨트롤러")]
    [SerializeField] private ToggleController settingsToggle;

    [Header("버튼")]
    [SerializeField] private Button closeButton;


    void Start()
    {
        if (closeButton != null && settingsToggle != null)
        {
            closeButton.onClick.AddListener(settingsToggle.Toggle);
        }    
    }
}
