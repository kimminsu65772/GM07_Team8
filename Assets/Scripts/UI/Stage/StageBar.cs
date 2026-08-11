using UnityEngine;
using UnityEngine.UI;

public class StageBar : MonoBehaviour
{
    private Image fillImage;

    private void Awake()
    {
        fillImage = GetComponent<Image>();
    }
    public void SetProgress(float progress)
    {
        fillImage.fillAmount = Mathf.Clamp01(progress);
    }
}
