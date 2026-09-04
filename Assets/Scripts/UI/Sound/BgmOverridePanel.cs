using UnityEngine;

public class BgmOverridePanel : MonoBehaviour
{
    [SerializeField] private SoundId overrideBgmId = SoundId.SmithyBGM;

    private void OnEnable()
    {
        if (SoundManager.Instance == null ||
            overrideBgmId == SoundId.None)
        {
            return;
        }

        SoundManager.Instance.PushBgmOverride(overrideBgmId);
    }

    private void OnDisable()
    {
        if (SoundManager.Instance == null)
        {
            return;
        }

        SoundManager.Instance.PopBgmOverride();
    }
}
