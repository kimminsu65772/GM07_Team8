using UnityEngine;

public class TestAirshipHealth : MonoBehaviour
{
    [Header("Test Health")]
    [SerializeField, Min(1)] private int maxHealth = 30;
    [SerializeField] private int currentHealth;

    private void Awake()
    {
        // 테스트 시작 시 현재 체력을 최대 체력으로 설정한다.
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        // 잘못된 피해량이거나 이미 사망했다면 처리하지 않는다.
        if (damageAmount <= 0 || currentHealth <= 0)
        {
            return;
        }

        // 체력이 음수가 되지 않도록 최소값을 0으로 제한한다.
        currentHealth = Mathf.Max(
            currentHealth - damageAmount,
            0);

        // 체력이 0이 되면 타깃을 제거한다.
        // 이때 EnemyAttack이 타깃 소멸을 정상 처리하는지도 확인한다.
        if (currentHealth == 0)
        {
            Destroy(gameObject);
        }
    }
}