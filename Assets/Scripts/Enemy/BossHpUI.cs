using UnityEngine;
using UnityEngine.UI;

public class BossHpUI : MonoBehaviour
{
    [SerializeField] private Slider hpSlider;

    private EnemyStats enemyStats;
    private Canvas hpCanvas;

    private void Awake()
    {
        hpCanvas = GetComponent<Canvas>();
        enemyStats = GetComponentInParent<EnemyStats>();

        if (enemyStats == null || hpSlider == null) return;

        enemyStats.EnemyDamaged += HandleDamaged;
        enemyStats.EnemyDied += HandleDied;
    }

    private void OnEnable()
    {
        if (hpCanvas != null) hpCanvas.enabled = true;
        if (enemyStats == null || hpSlider == null) return;

        hpSlider.maxValue = enemyStats.MaxHealth;
        hpSlider.value = enemyStats.MaxHealth;
    }


    private void HandleDamaged(EnemyStats damagedEnemy)
    {
        if (hpSlider == null) return;

        hpSlider.value = damagedEnemy.CurrentHealth;
    }

    private void HandleDied(EnemyStats deadEnemy)
    {
        if (hpCanvas != null) hpCanvas.enabled = false;
    }

    private void OnDestroy()
    {
        if (enemyStats == null) return;

        enemyStats.EnemyDamaged -= HandleDamaged;
        enemyStats.EnemyDied -= HandleDied;
    }
}