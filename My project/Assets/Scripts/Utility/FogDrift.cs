using UnityEngine;

public class FogDrift : MonoBehaviour
{
    public float speed = 0.2f;
    public float amount = 0.1f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * speed) * amount;
        transform.position = startPos + new Vector3(offset, offset, 0);
    }
}
