using UnityEngine;

public class ShopBridge : MonoBehaviour
{
    public void CloseShop()
    {
        LevelManager lm = FindObjectOfType<LevelManager>();

        if (lm != null)
        {
            lm.FinishShopping();
        }
        else
        {
            Debug.LogWarning("LevelManager not found!");
        }
    }
}