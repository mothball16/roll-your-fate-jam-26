using Assets.Scripts.Managers;
using DangryGames;
using UnityEngine;


public class GameController : MonoBehaviour
{
    public int level;
    public int health;
    public int bulletDamage;
    public int meleeDamage;
    public int bulletCount;
    public int coinCount;
    public float speedFactor;

    public int healthTier = 0;
    public int bulletTier = 0;
    public int meleeTier = 0;
    public int bulletCountTier = 0;
    public int speedTier = 0;

    public CharManager charManager = CharManager.Instance;
    public void Awake()
    {
        charManager.SpawnChar(CharType.Player, Vector2.zero);
    }


}