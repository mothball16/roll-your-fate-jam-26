using UnityEngine;

public class XObjectManager : MonoBehaviour
{
    [Header("Setup")]
    public Transform folder;        // Parent containing objects
    public GameObject prefab;       // Object to spawn

    [Header("Settings")]
    public float spacing = 2f;      // Distance between objects

    Transform GetClosestOnX()
    {
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (Transform child in folder)
        {
            float distance = Mathf.Abs(child.position.x - transform.position.x);

            if (distance < minDistance)
            {
                minDistance = distance;
                closest = child;
            }
        }

        return closest;
    }
    public void RemoveClosest()
    {
        Transform closest = GetClosestOnX();

        if (closest != null)
        {
            Destroy(closest.gameObject);
        }
    }

    public void AddNext()
    {
        Transform closest = GetClosestOnX();

        Vector3 spawnPos;

        if (closest != null)
        {
            // Decide direction (left or right of closest)
            float direction = Mathf.Sign(transform.position.x - closest.position.x);
            if (direction == 0) direction = 1; // fallback

            spawnPos = closest.position + new Vector3(direction * spacing, 0f, 0f);
        }
        else
        {
            // If no children exist, spawn at folder position
            spawnPos = folder.position;
        }

        Instantiate(prefab, spawnPos, Quaternion.identity, folder);
    }
}