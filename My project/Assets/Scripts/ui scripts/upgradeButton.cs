using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum UpgradeType
{
    Health,
    BulletDamage,
    MeleeDamage,
    BulletCount,
    Speed
}

public class UpgradeButton : MonoBehaviour
{
    [Header("References")]
    public GameController gameController;
    public Slider slider;
    public TextMeshProUGUI buttonText;

    [Header("Upgrade Settings")]
    public UpgradeType upgradeType;

    public int maxLevel = 5;
    private int currentLevel = 0;

    public int cost = 10;

    void Start()
    {
        slider.maxValue = maxLevel;
        slider.value = currentLevel;

        UpdateText();
    }

    public void Upgrade()
    {
        // Max level check
        if (currentLevel >= maxLevel)
            return;

        // Not enough coins
        if (gameController.coinCount < cost)
            return;

        // Spend coins
        gameController.coinCount -= cost;

        // Increase level
        currentLevel++;
        slider.value = currentLevel;

        // Increase cost
        cost *= 2;

        // Apply upgrade
        switch (upgradeType)
        {
            case UpgradeType.Health:
                gameController.health += 2;
                gameController.healthTier++;
                break;

            case UpgradeType.BulletDamage:
                gameController.bulletDamage += 2;
                gameController.bulletTier++;
                break;

            case UpgradeType.MeleeDamage:
                gameController.meleeDamage += 2;
                gameController.meleeTier++;
                break;

            case UpgradeType.BulletCount:
                gameController.bulletCount += 2;
                gameController.bulletCountTier++;
                break;

            case UpgradeType.Speed:
                gameController.speedFactor += 1.5f;
                gameController.speedTier++;
                break;
        }

        UpdateText();
    }

    void UpdateText()
    {
        if (currentLevel >= maxLevel)
        {
            buttonText.text = "MAX";
        }
        else
        {
            buttonText.text = "" + cost;
        }
    }

    void Update()
    {
        // Disable button if can't afford or maxed
        GetComponent<Button>().interactable =
            gameController.coinCount >= cost && currentLevel < maxLevel;
    }
}