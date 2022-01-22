using TMPro;
using UnityEngine;

public class StatsDisplay : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] bool Coin = false;
    TextMeshProUGUI t;
    void Start()
    {
        if (!Coin)
            GetComponent<TextMeshProUGUI>().text = (GameManager.instance.CurrentLevel + 1).ToString();
        else
        {
            t = GetComponent<TextMeshProUGUI>();
            GameManager.instance.OnCurrencyChange += UpdateCoin;
            UpdateCoin();
        }

        GameManager.instance.OnGameEnd += Start;
    }
    void UpdateCoin()
    {
        t.text = (GameManager.instance.Currency).ToString();
    }
}
