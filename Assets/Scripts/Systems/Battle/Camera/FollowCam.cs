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
    [SerializeField] private Transform startFollowPoint;
    [SerializeField] private StageTestManager stageTestManager;

    private bool isFollowing;

    private void OnEnable()
    {
        stageTestManager.OnStageCompleted -= StopFollowTarget;
        stageTestManager.OnStageCompleted += StopFollowTarget;
    }

    private void OnDisable()
    {
        stageTestManager.OnStageCompleted -= StopFollowTarget;
    }

    private void Start()
    {
        if (target == null)
        {
            Debug.LogError("FollowCam: 타겟이 설정되지 않았습니다.");
        }

        if (startFollowPoint == null)
        {
            Debug.LogError("FollowCam: 카메라 추적 시작 지점이 설정되지 않았습니다.");
        }

        isFollowing = false;
    }

    private void Update()
    {
        if (target == null || isFollowing)
        {
            return;
        }

        CheckStartFollow();
    }

    private void LateUpdate()
    {
        if (target == null || !isFollowing)
        {
            return;
        }

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

    private void StopFollowTarget(int stageNumber)
    {
        isFollowing = false;
    }

    private void CheckStartFollow()
    {
        if (isFollowing) return;

        Vector2 distanceX = new Vector2(target.position.x, 0f) - new Vector2(startFollowPoint.position.x, 0f);

        if (distanceX.sqrMagnitude < 0.01f)
        {
            isFollowing = true;
        }
    }   
}
