using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Managers;
using JetBrains.Annotations;
using System;
[Serializable]
public struct UpgradeEntry
{
    public UpgradeData Upgrade;
    public int Weight;
}

[CreateAssetMenu(fileName = "New Level Data", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    public Level LevelIdentifier;

    [Header("Spawns")]
    public List<EnemySpawnDef> EnemySpawns;
    public List<UpgradeEntry> Upgrades;

    [Header("Difficulty Settings")]
    public int MaxOnScreen = 3;
    public int TotalEnemies = 5;
    public float SpawnInterval = 5f;
}