using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime = 0.15f;

    
    [SerializeField] private bool useBounds = false;
    [SerializeField] private Vector2 minBounds;
    [SerializeField] private Vector2 maxBounds;

    private Vector3 velocity = Vector3.zero;
    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if(target == null) return;

        Vector3 desiredPos = new Vector3(target.position.x, target.position.y, transform.position.z);
        Vector3 smoothedPos = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, smoothTime);

        if (useBounds) // 카메라를 방 안에 가뒀는지 확인 
        {
            smoothedPos = ClampToBounds(smoothedPos);
        }

        transform.position = smoothedPos;


    }

    private Vector3 ClampToBounds(Vector3 pos) // 카메라를 방 안에 가두기
    {
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        float clampedX = Mathf.Clamp(pos.x,minBounds.x + camWidth, maxBounds.x - camWidth);
        float clampedY = Mathf.Clamp(pos.y,minBounds.y + camHeight, maxBounds.y - camHeight);

        return new Vector3(clampedX, clampedY, pos.z);
    }

    public void SetRoomBounds(Vector2 min, Vector2 max) 
    {
        minBounds = min;
        maxBounds = max;
    }

    public void SetTarget(Transform newTarget) 
    {
        target = newTarget;
    }






}