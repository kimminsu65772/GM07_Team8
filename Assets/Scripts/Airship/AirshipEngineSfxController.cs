using UnityEngine;

public class AirshipEngineSfxController : MonoBehaviour
{
    [SerializeField] private AirshipMovement movement;
    [SerializeField] private AudioSource engineSource;

    [SerializeField, Range(0f, 1f)]
    private float maxVolume = 1f;

    [SerializeField, Range(0f, 0.1f)]
    private float stopSpeedRatio = 0.001f;
    
    
    [Header("속도에 따른 피치")]
    [SerializeField, Range(0.1f, 3f)]
    private float minPitch = 0.1f;

    [SerializeField, Range(0.1f, 3f)]
    private float maxPitch = 1.0f;

    private void Awake()
    {
        if (movement == null)
            movement = GetComponentInParent<AirshipMovement>();

        if (engineSource == null)
            engineSource = GetComponent<AudioSource>();

        if (engineSource == null)
            return;

        engineSource.loop = true;
        engineSource.playOnAwake = false;
        engineSource.Stop();
    }

    private void LateUpdate()
    {
        if (movement == null ||
            engineSource == null ||
            engineSource.clip == null)
        {
            return;
        }

        float maxSpeed = movement.MaxMoveSpeed;

        float speedRatio = maxSpeed <= 0f
            ? 0f
            : Mathf.Clamp01(
                movement.CurrentMoveSpeed / maxSpeed
            );

        if (speedRatio <= stopSpeedRatio)
        {
            StopEngineSound();
            return;
        }

        float localVolume = maxVolume * speedRatio;

        engineSource.volume =
            SoundManager.Instance == null
                ? localVolume
                : SoundManager.Instance.ApplySfxVolume(
                    localVolume
                );
        engineSource.pitch =
            Mathf.Lerp(
                minPitch,
                maxPitch,
                speedRatio
            );

        if (!engineSource.isPlaying)
            engineSource.Play();
    }

    private void StopEngineSound()
    {
        if (engineSource == null)
            return;

        if (engineSource.isPlaying)
            engineSource.Stop();

        engineSource.volume = 0f;
        engineSource.pitch = minPitch;
    }

    private void OnDisable()
    {
        StopEngineSound();
    }
}