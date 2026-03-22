using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Managers;
using Assets.Scripts.Attacks;

[CreateAssetMenu(fileName = "New Upgrade Data", menuName = "Game/Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    public string Title;
    public string Description;
    public int MaxHealthBoost;
    public int HealthBoost;
    public int SpeedBoost;
    public Attack GiveAttack;
}