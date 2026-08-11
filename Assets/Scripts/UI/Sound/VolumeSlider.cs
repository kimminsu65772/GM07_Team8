using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeSlider : MonoBehaviour
{
    private Slider volumSlider;

    private void Awake()
    {
        volumSlider = GetComponent<Slider>();   
    }
    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("GameVolume", 1f);
        volumSlider.value = savedVolume;

        volumSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    private void OnVolumeChanged(float value)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetVolume(value);
        }
        else
        {
            AudioListener.volume = value;
            PlayerPrefs.SetFloat("GameVolume", value);
            PlayerPrefs.Save();
        }
    }
}
