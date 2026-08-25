using UnityEngine;
using TMPro;

public class HeroDisplayController : MonoBehaviour
{
    [Header("배치 설정")]
    [SerializeField] private Transform heroSpawnPoint;    

    private GameObject currentHeroInstance;                

    public void DisplayHero(HeroEntry entry)
    {
        ClearDisplay();

        if (entry == null) return;

        Transform parentTransform = heroSpawnPoint != null ? heroSpawnPoint : transform;
        if (entry.HeroPrefab != null)
        {
            currentHeroInstance = Instantiate(entry.HeroPrefab, parentTransform, false);

            RectTransform rect = currentHeroInstance.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = Vector2.zero;
                rect.localRotation = Quaternion.identity;
                rect.localScale = Vector3.one;
            }
            currentHeroInstance.transform.localScale = new Vector3(200f, 200f, 0f);
            Canvas worldCanvas = currentHeroInstance.GetComponentInChildren<Canvas>();
            if (worldCanvas != null)
            {
                worldCanvas.gameObject.SetActive(false); // 인벤토리에서는 월드 체력바 숨기기
            }

            MonoBehaviour[] scripts = currentHeroInstance.GetComponentsInChildren<MonoBehaviour>();
            foreach (var script in scripts)
            {
                string scriptName = script.GetType().Name;
                if (scriptName.Contains("Attack") || scriptName.Contains("Move") || scriptName.Contains("AI") || scriptName.Contains("Warrior") || scriptName.Contains("Hero") && !scriptName.Contains("Animation"))
                {
                    script.enabled = false;
                }
            }
            Animator animator = currentHeroInstance.GetComponentInChildren<Animator>();
            if (animator != null)
            {
                animator.SetInteger("State", 0);
            }
        }
    }
    public void ClearDisplay()
    {
        if (currentHeroInstance != null)
        {
            Destroy(currentHeroInstance);
            currentHeroInstance = null;
        }
    }
}