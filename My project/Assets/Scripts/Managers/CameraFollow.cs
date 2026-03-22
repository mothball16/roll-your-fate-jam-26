using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;

    public float smoothTime = 0.2f;
    public Vector3 offset = new Vector3(0f, 0f, -10f);
    private Vector3 _velocity = Vector3.zero;

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 targetPosition = player.position + offset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref _velocity,
            smoothTime
        );
    }
}