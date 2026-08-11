using UnityEngine;

[CreateAssetMenu(fileName = "HeroEntry", menuName = "Game/Hero/HeroEntry")]
public class HeroEntry : ScriptableObject
{
    [Header("영웅 설정")]
    [SerializeField] private string heroName;
    [SerializeField] private GameObject heroPrefab;
    [SerializeField] private HeroLocationEnum heroLocation;
    [SerializeField] private bool isDefaultOwned;
    [SerializeField, Min(1)] private int defaultLevel = 1;

    public string HeroName => heroName;
    public GameObject HeroPrefab => heroPrefab;
    public bool IsDefaultOwned => isDefaultOwned;

    public HeroLocationEnum HeroLocation => heroLocation;
    public int DefaultLevel => Mathf.Max(1, defaultLevel);


}
