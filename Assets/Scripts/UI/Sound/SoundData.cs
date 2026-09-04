using System;
using UnityEngine;

// 경로를 직접 입력하지 않고 enum으로 관리하여 사운드를 재생할 수 있도록 함.
public enum SoundId
{
    None = 0,

    // UI 버튼 관련
    UIButtonHover,
    UIButtonClick,

    // BGM
    Region1BGM = 100,
    Region2BGM,
    Region3BGM,
    Region4BGM,
    Region5BGM,
    SmithyBGM,

    // Craft 관련
    Crafting = 200,
    CraftCommon,
    CraftRare,
    CraftEpic,
    CraftLegendary,

    // 장비 관련
    EquipmentEquip = 300,
    EquipmentUnequip,
    ClickEquipment,
    DecomposeEquipment,
    WarningPopup,

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
