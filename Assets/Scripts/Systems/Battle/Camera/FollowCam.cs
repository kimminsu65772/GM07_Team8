using UnityEngine;

public class FollowCam : MonoBehaviour
{
    [Header("타겟 설정")]
    [SerializeField] private Transform target;

    [SerializeField] private bool smoothCamera = true;
    [SerializeField] private bool lockVerticalAxis = false;
    [SerializeField] private bool lockCameraSize = false;
    [SerializeField] private float cameraSize = 6f;
    [SerializeField] private float targetOffsetY = -0.3f;
    [SerializeField] private float targetOffsetX = -1f;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogError("FollowCam: 타겟이 설정되지 않았습니다.");
        }
    }

    private void Update()
    {
        Camera.main.orthographicSize = lockCameraSize ? 5f : cameraSize;

        float targetDistanceY = Camera.main.orthographicSize * targetOffsetY;
        float targetDistanceX = Camera.main.orthographicSize * -targetOffsetX;
        float smoothSpeed = 5.0f;

        Vector3 desiredPosition = new Vector3(target.position.x + targetDistanceX, lockVerticalAxis ? targetDistanceY : target.position.y + targetDistanceY, -10f);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.position = smoothCamera ? smoothedPosition : desiredPosition;
    }
    private void OnDrawGizmos()
    {
        Camera followCamera = GetComponent<Camera>();
        if (followCamera == null || !followCamera.orthographic)
        {
            return;
        }

        float cameraHeight = followCamera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * followCamera.aspect;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            transform.position,
            new Vector3(cameraWidth, cameraHeight, 0f));
    }
}
