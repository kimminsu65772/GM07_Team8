using System;
using UnityEngine;

public class VfxAnimationEventReceiver : MonoBehaviour
{
    // Animation Event에서 호출
    public void OnVfxFinished()
    {
        Destroy(transform.parent.gameObject);
    }
}