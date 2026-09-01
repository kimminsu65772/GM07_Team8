using System;
using UnityEngine;

// 경로를 직접 입력하지 않고 enum으로 관리하여 사운드를 재생할 수 있도록 함.
public enum SoundId
{
    None,

    // UI 버튼 관련
    UIButtonHover,
    UIButtonClick,

    // 적 관련
    EnemyDeath,
    EnemyAttack,
    EnemyUseSkills, // 스킬의 경우 고유의 사운드를 가지고 있으므로 그에 대한 정의가 필요함.

    // 영웅 관련
    HeroDeath,
    HeroAttack,
    HeroUseSkills, // 영웅 스킬도 마찬가지

    // 비행선 관련
    AirshipDestroyed,
    AirshipMove,
    AirshipAttack,

    // BGM
    TitleBGM,
    PrairieBGM,
    SmithyBGM,
}

[Serializable]
public sealed class SoundData
{
    [SerializeField] private SoundId soundId = SoundId.None;
    [SerializeField] private AudioClip clip;
    [SerializeField, Range(0f, 1f)] private float volume = 1f;
    [SerializeField] private Vector2 pitchRange = new Vector2(1f, 1f);
    [SerializeField, Min(1)] private int maxSimultaneous = 3;
    [SerializeField, Min(0f)] private float cooldown;

    public SoundId Id => soundId;
    public AudioClip Clip => clip;
    public float Volume => volume;
    public Vector2 PitchRange => pitchRange;
    public int MaxSimultaneous => maxSimultaneous;
    public float Cooldown => cooldown;
}
