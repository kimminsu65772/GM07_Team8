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
    public void ShowDamage(int damage, Vector3 position)
    {
        if (damagePopupPrefab == null) return;

        Vector3 spawnPos = position + new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(0.5f, 1f), 0);

        GameObject popup = Instantiate(damagePopupPrefab, spawnPos, Quaternion.identity, worldCanvasTransform);
        DamagePopup popupScript = popup.GetComponent<DamagePopup>();

        if (popupScript != null)
        {
            popupScript.Setup(damage);
        }
    }
}