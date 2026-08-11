using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Slider))]
public class BGMSlider : MonoBehaviour
{
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }
    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("BGM_Volume", 1f);
        slider.value = savedVolume;

        slider.onValueChanged.AddListener(OnSliderChanged);
    }
    private void OnSliderChanged(float value)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetBGMVolume(value);
        }
    }
}
