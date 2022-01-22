using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpButton : MonoBehaviour
{
    [SerializeField] float loadDelay = 10;
    [SerializeField] int powerUpTime = 5;
    [SerializeField] UbhShotCtrl Ubh;
    [SerializeField] Image Loader;
    [SerializeField] TextMeshProUGUI TimeText;
    Button b;
    // Start is called before the first frame update
    void Start()
    {
        b = GetComponent<Button>();
        b.interactable = false;
        StartCoroutine(Loading());
    }
    IEnumerator Loading()
    {
        Loader.DOFillAmount(0, 0);
        Loader.DOFillAmount(1, loadDelay);
        yield return new WaitForSeconds(loadDelay);
        b.interactable = true;
    }
    public void ShotActivated()
    {
        StartCoroutine(Shooting());
    }
    IEnumerator Shooting()
    {
        Loader.DOFillAmount(0, 0);
        int t = powerUpTime;
        TimeText.gameObject.SetActive(true);
        for (int i = 0; i < powerUpTime; i++)
        {
            TimeText.text = t.ToString();
            yield return new WaitForSeconds(1);
            t--;
        }
        TimeText.gameObject.SetActive(false);
        Ubh.StopShotRoutine();
        StartCoroutine(Loading());
    }
}
