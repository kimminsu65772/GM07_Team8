using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossTopHpUI : MonoBehaviour
{
    [SerializeField] private GameObject bossHpRoot;
    [SerializeField] private Slider bossHpSlider;
    [SerializeField] private Slider bossTimeSlider;
    [SerializeField] private TMP_Text bossTimeText;

    private EnemyStats bossStats;

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

        bossHpSlider.maxValue = bossStats.MaxHealth;
        bossHpSlider.value = bossStats.CurrentHealth;
    }

    private void HandleBossDamaged(EnemyStats damagedBoss)
    {
        bossHpSlider.value = damagedBoss.CurrentHealth;
    }

    private void HandleBossDied(EnemyStats deadBoss)
    {
        HideBossHp();
    }

    public void SetBossTime(float remainingTime, float maxTime)
    {
        if (bossTimeSlider != null)
        {
            bossTimeSlider.maxValue = maxTime;
            bossTimeSlider.value =
                Mathf.Clamp(
                    remainingTime,
                    0f,
                    maxTime
                );
        }

        if (bossTimeText != null &&
            bossTimeSlider != null)
        {
            float time =
                bossTimeSlider.value;

            int minutes =
                Mathf.FloorToInt(
                    time / 60f
                );

            int seconds =
                Mathf.FloorToInt(
                    time % 60f
                );

            bossTimeText.text =
                $"{minutes:00}:{seconds:00}";
        }
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