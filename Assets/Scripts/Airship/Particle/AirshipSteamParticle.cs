using UnityEngine;

public class AirshipSteamParticle : MonoBehaviour
{
    [SerializeField] private AirshipMovement movement;
    [SerializeField] private ParticleSystem steamParticles;

    [Header("정지 상태에서의 최소 방출량")]
    [SerializeField, Min(0f)]
    private float minEmissionRate = 5f;

    [Header("최고 속도에서의 최대 방출량")]
    [SerializeField, Min(0f)]
    private float maxEmissionRate = 30f;

    [Header("속도에 따른 방출량 곡선")]
    [SerializeField]
    private AnimationCurve emissionCurve =
        AnimationCurve.Linear(0f, 0f, 1f, 1f);

    private ParticleSystem.EmissionModule emission;

    private void Awake()
    {
        if (movement == null)
        {
            movement =
                GetComponentInParent<AirshipMovement>();
        }

        if (steamParticles == null)
        {
            steamParticles =
                GetComponent<ParticleSystem>();
        }

        if (steamParticles != null)
        {
            emission =
                steamParticles.emission;
        }
    }

    private void LateUpdate()
    {
        if (movement == null ||
            steamParticles == null)
        {
            return;
        }

        float maxSpeed =
            movement.MaxMoveSpeed;

        float speedRatio =
            maxSpeed <= 0f
                ? 0f
                : Mathf.Clamp01(
                    movement.CurrentMoveSpeed /
                    maxSpeed
                );

        float curveValue =
            emissionCurve == null
                ? speedRatio
                : Mathf.Clamp01(
                    emissionCurve.Evaluate(speedRatio)
                );

        float targetEmissionRate =
            Mathf.Lerp(
                minEmissionRate,
                maxEmissionRate,
                curveValue
            );

        emission.rateOverTime =
            targetEmissionRate;

        if (!steamParticles.isPlaying)
        {
            steamParticles.Play();
        }
    }

    private void OnDisable()
    {
        if (steamParticles == null)
        {
            return;
        }

        steamParticles.Stop(
            true,
            ParticleSystemStopBehavior.StopEmittingAndClear
        );
    }
}