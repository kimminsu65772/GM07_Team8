using UnityEngine;
using UnityEngine.UI;

public class HeroHPBar : MonoBehaviour
{
    [SerializeField] private Slider hpBar;
    private Hero hero;

    private void Awake()
    {
        hero = GetComponent<Hero>();
        hpBar.interactable = false;
    }

    void Update()
    {
        hpBar.value = hero.HPRatio;
    }
}
