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

    public HeroNameEnum HeroId => heroId;
    public string HeroName => heroId.ToString();
    public Sprite HeroIcon => heroIcon;
    public GameObject HeroPrefab => heroPrefab;
    public bool IsDefaultOwned => isDefaultOwned;
    public HeroLocationEnum HeroLocation => heroLocation;
    public int DefaultLevel => Mathf.Max(1, defaultLevel);
}
