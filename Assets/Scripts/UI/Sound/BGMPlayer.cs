using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip bgmClip;
    void Start()
    {
        if (bgmClip != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayBGM(bgmClip);
        }
    }
}
