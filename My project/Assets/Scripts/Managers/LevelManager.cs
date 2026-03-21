using UnityEngine;
using Assets.Scripts.Managers;
using Assets.Scripts.Components;

public enum LevelState
{
    LevelActive,
    ShopMenu,
    LevelTransition
}

public class LevelManager : MonoBehaviour
{
    public LevelState currentState;

    [Header("Level Settings")]
    public int currentLevel = 1;
    private int enemiesRemainingInLevel;
    private int enemiesCurrentlyActive;

    [Header("Spawner Config")]
    public Transform folder;
    [SerializeField] private float baseSpawnInterval = 2f;
    [SerializeField] private int baseMaxOnScreen = 5;

    [Header("Enemy Type")]
    public CharType enemyType = CharType.Skeleton;

    private float spawnInterval;
    private int maxOnScreen;

    private Transform[] spawnPoints;
    private float spawnTimer;

    [SerializeField] private Canvas shopCanvas;

    void Start()
    {
        CacheSpawnPoints();
        shopCanvas.gameObject.SetActive(false); // ✅ start hidden
        TransitionToState(LevelState.LevelActive);
    }

    void Update()
    {
        if (currentState == LevelState.LevelActive)
        {
            HandleLevelLogic();
        }
    }

    // ==============================
    // STATE MANAGEMENT
    // ==============================

    public void TransitionToState(LevelState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case LevelState.LevelActive:
                PrepareLevel();
                break;

            case LevelState.ShopMenu:
                OpenShopUI();
                break;

            case LevelState.LevelTransition:
                StartNextLevel();
                break;
        }
    }

    // ==============================
    // LEVEL LOGIC
    // ==============================

    private void HandleLevelLogic()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval &&
            enemiesCurrentlyActive < maxOnScreen &&
            enemiesRemainingInLevel > 0)
        {
            SpawnEnemy();
            spawnTimer -= spawnInterval;
        }

        // Win condition
        if (enemiesRemainingInLevel <= 0 && enemiesCurrentlyActive <= 0)
        {
            TransitionToState(LevelState.ShopMenu);
        }
    }

    private void PrepareLevel()
    {
        enemiesRemainingInLevel = 5 + (currentLevel * 5); // scalable
        enemiesCurrentlyActive = 0;

        spawnInterval = Mathf.Max(0.5f, baseSpawnInterval - currentLevel * 0.1f);
        maxOnScreen = baseMaxOnScreen + currentLevel;

        spawnTimer = spawnInterval; // spawn immediately

        shopCanvas.gameObject.SetActive(false); // ✅ hide shop

        Debug.Log($"--- Level {currentLevel} Started ---");
    }

    private void OpenShopUI()
    {
        shopCanvas.gameObject.SetActive(true);
        Debug.Log("Level Clear! Opening Shop...");
    }

    public void FinishShopping()
    {
        shopCanvas.gameObject.SetActive(false); // ❗ FIXED (was true)
        Debug.Log("Closing Shop...");
        TransitionToState(LevelState.LevelTransition);
    }

    private void StartNextLevel()
    {
        currentLevel++;
        TransitionToState(LevelState.LevelActive);
    }

    // ==============================
    // SPAWNING (CHAR MANAGER)
    // ==============================

    void SpawnEnemy()
    {
        Transform rp = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject enemy = CharManager.Instance.SpawnChar(enemyType, rp.position);

        if (enemy == null) return;

        enemiesRemainingInLevel--; // ✅ correct
        enemiesCurrentlyActive++;

        // Hook death event
        Humanoid humanoid = enemy.GetComponent<Humanoid>();

        if (humanoid != null)
        {
            humanoid.OnDeath += () =>
            {
                enemiesCurrentlyActive = Mathf.Max(0, enemiesCurrentlyActive - 1);
            };
        }
    }

    // ==============================
    // HELPERS
    // ==============================

    void CacheSpawnPoints()
    {
        spawnPoints = new Transform[folder.childCount];

        for (int i = 0; i < folder.childCount; i++)
        {
            spawnPoints[i] = folder.GetChild(i);
        }
    }
}