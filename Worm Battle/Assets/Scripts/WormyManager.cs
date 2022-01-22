using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WormyManager : MonoBehaviour
{
    Wormy[] wormies;
    public Transform wormyCamera;

    public static WormyManager singleton;

    public bool fire { set; get; }
    private int currentWormy;
    public Text PingText;
    public Text PlayerName;

    void Start()
    {
        fire = false;
        if (singleton != null)
        {
            Destroy(gameObject);
            return;
        }

        singleton = this;

        wormies = GameObject.FindObjectsOfType<Wormy>();
        wormyCamera = Camera.main.transform;

        for (int i = 0; i < wormies.Length; i++)
        {
            wormies[i].wormId = i;
        }
    }

    private void Awake()
    {
        PlayerName.text = PhotonNetwork.playerName;
    }

    private void Update()
    {
        PingText.text = "PING: " + PhotonNetwork.GetPing();
    }

    public void NextWorm()
    {
        fire = false;
        StartCoroutine(NextWormCoroutine());
        //StartCoroutine(TimeBeforeFire());
    }

    public IEnumerator NextWormCoroutine()
    {
        var nextWorm = currentWormy + 1;
        currentWormy = -1;

        yield return new WaitForSeconds(2);

        currentWormy = nextWorm;
        if (currentWormy >= wormies.Length)
        {
            currentWormy = 0;
        }

        wormyCamera.SetParent(wormies[currentWormy].transform);
        wormyCamera.localPosition = Vector3.zero + Vector3.back * 10;
    }


    public bool IsMyTurn(int i)
    {
        return i == currentWormy;
    }
}
