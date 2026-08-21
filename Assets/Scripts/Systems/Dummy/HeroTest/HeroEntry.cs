using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "HeroEntry", menuName = "Game/Hero/HeroEntry")]
public class HeroEntry : ScriptableObject
{
    [Header("영웅 설정")]
    [SerializeField] private HeroNameEnum heroId;
    [SerializeField] private Sprite heroIcon;
    [SerializeField] private GameObject heroPrefab;
    [SerializeField] private HeroLocationEnum heroLocation;
    [SerializeField] private bool isDefaultOwned;
    [SerializeField, Min(1)] private int defaultLevel = 1;

    [Header("스킬 관련 설정")]
    [SerializeField] private Sprite skillIcon;
    [SerializeField] private string skillName;
    [SerializeField] private string skillDescription;

    public HeroNameEnum HeroId => heroId;
    public string HeroName => heroId.ToString();
    public Sprite HeroIcon => heroIcon;
    public GameObject HeroPrefab => heroPrefab;
    public bool IsDefaultOwned => isDefaultOwned;
    public HeroLocationEnum HeroLocation => heroLocation;
    public int DefaultLevel => Mathf.Max(1, defaultLevel);
    public Sprite SkillIcon => skillIcon;
    public string SkillName => skillName;
    public string SkillDescription => skillDescription;
}
