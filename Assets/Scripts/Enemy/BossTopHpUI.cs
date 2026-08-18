using UnityEngine;
using UnityEngine.UI;

public class BossTopHpUI : MonoBehaviour
{
    [SerializeField] private GameObject bossHpRoot;
    [SerializeField] private Slider bossHpSlider;

    private EnemyStats bossStats;

    private void Awake()
    {
        HideBossHp();
    }

    public void SetBoss(EnemyStats newBoss)
    {
        if (bossStats != null)
        {
            bossStats.EnemyDamaged -= HandleBossDamaged;
            bossStats.EnemyDied -= HandleBossDied;
        }

        bossStats = newBoss;

        if (bossStats == null)
        {
            HideBossHp();
            return;
        }

        bossStats.EnemyDamaged += HandleBossDamaged;
        bossStats.EnemyDied += HandleBossDied;

        bossHpRoot.SetActive(true);

        bossHpSlider.maxValue =
            bossStats.MaxHealth;

        bossHpSlider.value =
            bossStats.CurrentHealth;
    }

    private void HandleBossDamaged(
        EnemyStats damagedBoss)
    {
        bossHpSlider.value =
            damagedBoss.CurrentHealth;
    }

    private void HandleBossDied(
        EnemyStats deadBoss)
    {
        HideBossHp();
    }

    public void HideBossHp()
    {
        if (bossStats != null)
        {
            bossStats.EnemyDamaged -= HandleBossDamaged;
            bossStats.EnemyDied -= HandleBossDied;
        }

        bossStats = null;

        if (bossHpRoot != null)
        {
            bossHpRoot.SetActive(false);
        }
    }
}