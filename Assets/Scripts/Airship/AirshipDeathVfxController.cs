using System;
using DG.Tweening;
using UnityEngine;

public class AirshipDeathVfxController : MonoBehaviour
{
    [SerializeField] private GameObject[] vfxs = new GameObject[0];
    [SerializeField] private Transform body;
    [SerializeField] private Transform bodyForPunch;

    [SerializeField] private AudioClip boomSfx;

    private Sequence vfxSeq;
    private Tween bodyAnim;
    private Tween bodyPunchAnim;
    private void Awake()
    {
        SetVfxSeq();
        SetBodyAnim();
    }

    private void SetVfxSeq()
    {
        vfxSeq = DOTween.Sequence().SetAutoKill(false).Pause();
        vfxSeq
            .Join(DOVirtual.DelayedCall(
                0.1f,
                () =>
                {
                    vfxs[0].SetActive(true);
                    SoundManager.Instance.PlaySound(boomSfx, 0.7f);
                }
            ))
            .Join(DOVirtual.DelayedCall(
                0.2f,
                () =>
                {
                    vfxs[1].SetActive(true);
                    SoundManager.Instance.PlaySound(boomSfx, 0.2f);
                }
            ))
            .Join(DOVirtual.DelayedCall(
                0.3f,
                () =>
                {
                    vfxs[2].SetActive(true);
                    SoundManager.Instance.PlaySound(boomSfx, 0.2f);
                }
            ))
            .Join(DOVirtual.DelayedCall(
                0.65f,
                () =>
                {
                    vfxs[3].SetActive(true);
                    SoundManager.Instance.PlaySound(boomSfx, 0.2f);
                }
            ))
            .Join(DOVirtual.DelayedCall(
                0.8f,
                () =>
                {
                    vfxs[4].SetActive(true);
                    SoundManager.Instance.PlaySound(boomSfx, 0.2f);
                }
            ))
            .Join(DOVirtual.DelayedCall(
                1f,
                () =>
                {
                    vfxs[5].SetActive(true);
                    SoundManager.Instance.PlaySound(boomSfx, 0.2f);
                }
            ));
    }

    private void SetBodyAnim()
    {
        bodyAnim = body.DOLocalMoveY(-0.5f, 1.5f)
            .SetEase(Ease.Linear)
            .SetAutoKill(false)
            .Pause();
        bodyPunchAnim = bodyForPunch.DOPunchPosition(
                Vector3.right*0.3f,
                1.2f,
                12,
                1f,
                false)
            .SetAutoKill(false)
            .Pause()
            .OnComplete(() => bodyForPunch.localPosition = Vector3.zero);
    }
    
    public void PlayDeadAnimAndVfx()
    {
        InitObjs();
        bodyAnim.Restart();
        bodyPunchAnim.Restart();
        vfxSeq.Restart();
    }

    public void InitObjs()
    {
        bodyAnim.Pause();
        bodyPunchAnim.Pause();
        vfxSeq.Pause();
        body.localPosition = Vector3.zero;
        bodyForPunch.localPosition = Vector3.zero;
        
        for (int i = 0; i < vfxs.Length; i++)
        {
            vfxs[i].SetActive(false);
        }
    }
}
