using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("오디오 관리")]
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioSource bgmAudioSource;

    [Header("오디오 테이블")]
    [SerializeField] private SoundTableSO soundTable;

    [Header("BGM 전환 시간")]
    [SerializeField] private float bgmFadeDuration = 1f;
    
    
    private float masterVolume = 1f;
    private float sfxVolume = 1f;
    private float bgmVolume = 1f;

    private Dictionary<SoundId, SoundData> soundDictTable = new();
    private Dictionary<SoundId, float> lastPlayedTime = new();
    private Dictionary<SoundId, int> playingCount = new();

    private Coroutine bgmFadeCoroutine;

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

        masterVolume = PlayerPrefs.GetFloat("GameVolume", 0.5f);
        sfxVolume = PlayerPrefs.GetFloat("SFX_Volume", 0.5f);
        bgmVolume = PlayerPrefs.GetFloat("BGM_Volume", 0.5f);

        if (bgmAudioSource != null)
        {
            bgmAudioSource.volume = bgmVolume * masterVolume;
        }

        InitializeSoundTable();
    }
    //효과음 재생 함수
    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (clip != null && sfxAudioSource != null)
        {
            sfxAudioSource.PlayOneShot(clip, volume * sfxVolume * masterVolume);
        }
    }

    public void PlaySound(SoundId soundId, float volumeMultiplier = 1f)
    {
        if (sfxAudioSource == null)
        {
            Debug.LogWarning("sfxAudioSource가 없습니다.");
            return;
        }

        if (!soundDictTable.TryGetValue(soundId, out SoundData soundData) || soundData.Clip == null)
        {
            Debug.LogWarning($"SoundId {soundId}에 해당하는 SoundData가 없습니다.");
            return;
        }

        if (!CanPlaySound(soundData)) return;

        float pitch = Random.Range(soundData.PitchRange.x, soundData.PitchRange.y);
        float volume = soundData.Volume * volumeMultiplier * sfxVolume * masterVolume;

        sfxAudioSource.pitch = pitch;
        sfxAudioSource.PlayOneShot(soundData.Clip, volume);
        sfxAudioSource.pitch = 1f;

        float duration = soundData.Clip.length / pitch;
        StartCoroutine(ReleasePlayingCount(soundData.Id, duration));
    }

    private bool CanPlaySound(SoundData soundData)
    {
        SoundId soundId = soundData.Id;

        // 쿨타임이 다 돌았는지 체크하고 재생가능하면 쿨타임 갱신
        if (soundData.Cooldown > 0f &&
            lastPlayedTime.TryGetValue(soundId, out float lastTime))
        {
            if (Time.unscaledTime - lastTime < soundData.Cooldown)
            {
                return false;
            }
        }

        lastPlayedTime[soundId] = Time.unscaledTime;

        // 동시에 재생 가능한 최대 개수 체크
        int currentCount = playingCount.TryGetValue(soundId, out int count) ? count : 0;

        if (currentCount >= soundData.MaxSimultaneous)
        {
            return false;
        }

        playingCount[soundId] = currentCount + 1;

        return true;
    }

    private IEnumerator ReleasePlayingCount(SoundId soundId, float cooldown)
    {
        yield return new WaitForSeconds(cooldown);

        if (!playingCount.TryGetValue(soundId, out int count))
        {
            yield break;
        }

        count--;

        if (count <= 0)
        {
            playingCount.Remove(soundId);
        }
        else
        {
            playingCount[soundId] = count;
        }
    }

    //배경음악 재생 함수
    public void PlayBGM(AudioClip bgmClip)
    {
        if (bgmClip == null) return;
        if (bgmAudioSource.clip == bgmClip && bgmAudioSource.isPlaying)
        {
            bgmAudioSource.volume = bgmVolume * masterVolume;
            return;
        }

        bgmAudioSource.clip = bgmClip;
        bgmAudioSource.volume = bgmVolume * masterVolume;
        bgmAudioSource.Play();
    }

    public void PlayBGM(SoundId soundId)
    {
        if (bgmAudioSource == null)
        {
            Debug.LogWarning("bgmAudioSource가 없습니다.");
            return;
        }

        if (!soundDictTable.TryGetValue(soundId, out SoundData soundData) || soundData.Clip == null)
        {
            Debug.LogWarning($"SoundId {soundId}에 해당하는 BGM SoundData가 없습니다.");
            return;
        }

        AudioClip bgmClip = soundData.Clip;
        float volume = soundData.Volume * bgmVolume * masterVolume;

        if (bgmAudioSource.clip == bgmClip && bgmAudioSource.isPlaying)
        {
            bgmAudioSource.volume = volume;
            return;
        }

        if (bgmFadeCoroutine != null)
        {
            StopCoroutine(bgmFadeCoroutine);
        }

        bgmFadeCoroutine = StartCoroutine(ChangeBGMWithFade(soundData));
    }

    private IEnumerator ChangeBGMWithFade(SoundData soundData)
    {
        float targetVolume = soundData.Volume * bgmVolume * masterVolume;
        float startVolume = bgmAudioSource.volume;

        float time = 0f;

        // 먼저 현재 브금을 페이드 아웃 처리한다.
        while (time < bgmFadeDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(time / bgmFadeDuration);
            bgmAudioSource.volume = Mathf.Lerp(startVolume, 0f, t);
            yield return null;
        }

        bgmAudioSource.clip = soundData.Clip;
        bgmAudioSource.volume = 0f;
        bgmAudioSource.Play();

        time = 0f;

        while (time < bgmFadeDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(time / bgmFadeDuration);
            bgmAudioSource.volume = Mathf.Lerp(0f, targetVolume, t);
            yield return null;
        }

        bgmAudioSource.volume = targetVolume;
        bgmFadeCoroutine = null;
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

    private void InitializeSoundTable()
    {
        soundDictTable.Clear();

        if (soundTable == null) return;

        foreach (var soundData in soundTable.Sounds)
        {
            if (soundData == null || soundData.Id == SoundId.None || soundData.Clip == null ) continue;

            if (soundDictTable.ContainsKey(soundData.Id))
            {
                Debug.LogWarning($"중복된 SoundId가 있습니다. SoundId: {soundData.Id}");
                continue;
            }

            soundDictTable.Add(soundData.Id, soundData);
        }
    }
}
