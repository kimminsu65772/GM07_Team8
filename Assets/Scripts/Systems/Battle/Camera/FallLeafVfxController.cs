using System;
using UnityEngine;

public class FallLeafVfxController : MonoBehaviour
{
    [SerializeField] private FollowCam followCamera;
    [SerializeField] private ParticleSystem leafParticles;

    private AirshipMovement airshipMove;
    private float maxCameraSpeed = 1f;

    [Header("정지 시 X축 속도 범위")]
    [SerializeField] private float stoppedVelocityXMin = -1f;
    [SerializeField] private float stoppedVelocityXMax = 1f;

    [Header("최대 속도 시 X축 속도 범위")]
    [SerializeField] private float maxVelocityXMin = -7f;
    [SerializeField] private float maxVelocityXMax = -7f;
    
    private ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime;

    private void Awake()
    {
        if (leafParticles == null)
            return;

        velocityOverLifetime =
            leafParticles.velocityOverLifetime;

        velocityOverLifetime.enabled = true;
        velocityOverLifetime.space =
            ParticleSystemSimulationSpace.Local;
    }

    private void Start()
    {
        airshipMove = BattleManager.Instance.Airship.Movement;
    }

    private void LateUpdate()
    {
        if (followCamera == null ||
            leafParticles == null)
        {
            return;
        }

        maxCameraSpeed = airshipMove.MaxMoveSpeed;
        float speedRatio = maxCameraSpeed <= 0f
            ? 0f
            : Mathf.Clamp01(
                followCamera.HorSpeed /
                maxCameraSpeed
            );

        float minX = Mathf.Lerp(
            stoppedVelocityXMin,
            maxVelocityXMin,
            speedRatio
        );

        float maxX = Mathf.Lerp(
            stoppedVelocityXMax,
            maxVelocityXMax,
            speedRatio
        );

        velocityOverLifetime.x =
            new ParticleSystem.MinMaxCurve(minX, maxX);
    }
}