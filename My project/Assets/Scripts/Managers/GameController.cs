using Assets.Scripts.Attacks;
using Assets.Scripts.Components;
using Assets.Scripts.Managers;
using DangryGames;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum BattleState
{
    LevelSetup,
    LevelActive,
    LevelIntermission,
    
}
public class GameController : MonoBehaviour
{
    public BattleState currentState = BattleState.LevelSetup;
    public Level Level = Level.Placeholder;
    public GameObject Player;

    public int XPLevel;
    public float XP;
    static public int[] XPReqs = new int[] { 5, 10, 20, 30, 40, 50, 60, 70, 80, 100};

    [Header("Scenes")]
    public string WinSceneName = "WinScene";
    public string LoseSceneName = "LoseScene";

    private LevelManager levelManager;
    private CharManager charManager;
    private GameUIManager uiManager;

    public List<UpgradeData> bought;

    public AudioClip? win;

    public void Start()
    {
        bought = new();
        // get refs
        levelManager = LevelManager.Instance;
        charManager = CharManager.Instance;
        uiManager = GameUIManager.Instance;
        uiManager.ShopExitCallback = FinishShop;

        // setup shit
        Player = charManager.SpawnChar(CharType.Player, Vector2.zero);
        var cameraFollow = Camera.main.GetComponent<CameraFollow>();
        cameraFollow.player = Player.transform;

        uiManager.SetPlayer(Player);

        if (Player.TryGetComponent<Humanoid>(out var playerHumanoid))
        {
            playerHumanoid.OnDeath += LoseGame;
        }
    }

    public void AddXP(int add)
    {
        XP += add;
        if (Player != null)
            Player.GetComponent<Humanoid>().TakeDamage(-add);
        if (XP >= XPReqs[XPLevel])
        {
            XP = 0;
            XPLevel = Mathf.Min(XPLevel+1, XPReqs.Length - 1);

            OpenShop();
        }
    }
    private void OpenShop()
    {
        Time.timeScale = 0.1f;
        var upgrades = levelManager.GetRandomUpgradesForLevel(Level);
        uiManager.OpenShopUI(bought, upgrades);
    }

    public void FinishShop(UpgradeData upgrade)
    {
        if (upgrade != null)
            ApplyUpgrade(upgrade);
        Time.timeScale = 1f;
    }



    public void ApplyUpgrade(UpgradeData upgrade)
    {
        if (!Player.TryGetComponent<CharacterController>(out var charController))
        {
            Debug.LogWarning("no charcontroller in player");
            return;
        }
        if (!Player.TryGetComponent<Humanoid>(out var humanoid))
        {
            Debug.LogWarning("no humanoid in player");
            return;
        }
        if (!Player.TryGetComponent<AutoAttacker>(out var attacker))
        {
            Debug.LogWarning("no autoattacker in player");
            return;
        }
        humanoid.MaxHealth += upgrade.MaxHealthBoost;
        humanoid.TakeDamage(-upgrade.HealthBoost);

        charController.SetMaxSpeed(charController.MaxSpeed + upgrade.SpeedBoost);

        if (upgrade.GiveAttack)
        {
            var newAttack = new AttackState
            {
                AttackDef = upgrade.GiveAttack,
                CurCooldown = upgrade.GiveAttack.Cooldown
            };
            attacker.Attacks.Add(newAttack);
        }

        bought.Add(upgrade);
    }


    public void WinGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(0);
    }

    public void LoseGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(0);
    }

    public void Update()
    {
        switch (currentState)
        {
            case BattleState.LevelActive:
                bool wonLevel = levelManager.HandleLevelLogic();
                if (wonLevel)
                    currentState = BattleState.LevelIntermission;
                break;
            case BattleState.LevelIntermission:
                currentState = BattleState.LevelSetup;
                break;
            case BattleState.LevelSetup:
                int nextLevelIndex = (int)Level + 1;
                AudioSource.PlayClipAtPoint(win, Camera.main.transform.position);
                if (!System.Enum.IsDefined(typeof(Level), nextLevelIndex))
                    WinGame();
                else
                    Level = (Level)nextLevelIndex;

                levelManager.PrepareLevel(Level);
                currentState = BattleState.LevelActive;
                break;
        }
    }

}