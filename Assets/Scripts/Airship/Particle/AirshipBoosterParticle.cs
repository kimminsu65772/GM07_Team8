using UnityEngine;

public class AirshipBoosterParticle : MonoBehaviour
{
    [SerializeField] private AirshipMovement movement;
    [SerializeField] private ParticleSystem boosterParticles;

    [Header("최고 속도에서의 최대 방출량")]
    [SerializeField, Min(0f)]
    private float maxEmissionRate = 100f;
    
    [Header("최고 속도에서의 Z축 최대 속도")]
    [SerializeField]
    private float maxVelocityZ = -0.4f;

    private ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime;

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

        if (boosterParticles == null)
        {
            boosterParticles =
                GetComponent<ParticleSystem>();
        }

        if (boosterParticles != null)
        {
            emission =
                boosterParticles.emission;
            
            velocityOverLifetime =
                boosterParticles.velocityOverLifetime;

            velocityOverLifetime.enabled = true;
            velocityOverLifetime.space =
                ParticleSystemSimulationSpace.Local;
        }
    }

    private void LateUpdate()
    {
        if (movement == null ||
            boosterParticles == null)
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
        
        float targetVelocityZ =
            maxVelocityZ * (1f - speedRatio);

        velocityOverLifetime.z =
            new ParticleSystem.MinMaxCurve(targetVelocityZ);

        float curveValue =
            emissionCurve == null
                ? speedRatio
                : emissionCurve.Evaluate(speedRatio);

        emission.rateOverTime =
            maxEmissionRate * curveValue;

        if (speedRatio > 0.001f)
        {
            if (!boosterParticles.isPlaying)
            {
                boosterParticles.Play();
            }
        }
        else if (boosterParticles.isPlaying)
        {
            boosterParticles.Stop(
                true,
                ParticleSystemStopBehavior.StopEmitting
            );
        }
    }

    private void OnDisable()
    {
        if (boosterParticles == null)
        {
            return;
        }

        boosterParticles.Stop(
            true,
            ParticleSystemStopBehavior.StopEmittingAndClear
        );
    }
}