using System.Collections;
using UnityEngine;

public class EffectPlayer : MonoBehaviour
{
    [Header("이펙트 설정")]
    [SerializeField] private SpriteRenderer selfEffect;
    [SerializeField] private SpriteRenderer targetEffect;
    [SerializeField] private Sprite[] attackFrames;
    [SerializeField] private Sprite[] skillFrames;
    [SerializeField] private Sprite[] targetFrames;
    [SerializeField] private float frameTime = 0.05f;

    private Coroutine selfEffectCoroutine;
    private Coroutine targetEffectCoroutine;

    private Sprite[] tmpFrames;

    private void Awake()
    {
        if (selfEffect == null)
        {
            selfEffect = GetComponentInChildren<SpriteRenderer>();
        }
    }

    public void PlayAttackEffect(Vector2 posPreset, Vector2 scalePreset, Vector3? rotationPreset = null)
    {
        Vector3 rotation = rotationPreset ?? Vector3.zero;

        SetSelfEffect(posPreset, scalePreset, rotation);

        PlaySelfEffect(attackFrames);
    }

    public void PlaySkillEffect(Vector2 posPreset, Vector2 scalePreset, Vector3? rotationPreset = null)
    {
        Vector3 rotation = rotationPreset ?? Vector3.zero;

        SetSelfEffect(posPreset, scalePreset, rotation);

        PlaySelfEffect(skillFrames);    
    }

    // 플레이어 생성 시 awake에서 지정
    public void PlayTargetEffect(Transform pos, Vector2 posPreset, Vector2 scalePreset)
    {
        SetTargetEffect(pos.position, posPreset, scalePreset);  

        PlayTargetEffect(targetFrames);
    }

    private void SetSelfEffect(Vector2 posPreset, Vector2 scalePreset, Vector3 rotationPreset)
    {
        selfEffect.transform.localPosition = new Vector3(posPreset.x, posPreset.y, 0);
        selfEffect.transform.localScale = new Vector3(scalePreset.x, scalePreset.y, 1);
        selfEffect.transform.localEulerAngles = rotationPreset;
    }

    // 스킬 실행 시 지정
    private void SetTargetEffect(Vector3 targetPosition, Vector2 posPreset, Vector2 scalePreset)
    {
        targetEffect.transform.position = targetPosition + new Vector3(posPreset.x, posPreset.y, 0);
        targetEffect.transform.localScale = new Vector3(scalePreset.x, scalePreset.y, 1);
    }

    private void PlaySelfEffect(Sprite[] frames)
    {
        if (selfEffectCoroutine != null)
        {
            StopCoroutine(selfEffectCoroutine);
        }

        selfEffectCoroutine = StartCoroutine(
            PlayEffectCoroutine(selfEffect, frames, true)
        );
    }

    private void PlayTargetEffect(Sprite[] frames)
    {
        if (targetEffectCoroutine != null)
        {
            StopCoroutine(targetEffectCoroutine);
        }

        targetEffectCoroutine = StartCoroutine(
            PlayEffectCoroutine(targetEffect, frames, false)
        );
    }

    private IEnumerator PlayEffectCoroutine(
    SpriteRenderer renderer,
    Sprite[] frames,
    bool isSelfEffect)
    {
        if (frames == null || frames.Length == 0)
        {
            Debug.LogWarning($"{gameObject.name}: 이펙트 프레임이 비어 있습니다.");

            renderer.sprite = null;

            if (isSelfEffect)
                selfEffectCoroutine = null;
            else
                targetEffectCoroutine = null;

            yield break;
        }

        for (int i = 0; i < frames.Length; i++)
        {
            renderer.sprite = frames[i];

            yield return new WaitForSeconds(frameTime);
        }

        renderer.sprite = null;

        if (isSelfEffect)
            selfEffectCoroutine = null;
        else
            targetEffectCoroutine = null;
    }

    public void ChangeFrames()
    {
        tmpFrames = attackFrames;
        attackFrames = targetFrames;
        targetFrames = tmpFrames;
    }
}