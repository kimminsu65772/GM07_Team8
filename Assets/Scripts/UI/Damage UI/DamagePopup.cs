using UnityEngine;
using TMPro;
using System.Collections;
public class DamagePopup : MonoBehaviour
{
    [SerializeField] private TMP_Text damageText;

    [Header("일반")]
    [SerializeField] private Color normalColor = Color.white;

    [Header("크리티컬")]
    [SerializeField] private Color criticalColor = Color.yellow;

    [Header("힐 (회복)")]
    [SerializeField] private Color healColor = Color.green;

    [SerializeField] private float moveSpeed = 1f;       
    [SerializeField] private float disappearTimer = 0.8f; 
    private Color textColor;
    
    
    private PoolingManager poolingManager;
    
    public void Setup(DamageInfo damageInfo)
    {
        StopAllCoroutines();
        Debug.Log($"Popup - Damage: {damageInfo.Damage}, IsHeal: {damageInfo.IsHeal}");
        damageText.text = GameFormatUtils.ToIdleNumber(damageInfo.Damage);
        bool isHealCondition = damageInfo.IsHeal || damageInfo.Damage < 0;
        if (isHealCondition)
        {
            damageText.color = healColor;
        }
        else if (damageInfo.IsCritical)
        {
            damageText.color = criticalColor;
        }
        else
        {
            damageText.color = normalColor;
        }
        textColor = damageText.color;
        StartCoroutine(PopupRoutine());
    }
    private IEnumerator PopupRoutine()
    {
        float timer = disappearTimer;
        Vector3 moveDirection = new Vector3(Random.Range(-0.5f, 0.5f), 1f, 0).normalized; 
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
            if (textColor.a > 0)
            {
                textColor.a -= Time.deltaTime / disappearTimer;
                damageText.color = textColor;
            }
            yield return null;
        }
        if (poolingManager != null)
        {
            poolingManager.ReleaseDamagePopup(this);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    
    public void SetPoolingManager(
        PoolingManager poolingManager)
    {
        this.poolingManager = poolingManager;
    }
}