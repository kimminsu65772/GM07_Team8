using UnityEngine;

public class EnemyDamageTest : MonoBehaviour
{
    [SerializeField, Min(1)] private int damageAmount = 25;

    private void OnGUI()
    {
        if (GUI.Button(
            new Rect(20f, 20f, 200f, 45f),
            $"모든 적에게 {damageAmount} 피해"))
        {
            DamageAllEnemies();
        }
    }

    private void DamageAllEnemies()
    {
        // 현재 씬에 생성되어 있는 모든 적을 찾는다.
        // 테스트 씬에서 웨이브 진행을 확인하기 위한 기능이다.
        EnemyStats[] enemies = FindObjectsByType<EnemyStats>(
            FindObjectsSortMode.None);

        foreach (EnemyStats enemy in enemies)
        {
            // 버튼을 누르는 사이 적이 사망했을 가능성을 확인한다.
            if (enemy != null)
            {
                enemy.TakeDamage(new DamageInfo(damageAmount));
            }
        }
    }
}