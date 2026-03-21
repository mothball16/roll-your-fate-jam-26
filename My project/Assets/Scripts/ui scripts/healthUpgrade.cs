using UnityEngine;
using UnityEngine.UI;

public class healthUpgrade : MonoBehaviour
{
    public RectTransform healthBar; // the UI bar
    public int maxHealth = 100;
    public int currentHealth = 100;

    public float widthPerHealth = 2f; // how much width each health adds

    void Start()
    {
        UpdateBarSize();
    }

    public void UpgradeHealth(int amount)
    {
        maxHealth += amount;
        currentHealth = maxHealth; // optional full heal
        UpdateBarSize();
    }

    void UpdateBarSize()
    {
        float newWidth = maxHealth * widthPerHealth;
        healthBar.sizeDelta = new Vector2(newWidth, healthBar.sizeDelta.y);
    }
}