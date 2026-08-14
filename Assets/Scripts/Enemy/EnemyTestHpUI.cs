using UnityEngine;
using UnityEngine.UI;

public class BossHpUI : MonoBehaviour
{
    [SerializeField] private Slider hpSlider;

    private EnemyStats enemyStats;

    private void Awake()
    {
        enemyStats =
            GetComponentInParent<EnemyStats>();

        if (enemyStats == null)
        {
            return;
        }

        hpSlider.maxValue =
            enemyStats.MaxHealth;

        hpSlider.value =
            enemyStats.CurrentHealth;

        enemyStats.EnemyDamaged +=
            HandleDamaged;

        enemyStats.EnemyDied +=
            HandleDied;
    }

    private void HandleDamaged(
        EnemyStats damagedEnemy)
    {
        hpSlider.value =
            damagedEnemy.CurrentHealth;
    }

    private void HandleDied(
        EnemyStats deadEnemy)
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (enemyStats == null)
        {
            return;
        }

        enemyStats.EnemyDamaged -=
            HandleDamaged;

        enemyStats.EnemyDied -=
            HandleDied;
    }
}