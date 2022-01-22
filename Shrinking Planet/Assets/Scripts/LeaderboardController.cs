using UnityEngine.UI;
using LootLocker.Requests;
using UnityEngine;

public class LeaderboardController : MonoBehaviour
{
    public InputField PlayerName;
    public GameObject LeaderboardCanvas;
    public int ID;
    int maxScores = 6;
    public Text[] Entries;
    public Text[] Ranks;

    private void Start()
    {
        LootLockerSDKManager.StartSession("Player", (response) =>
        {
            if (response.success)
            {
               // Debug.Log("Success");
            }
            else
            {
               // Debug.Log("Failed");
            }
        });
    }

    public void ShowScores()
    {
        LeaderboardCanvas.SetActive(true);
        LootLockerSDKManager.GetScoreList(ID, maxScores, (response) =>
        {
            if (response.success)
            {
                LootLockerLeaderboardMember[] scores = response.items;

                for (int i = 0; i < scores.Length; i++)
                {
                    Entries[i].text = (scores[i].rank + ". " + scores[i].member_id);
                    Ranks[i].text = scores[i].score.ToString();
                }

                if(scores.Length < maxScores)
                {
                    for (int i = scores.Length; i < maxScores; i++)
                    {
                        Entries[i].text = (i + 1).ToString() + ". ";
                        Ranks[i].text = " ";
                    }
                }
            }
            else
            {
              //  Debug.Log("Failed");
            }
        }
        );
    }

    public void QuitLeaderboard()
    {
        LeaderboardCanvas.SetActive(false);
    }

    public void SubmitScore()
    {
        LootLockerSDKManager.SubmitScore(PlayerName.text, int.Parse(PlayerPrefs.GetFloat("HiScore").ToString("0.#")), ID, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Success");
            }
            else
            {
                Debug.Log("Failed");
            }
        });
    }
}
