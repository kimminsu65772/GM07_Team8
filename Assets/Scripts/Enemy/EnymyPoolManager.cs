using System.Collections.Generic;
using UnityEngine;

public class EnemyPoolManager : MonoBehaviour
{
    private readonly Dictionary<GameObject, Stack<EnemyStats>>
        inactiveEnemies = new Dictionary<GameObject, Stack<EnemyStats>>();

    private readonly Dictionary< EnemyStats, GameObject>enemyPrefabByInstance = new Dictionary< EnemyStats, GameObject>();

    public EnemyStats GetEnemy( GameObject enemyPrefab, Vector3 position, Quaternion rotation)
    {
        if (enemyPrefab == null)
        {
            Debug.LogError( "가져올 적 프리팹이 없습니다.", this );

            return null;
        }

        if (!inactiveEnemies.TryGetValue( enemyPrefab,out Stack<EnemyStats> enemyPool))
        {
            enemyPool =  new Stack<EnemyStats>();

            inactiveEnemies.Add(  enemyPrefab,enemyPool );
        }

        EnemyStats enemyStats;

        if (enemyPool.Count > 0)
        {
            enemyStats =  enemyPool.Pop();
        }
        else
        {
            GameObject enemyObject =Instantiate( enemyPrefab, transform);

            enemyStats =  enemyObject.GetComponent<EnemyStats>();

            if (enemyStats == null)
            {
                Debug.LogError( $"{enemyPrefab.name}에 EnemyStats가 없습니다.", enemyPrefab );

                Destroy(enemyObject);
                return null;
            }

            enemyPrefabByInstance[ enemyStats ] = enemyPrefab;
        }

        enemyStats.transform.SetPositionAndRotation( position,   rotation );

         enemyStats.gameObject.SetActive(true);
         enemyStats.ResetForPool();
       

        return enemyStats;
    }

    public void ReleaseEnemy(
        EnemyStats enemyStats)
    {
        if (enemyStats == null || !enemyStats.gameObject.activeSelf)
        {
            return;
        }

        if (!enemyPrefabByInstance.TryGetValue( enemyStats,out GameObject enemyPrefab))
        {
            Debug.LogError( $"{enemyStats.name}의 원본 프리팹을 찾지 못했습니다.",enemyStats );

            return;
        }

        if (!inactiveEnemies.TryGetValue( enemyPrefab,out Stack<EnemyStats> enemyPool))
        {
            enemyPool = new Stack<EnemyStats>();

            inactiveEnemies.Add( enemyPrefab, enemyPool );
        }

        enemyStats.gameObject.SetActive(false);

        enemyStats.transform.SetParent(  transform );

        enemyPool.Push(  enemyStats );
    }
}