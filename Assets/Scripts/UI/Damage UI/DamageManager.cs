using UnityEngine;
public class DamageManager : MonoBehaviour
{
    public static DamageManager Instance;

    [Header("References")]
    [SerializeField] private Transform worldCanvasTransform; 
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    public void ShowDamage(
        DamageInfo damageInfo,
        Vector3 position)
    {
        if (worldCanvasTransform == null ||
            PoolingManager.Instance == null)
        {
            return;
        }

        DamagePopup popup =
            PoolingManager.Instance.GetDamagePopup(
                position,
                worldCanvasTransform
            );

        if (popup == null)
        {
            return;
        }

        popup.Setup(damageInfo);
    }
}