using TMPro;
using UnityEngine;

public class CoinUI : MonoBehaviour
{
    public GameController gameController;
    public TextMeshProUGUI coinText;

    void Update()
    {
        coinText.text = "" + gameController.coinCount;
    }
}