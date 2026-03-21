using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Setup")]
    public Transform folder;        // Parent containing spawn points
    public GameObject prefab;

    [SerializeField]
    private int enemyToSpawn = 10;

    [SerializeField]
    private int maxEnemies = 5;

    [SerializeField]
    private float spawnInterval = 2f;

    private float timer = 0f;
    private Transform[] spawnPoints;

    private int enemyCount;


    void Start()
    {
        // Cache all spawn points
        spawnPoints = new Transform[folder.childCount];
        for (int i = 0; i < folder.childCount; i++)
        {
            spawnPoints[i] = folder.GetChild(i);
        }
    }

    void Update()
    {
        if (enemyToSpawn <= 0) return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval && enemyCount < maxEnemies)
        {
            timer = 0f;

            // Pick random spawn point
            Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Spawn enemy
            Instantiate(prefab, randomPoint.position, Quaternion.identity);

            enemyCount++;
            enemyToSpawn--;
        }
    }
}