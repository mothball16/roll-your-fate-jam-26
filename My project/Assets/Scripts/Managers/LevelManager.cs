using Assets.Scripts.Components;
using Assets.Scripts.Managers;
using DangryGames;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.WSA;



public enum Level
{
    Placeholder,
    LevelOne,
    LevelTwo,
    LevelThree
}

[Serializable]
public struct LevelSpawnPoints
{
    public Level Level;
    public Transform Folder;
}

[Serializable]
public struct EnemySpawnDef
{
    public CharType EnemyType;
    public float SpawnWeight;
}

public class LevelManager : MonoSingleton<LevelManager>
{

    [Header("References")]
    public GameController gameController; 
    public List<LevelSpawnPoints> SpawnPoints;
    public List<LevelData> LevelsData;

    private readonly Dictionary<Level, Transform[]> _spawnPoints = new();
    [SerializeField]
    private float _spawnTimer;

    [SerializeField]
    private float _spawnInterval;
    [SerializeField]
    private int _enemiesCurrentlyActive;
    [SerializeField]
    private int _enemiesRemainingInLevel;
    [SerializeField]
    private int _maxOnScreen;

    public int EnemiesLeft => _enemiesRemainingInLevel + _enemiesCurrentlyActive;

    private Level curLevel => gameController.Level;

    void Start()
    {
        if (gameController == null)
        {
            gameController = FindObjectsByType<GameController>().First();
        }

        CacheSpawnPoints();
    }

    public bool HandleLevelLogic()
    {
        _spawnTimer += Time.deltaTime;

        if (_spawnTimer >= _spawnInterval &&
            _enemiesCurrentlyActive < _maxOnScreen &&
            _enemiesRemainingInLevel > 0)
        {
            SpawnEnemy(curLevel);
            _spawnTimer -= _spawnInterval;
        }

        // Win condition
        if (_enemiesRemainingInLevel <= 0 && _enemiesCurrentlyActive <= 0)
        {
            return true;
        }
        return false;
    }

    public void PrepareLevel(Level level)
    {
        LevelData currentData = GetLevelData(level);
        if (currentData == null) return;

        _enemiesRemainingInLevel = currentData.TotalEnemies;
        _enemiesCurrentlyActive = 0;

        _spawnInterval = currentData.SpawnInterval;
        _maxOnScreen = currentData.MaxOnScreen;

        _spawnTimer = _spawnInterval;

        Debug.Log($"--- Level {level} Started ---");
    }



    void SpawnEnemy(Level level)
    {
        var spawnsInLevel = _spawnPoints[level];
        Transform rp = spawnsInLevel[UnityEngine.Random.Range(0, spawnsInLevel.Length)];

        CharType typeToSpawn = GetRandomEnemyTypeForLevel(level);
        GameObject enemy = CharManager.Instance.SpawnChar(typeToSpawn, rp.position);

        if (enemy == null) return;

        _enemiesRemainingInLevel--;
        _enemiesCurrentlyActive++;

        
        if (enemy.TryGetComponent<Humanoid>(out var humanoid))
        {
            humanoid.OnDeath += () =>
            {
                gameController.AddXP(humanoid.XPOnDeath);
                _enemiesCurrentlyActive = Mathf.Max(0, _enemiesCurrentlyActive - 1);
            };
        }
    }

    // ==============================
    // HELPERS
    // ==============================

    public CharType GetRandomEnemyTypeForLevel(Level level)
    {
        LevelData data = GetLevelData(level);
        if (data == null || data.EnemySpawns == null || data.EnemySpawns.Count == 0)
            return CharType.Orc; // Fallback

        float totalWeight = data.EnemySpawns.Sum(x => x.SpawnWeight);
        float randomValue = UnityEngine.Random.Range(0, totalWeight);
        float currentWeight = 0f;

        foreach (var spawnDef in data.EnemySpawns)
        {
            currentWeight += spawnDef.SpawnWeight;
            if (randomValue <= currentWeight)
            {
                return spawnDef.EnemyType;
            }
        }

        return data.EnemySpawns.Last().EnemyType;
    }

    public List<UpgradeData> GetRandomUpgradesForLevel(Level level)
    {
        LevelData data = GetLevelData(level);
        List<UpgradeData> upgrades = new();
        for (int i = 0; i < 3; i++)
        {

            float totalWeight = data.Upgrades.Sum(x => x.Weight);
            float randomValue = UnityEngine.Random.Range(0, totalWeight);
            float currentWeight = 0f;

            foreach (var spawnDef in data.Upgrades)
            {
                currentWeight += spawnDef.Weight;
                if (randomValue <= currentWeight && !upgrades.Contains(spawnDef.Upgrade))
                {
                    upgrades.Add(spawnDef.Upgrade);
                }
            }
        }
        return upgrades;
    }

    private LevelData GetLevelData(Level level)
    {
        return LevelsData.FirstOrDefault(x => x.LevelIdentifier == level);
    }


    void CacheSpawnPoints()
    {
        foreach(var LevelSpawnPoints in SpawnPoints)
        {
            var level = LevelSpawnPoints.Level;
            var folder = LevelSpawnPoints.Folder;
            _spawnPoints[level] = new Transform[folder.childCount];
            for (int i = 0; i < folder.childCount; i++)
            {
                _spawnPoints[level][i] = folder.GetChild(i);
            }
        }

    }
}