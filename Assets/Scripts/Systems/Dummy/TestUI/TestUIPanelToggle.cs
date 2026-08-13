using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestUIPanelToggle : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Button toggleButton;
    [SerializeField] private TMP_Text buttonText;

    private void OnEnable()
    {
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(TogglePanels);
        }

        RefreshButtonLabel();
    }

    private void OnDisable()
    {
        if (toggleButton != null)
        {
            toggleButton.onClick.RemoveListener(TogglePanels);
        }
    }

    private void TogglePanels()
    {
        if (panelRoot == null)
        {
            return;
        }

        panelRoot.SetActive(!panelRoot.activeSelf);
        RefreshButtonLabel();
    }

    private void RefreshButtonLabel()
    {
        if (buttonText == null || panelRoot == null)
        {
            return;
        }

        buttonText.text = panelRoot.activeSelf
            ? "Close Tests"
            : "Open Tests";
    }
}
