using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    public float minY = -1.0f;
    public float minX = -10.0f;
    public float maxX = 10.0f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        desiredPosition.z = transform.position.z;

        desiredPosition.y = Mathf.Max(desiredPosition.y, minY);
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}