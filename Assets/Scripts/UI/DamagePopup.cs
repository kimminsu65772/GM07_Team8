using UnityEngine;
using TMPro;
using System.Collections;
public class DamagePopup : MonoBehaviour
{
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private float moveSpeed = 1f;       
    [SerializeField] private float disappearTimer = 0.8f; 
    private Color textColor;
    public void Setup(int damage)
    {
        damageText.text = damage.ToString();
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
        Destroy(gameObject);
    }
}