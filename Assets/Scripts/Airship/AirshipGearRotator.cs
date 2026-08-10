using UnityEngine;

public class AirshipGearRotator : MonoBehaviour
{
    [SerializeField] private AirshipMovement airshipMovement;
    [SerializeField] private bool rotateRight = true;
    [SerializeField, Min(0f)] private float minRotationSpeed = 30f;
    [SerializeField, Min(0f)] private float maxRotationSpeed = 180f;

    private float currentAngle;
    private Vector3 baseLocalEulerAngles;

    private void Awake()
    {
        if (airshipMovement == null)
            airshipMovement = GetComponentInParent<AirshipMovement>();

        baseLocalEulerAngles = transform.localEulerAngles;
        currentAngle = baseLocalEulerAngles.z;
    }

    private void Update()
    {
        if (airshipMovement == null)
            return;

        float maxMoveSpeed = airshipMovement.MaxMoveSpeed;

        float speedRatio = maxMoveSpeed <= 0f
            ? 0f
            : Mathf.Clamp01(
                airshipMovement.CurrentMoveSpeed / maxMoveSpeed
            );

        float rotationSpeed = Mathf.Lerp(
            minRotationSpeed,
            maxRotationSpeed,
            speedRatio
        );

        float direction = rotateRight ? -1f : 1f;

        currentAngle += direction *
                        rotationSpeed *
                        Time.deltaTime;

        while (currentAngle >= 360f)
            currentAngle -= 360f;

        while (currentAngle < 0f)
            currentAngle += 360f;

        transform.localEulerAngles = new Vector3(
            baseLocalEulerAngles.x,
            baseLocalEulerAngles.y,
            currentAngle
        );
    }
}