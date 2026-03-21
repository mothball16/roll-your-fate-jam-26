using TMPro;
using UnityEngine;

public class levelUp : MonoBehaviour
{
    int count = 1;

    [SerializeField]
    public TextMeshProUGUI text;

    public void addLevel()
    {
        text.text = "Level " + count;
        count++;
    }
}
