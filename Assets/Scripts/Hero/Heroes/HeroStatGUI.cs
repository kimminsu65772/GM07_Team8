using TMPro;
using UnityEngine;

public class HeroStatGUI : MonoBehaviour
{
    [SerializeField] private Hero hero1;
    [SerializeField] private Hero hero2;

    [SerializeField] private TMP_Text hero1_T;
    [SerializeField] private TMP_Text hero2_T;

    void Update()
    {
        hero1_T.text = $"id : {hero1.HeroID}\n" +
                            $"name : {hero1.HeroName}\n" +
                            $"hp : {hero1.HeroCurrentHP} / {hero1.HeroMaxHP}\n" +
                            $"atk : {hero1.HeroAtk}\n" +
                            $"def : {hero1.HeroDef}\n" +
                            $"atktime : {hero1.HeroAttackTime}\n" +
                            $"state : {hero1.HeroState}";

        hero2_T.text = $"id : {hero2.HeroID}\n" +
                            $"name : {hero2.HeroName}\n" +
                            $"hp : {hero2.HeroCurrentHP} / {hero2.HeroMaxHP}\n" +
                            $"atk : {hero2.HeroAtk}\n" +
                            $"def : {hero2.HeroDef}\n" +
                            $"atktime : {hero2.HeroAttackTime}\n" +
                            $"state : {hero2.HeroState}";
    }
}
