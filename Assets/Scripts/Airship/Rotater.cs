using System;
using DG.Tweening;
using UnityEngine;

public class Rotater : MonoBehaviour
{
    public float angle = 0f;

    [ContextMenu("Clear")]
    public void ClearTween()
    {
        DOTween.Clear(true);
    }
    [ContextMenu("Rotate")]
    public void Test()
    {
        transform.DOKill();
        transform
            .DORotate(new Vector3(0, 0, angle), 1f, RotateMode.FastBeyond360)
            .SetRelative()
            .SetLoops(-1, LoopType.Incremental)
            .SetEase(Ease.Linear);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Test();
        }
    }
}
