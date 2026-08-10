using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 비행서의 움직임. 감가속 구현.
/// </summary>
public class AirshipMovement : MonoBehaviour
{
    [SerializeField] private float maxMoveSpeed = 1f;
    [SerializeField] private float accelerationTime = 0.5f;
    [SerializeField] private float decelerationTime = 0.3f;
    [SerializeField] private Vector3 moveDir = Vector3.right;

    private float currentMoveSpeed;
    private float targetMoveSpeed;
    public float CurrentMoveSpeed => currentMoveSpeed;
    public float MaxMoveSpeed => maxMoveSpeed;
    
    public void ApplyStats(AirshipRuntimeStats stats)
    {
        if (stats == null)
        {
            return;
        }
        maxMoveSpeed = stats.MoveSpeed;
    }
    public void MoveForward()
    {
        targetMoveSpeed = maxMoveSpeed;
        UpdateSpeed(accelerationTime);
        Move();
    }

    public void Stop()
    {
        targetMoveSpeed = 0f;
        UpdateSpeed(decelerationTime);
        Move();
    }

    public void StopImmediately()
    {
        targetMoveSpeed = 0f;
        currentMoveSpeed = 0f;
    }

    private void UpdateSpeed(float duration)
    {
        if (duration <= 0f)
        {
            currentMoveSpeed = targetMoveSpeed;
            return;
        }

        float speedChangePerSecond = maxMoveSpeed / duration;

        currentMoveSpeed = Mathf.MoveTowards(
            currentMoveSpeed,
            targetMoveSpeed,
            speedChangePerSecond * Time.deltaTime
        );
    }

    private void Move()
    {
        if (currentMoveSpeed <= 0f)
        {
            return;
        }

        transform.position += moveDir.normalized * currentMoveSpeed * Time.deltaTime;
    }
}
