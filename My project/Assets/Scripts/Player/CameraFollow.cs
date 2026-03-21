using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;

    [Header("Settings")]
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0f, 0f, 0f);

    void LateUpdate()
    {
        if (player == null) return;

        // Target position (player + offset)
        Vector3 targetPosition = player.position + offset;

        // Smoothly move camera
        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );

        transform.position = smoothedPosition;
    }
}