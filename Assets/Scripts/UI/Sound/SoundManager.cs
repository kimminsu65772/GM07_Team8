using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("오디오 관리")]
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioSource bgmAudioSource;

    private float masterVolume = 1f;
    private float sfxVolume = 1f;
    private float bgmVolume = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (sfxAudioSource ==null)
        {
            sfxAudioSource = gameObject.AddComponent<AudioSource>();
        }
        sfxAudioSource.spatialBlend = 0f;
        sfxAudioSource.playOnAwake = false;

        if (bgmAudioSource ==null)
        {
            bgmAudioSource = gameObject.AddComponent<AudioSource>();
        }
        bgmAudioSource.spatialBlend = 0f;
        bgmAudioSource.playOnAwake = false;
        bgmAudioSource.loop = true;

        masterVolume = PlayerPrefs.GetFloat("GameVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFX_Volume", 1f);
        bgmVolume = PlayerPrefs.GetFloat("BGM_Volume", 1f);

        if (bgmAudioSource != null)
        {
            bgmAudioSource.volume = bgmVolume * masterVolume;
        }
    }
    //효과음 재생 함수
    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (clip != null && sfxAudioSource != null)
        {
            sfxAudioSource.PlayOneShot(clip, volume * sfxVolume * masterVolume);
        }
    }
    //배경음악 재생 함수
    public void PlayBGM(AudioClip bgmClip)
    {
        if (bgmClip == null) return;
        if (bgmAudioSource.clip == bgmClip && bgmAudioSource.isPlaying)
        {
            bgmAudioSource.volume = bgmVolume * masterVolume;
        }

        bgmAudioSource.clip = bgmClip;
        bgmAudioSource.volume = bgmVolume * masterVolume;
        bgmAudioSource.Play();
    }
    //효과음 볼륨 조절
    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        PlayerPrefs.SetFloat("SFX_Volume", volume);
        PlayerPrefs.Save();
    }
    //배경음악 볼륨 조절
    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        if (bgmAudioSource != null)
        {
            bgmAudioSource.volume = bgmVolume * masterVolume;
        }
        PlayerPrefs.SetFloat("BGM_Volume", volume);
        PlayerPrefs.Save();
    }
    public void SetVolume(float volume)
    {
        masterVolume = volume;

        if (bgmAudioSource != null)
        {
            bgmAudioSource.volume = bgmVolume * masterVolume;
        }

        PlayerPrefs.SetFloat("GameVolume", volume);
        PlayerPrefs.Save();
    }
}
