using UnityEngine;
using UnityEngine.UI;

public class ResetButton : MonoBehaviour
{
    private Button resetButton;

    private void Awake()
    {
        gameObject.TryGetComponent<Button>(out Button button);
        resetButton = button;
    }

    private void OnEnable()
    {
        if (resetButton == null) return;

        resetButton.onClick.RemoveListener(OnResetButtonClicked);
        resetButton.onClick.AddListener(OnResetButtonClicked);
    }

    private void OnDisable()
    {
        if (resetButton == null) return;

        resetButton.onClick.RemoveListener(OnResetButtonClicked);
    }

    private void OnResetButtonClicked()
    {
        if (PlayerInfo.Instance != null)
        {
            PlayerInfo.Instance.ResetData();
        }
    }
}
