using UnityEngine;
public class DamageManager : MonoBehaviour
{
    public static DamageManager Instance;

    [Header("References")]
    [SerializeField] private GameObject damagePopupPrefab; 
    [SerializeField] private Transform worldCanvasTransform; 
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    public void ShowDamage(DamageInfo damageInfo, Vector3 position)
    {
        if (damagePopupPrefab == null)return;

        GameObject popup = Instantiate(damagePopupPrefab, position, Quaternion.identity, worldCanvasTransform);
        DamagePopup popupScript = popup.GetComponent<DamagePopup>();

        if (popupScript != null)
        {
            popupScript.Setup(damageInfo);
        }
    }
}