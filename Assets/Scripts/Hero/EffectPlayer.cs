using System.Collections;
using UnityEngine;

public class EffectPlayer : MonoBehaviour
{
    [Header("이펙트 설정")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] attackFrames;
    [SerializeField] private Sprite[] skillFrames;
    [SerializeField] private float frameTime = 0.05f;

    private Coroutine effectCoroutine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void PlayAttackEffect()
    {
        if (effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);
        }

        effectCoroutine = StartCoroutine(PlayEffectCoroutine(attackFrames));
    }

    public void PlaySkillEffect()
    {
        if (effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);
        }

        effectCoroutine = StartCoroutine(PlayEffectCoroutine(skillFrames));
    }

    private IEnumerator PlayEffectCoroutine(Sprite[] frames)
    {
        if (frames == null || frames.Length == 0)
        {
            Debug.LogWarning($"{gameObject.name}: AttackFrames가 비어 있습니다.");
            yield break;
        }

        for (int i = 0; i < frames.Length; i++)
        {
            spriteRenderer.sprite = frames[i];
            yield return new WaitForSeconds(frameTime);
        }
        spriteRenderer.sprite = null;
        effectCoroutine = null;
    }
}