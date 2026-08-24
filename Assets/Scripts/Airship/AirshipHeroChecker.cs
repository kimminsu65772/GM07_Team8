using System.Collections.Generic;
using UnityEngine;

public class AirshipHeroChecker : MonoBehaviour
{
    public Hero FindHealTarget()
    {
        IReadOnlyList<Hero> heroes =
            BattleManager.Instance.SpawnedHeroes;

        Hero target = null;
        float lowestHealthRatio = float.MaxValue;

        for (int i = 0; i < heroes.Count; i++)
        {
            Hero hero = heroes[i];

            if (hero == null ||
                !hero.isActiveAndEnabled ||
                hero.IsDead ||
                hero.Location != HeroLocationEnum.Front ||
                hero.HeroMaxHP <= 0f ||
                hero.HeroCurrentHP <= 0f ||
                hero.HeroCurrentHP >= hero.HeroMaxHP)
            {
                continue;
            }

            float healthRatio =
                (float)hero.HeroCurrentHP / (float)hero.HeroMaxHP;

            if (healthRatio < lowestHealthRatio)
            {
                lowestHealthRatio = healthRatio;
                target = hero;
            }
        }

        return target;
    }
}