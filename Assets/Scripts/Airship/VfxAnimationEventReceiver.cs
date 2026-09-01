using UnityEngine;

public class VfxAnimationEventReceiver : MonoBehaviour
{
    private PoolingManager poolingManager;
    private GameObject pooledVfx;
    [SerializeField]private bool isAirshipDeathVfx = false;

    public void SetPoolingManager(
        PoolingManager poolingManager,
        GameObject pooledVfx)
    {
        this.poolingManager = poolingManager;
        this.pooledVfx = pooledVfx;
    }

    // Animation Event에서 호출
    public void OnVfxFinished()
    {
        if (isAirshipDeathVfx)
        {
            gameObject.SetActive(false);
            return;
        }
        
        if (pooledVfx == null)
        {
            gameObject.SetActive(false);
            return;
        }

        if (poolingManager == null)
        {
            Debug.LogError(
                "VFX에 PoolingManager가 연결되지 않았습니다.",
                this
            );

            pooledVfx.SetActive(false);
            return;
        }

        poolingManager.ReleaseFreezeImpactVfx(
            pooledVfx
        );
    }
}