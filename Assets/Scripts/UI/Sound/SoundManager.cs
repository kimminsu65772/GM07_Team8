using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("오디오 관리")]
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioSource bgmAudioSource;
    [SerializeField] private AudioSource overrideBgmAudioSource; // 패널 전용 브금 오디오 소스

    [Header("오디오 테이블")]
    [SerializeField] private SoundTableSO soundTable;

    [Header("BGM 전환 시간")]
    [SerializeField] private float bgmFadeDuration = 1f;
    [SerializeField, Range(0f, 1f)] private float bgmOverrideDuckedVolume = 0f;

    [Header("오디오 출력 관련 정책 설정")]
    [SerializeField, Min(0f)] private float sfxCooldown = 0.05f; // 효과음 쿨타임
    [SerializeField, Min(1)] private int sfxMaxSimultaneous = 10; // 동시에 재생 가능한 최대 효과음 개수


    private float masterVolume = 1f;
    private float sfxVolume = 1f;
    private float bgmVolume = 1f;

    private Dictionary<SoundId, SoundData> soundDictTable = new();
    private Dictionary<AudioClip, float> lastPlayedTime = new();
    private Dictionary<AudioClip, int> playingCount = new();

    private Coroutine bgmFadeCoroutine;
    private BgmTrack baseBgmTrack;

    // 특정 브금이 재생되는 패널이 여러 개라면 닫히는 순서에 따라
    // 브금이 차례대로 종료되고 이전 브금이 재생될 수 있도록 스택으로 관리
    private readonly Stack<BgmTrack> bgmOverrideStack = new();

    private struct BgmTrack
    {
        public AudioClip Clip;
        public float Volume;

        public BgmTrack(AudioClip clip, float volume)
        {
            Clip = clip;
            Volume = volume;
        }
    }

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
        ConfigureAudioSource(sfxAudioSource, false);

        if (bgmAudioSource ==null)
        {
            bgmAudioSource = gameObject.AddComponent<AudioSource>();
        }
        ConfigureAudioSource(bgmAudioSource, true);

        if (overrideBgmAudioSource == null)
        {
            overrideBgmAudioSource = gameObject.AddComponent<AudioSource>();
        }
        ConfigureAudioSource(overrideBgmAudioSource, true);

        masterVolume = PlayerPrefs.GetFloat("GameVolume", 0.5f);
        sfxVolume = PlayerPrefs.GetFloat("SFX_Volume", 0.5f);
        bgmVolume = PlayerPrefs.GetFloat("BGM_Volume", 0.5f);

        if (bgmAudioSource != null)
        {
            bgmAudioSource.volume = bgmVolume * masterVolume;
        }

        InitializeSoundTable();
    }

    // 오디오 설정 코드가 중복되어서 별도의 메서드로 분리함.
    private static void ConfigureAudioSource(
        AudioSource audioSource,
        bool loop)
    {
        if (audioSource == null)
        {
            return;
        }

        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;
        audioSource.loop = loop;
    }
    //효과음 재생 함수
    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (sfxAudioSource == null)
        {
            Debug.LogWarning("sfxAudioSource가 없습니다.");
            return;
        }

        if (clip == null)
        {
            Debug.LogWarning("재생할 AudioClip이 없습니다.");
            return;
        }

        if (!CanPlayClip(clip, sfxCooldown, sfxMaxSimultaneous)) return;

        sfxAudioSource.PlayOneShot(clip, volume * sfxVolume * masterVolume);

        float duration = clip.length;
        StartCoroutine(ReleasePlayingCount(clip, duration));
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

        if (!CanPlayClip(soundData.Clip, soundData.Cooldown, soundData.MaxSimultaneous)) return;

        float pitch = Random.Range(soundData.PitchRange.x, soundData.PitchRange.y);
        float volume = soundData.Volume * volumeMultiplier * sfxVolume * masterVolume;

        sfxAudioSource.pitch = pitch;
        sfxAudioSource.PlayOneShot(soundData.Clip, volume);
        sfxAudioSource.pitch = 1f;

        float duration = soundData.Clip.length / pitch;
        StartCoroutine(ReleasePlayingCount(soundData.Clip, duration));
    }

    private bool CanPlayClip(AudioClip clip, float cooldown, int maxSimultaneous)
    {
        if (clip == null) return false;


        // 쿨타임이 다 돌았는지 체크하고 재생가능하면 쿨타임 갱신
        if (cooldown > 0f &&
            lastPlayedTime.TryGetValue(clip, out float lastTime))
        {
            if (Time.unscaledTime - lastTime < cooldown)
            {
                return false;
            }
        }

        // 동시에 재생 가능한 최대 개수 체크
        int currentCount = playingCount.TryGetValue(clip, out int count) ? count : 0;

        if (currentCount >= maxSimultaneous)
        {
            return false;
        }

        // 모든 재생 조건을 만족하면 쿨타임 갱신 및 재생 카운트 증가
        lastPlayedTime[clip] = Time.unscaledTime;
        playingCount[clip] = currentCount + 1;
        

        return true;
    }

    private IEnumerator ReleasePlayingCount(AudioClip clip, float duration)
    {
        yield return new WaitForSecondsRealtime(duration);

        if (!playingCount.TryGetValue(clip, out int count))
        {
            yield break;
        }

        count--;

        if (count <= 0)
        {
            playingCount.Remove(clip);
        }
        else
        {
            playingCount[clip] = count;
        }
    }

    //배경음악 재생 함수
    public void PlayBGM(AudioClip bgmClip)
    {
        if (bgmClip == null) return;

        SetBaseBGM(
            new BgmTrack(
                bgmClip,
                1f
            )
        );
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

        SetBaseBGM(
            new BgmTrack(
                soundData.Clip,
                soundData.Volume
            )
        );
    }

    // 패널에서 특정 브금을 덮어쓰는 경우를 위한 메서드
    public void PushBgmOverride(SoundId soundId)
    {
        if (!soundDictTable.TryGetValue(soundId, out SoundData soundData) ||
            soundData.Clip == null)
        {
            Debug.LogWarning($"SoundId {soundId}에 해당하는 BGM SoundData가 없습니다.");
            return;
        }

        bgmOverrideStack.Push(
            new BgmTrack(
                soundData.Clip,
                soundData.Volume
            )
        );

        ApplyBgmState();
    }

    public void PopBgmOverride()
    {
        if (bgmOverrideStack.Count == 0)
        {
            return;
        }

        bgmOverrideStack.Pop();
        ApplyBgmState();
    }

    private void SetBaseBGM(BgmTrack track)
    {
        baseBgmTrack = track;
        ApplyBgmState();
    }

    private void ApplyBgmState()
    {
        if (bgmFadeCoroutine != null)
        {
            StopCoroutine(bgmFadeCoroutine);
        }

        bgmFadeCoroutine = StartCoroutine(ApplyBgmStateRoutine());
    }

    private IEnumerator ApplyBgmStateRoutine()
    {
        BgmTrack activeOverride =
            bgmOverrideStack.Count > 0
                ? bgmOverrideStack.Peek()
                : default;

        bool hasOverride =
            bgmOverrideStack.Count > 0 &&
            activeOverride.Clip != null;

        BgmTrack overrideTrack =
            hasOverride
                ? activeOverride
                : default;

        if (baseBgmTrack.Clip != null)
        {
            PlaySourceIfNeeded(
                bgmAudioSource,
                baseBgmTrack
            );
        }

        if (hasOverride)
            PlaySourceIfNeeded(overrideBgmAudioSource, overrideTrack);

        float baseStartVolume =
            bgmAudioSource != null ? bgmAudioSource.volume : 0f;

        float overrideStartVolume =
            overrideBgmAudioSource != null ? overrideBgmAudioSource.volume : 0f;

        float baseTargetVolume =
            baseBgmTrack.Clip == null
                ? 0f
                : GetBgmVolume(baseBgmTrack.Volume) *
                  (hasOverride ? bgmOverrideDuckedVolume : 1f);

        float overrideTargetVolume =
            hasOverride
                ? GetBgmVolume(overrideTrack.Volume)
                : 0f;

        float time = 0f;

        while (time < bgmFadeDuration)
        {
            time += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(time / bgmFadeDuration);

            if (bgmAudioSource != null)
            {
                bgmAudioSource.volume =
                    Mathf.Lerp(baseStartVolume, baseTargetVolume, t);
            }

            if (overrideBgmAudioSource != null)
            {
                overrideBgmAudioSource.volume =
                    Mathf.Lerp(overrideStartVolume, overrideTargetVolume, t);
            }

            yield return null;
        }

        if (bgmAudioSource != null)
            bgmAudioSource.volume = baseTargetVolume;

        if (overrideBgmAudioSource != null)
        {
            overrideBgmAudioSource.volume = overrideTargetVolume;

            if (!hasOverride)
            {
                overrideBgmAudioSource.Stop();
                overrideBgmAudioSource.clip = null;
            }
        }

        bgmFadeCoroutine = null;
    }

    private void PlaySourceIfNeeded(AudioSource audioSource, BgmTrack track)
    {
        if (audioSource == null ||
            track.Clip == null)
        {
            return;
        }

        if (audioSource.clip == track.Clip &&
            audioSource.isPlaying)
        {
            return;
        }

        audioSource.clip = track.Clip;
        audioSource.volume = 0f;
        audioSource.Play();
    }

    private float GetBgmVolume(float localVolume)
    {
        return localVolume * bgmVolume * masterVolume;
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
            bgmAudioSource.volume =
                GetBgmVolume(baseBgmTrack.Volume) *
                (bgmOverrideStack.Count > 0 ? bgmOverrideDuckedVolume : 1f);
        }

        if (overrideBgmAudioSource != null &&
            bgmOverrideStack.Count > 0)
        {
            overrideBgmAudioSource.volume =
                GetBgmVolume(bgmOverrideStack.Peek().Volume);
        }
        PlayerPrefs.SetFloat("BGM_Volume", volume);
        PlayerPrefs.Save();
    }
    public void SetVolume(float volume)
    {
        masterVolume = volume;

        if (bgmAudioSource != null)
        {
            bgmAudioSource.volume =
                GetBgmVolume(baseBgmTrack.Volume) *
                (bgmOverrideStack.Count > 0 ? bgmOverrideDuckedVolume : 1f);
        }

        if (overrideBgmAudioSource != null &&
            bgmOverrideStack.Count > 0)
        {
            overrideBgmAudioSource.volume =
                GetBgmVolume(bgmOverrideStack.Peek().Volume);
        }

        PlayerPrefs.SetFloat("GameVolume", volume);
        PlayerPrefs.Save();
    }
    /// <summary>
    /// 외부에서 볼륨설정값을 접근해 볼륨 설정하기 위한 함수
    /// </summary>
    /// <param name="localVolume"></param>
    /// <returns></returns>
    public float ApplySfxVolume(float localVolume)
    {
        return localVolume * sfxVolume * masterVolume;
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
