using UnityEngine;
using Unity.RemoteConfig;

public class RemoteConfig : MonoBehaviour
{

    public struct userAttributes
    {
        // Optionally declare variables for any custom user attributes; if none keep an empty struct:
        public bool expansionFlag;
    }

    public struct appAttributes
    {
        // Optionally declare variables for any custom app attributes; if none keep an empty struct:
        public int editLevel;
        public bool increaseLevel;
    }

    // Optionally declare a unique assignmentId if you need it for tracking:
    public string assignmentId;

    // Declare any Settings variables you’ll want to configure remotely:
    public int[] spawnIndexes;
    public int[] spawnCounts;

    void Awake()
    {
        // Add a listener to apply settings when successfully retrieved: 
        ConfigManager.FetchCompleted += ApplyRemoteSettings;

        // Set the user’s unique ID:
        ConfigManager.SetCustomUserID("some-user-id");

        // Fetch configuration setting from the remote service: 
        ConfigManager.FetchConfigs<userAttributes, appAttributes>(new userAttributes(), new appAttributes());
    }

    void ApplyRemoteSettings(ConfigResponse configResponse)
    {
        // Conditionally update settings, depending on the response's origin:
        switch (configResponse.requestOrigin)
        {
            case ConfigOrigin.Default:
                Debug.Log("No settings loaded this session; using default values.");
                break;
            case ConfigOrigin.Cached:
                Debug.Log("No settings loaded this session; using cached values from a previous session.");
                break;
            case ConfigOrigin.Remote:

                Debug.Log("New settings loaded this session; update values accordingly.");
                //if (ConfigManager.appConfig.GetInt("levelToEdit") != -1)
                //{
                    for (int i = 0; i < ConfigManager.appConfig.GetInt("newLevelLength"); i++)
                    {
                        spawnIndexes[i] = ConfigManager.appConfig.GetInt($"newSpawnIndex {i}");
                        spawnCounts[i] = ConfigManager.appConfig.GetInt($"newSpawnCount {i}");
                        Debug.Log(spawnCounts[i] + "\n");
                    }

                    GameObject.Find("Manager").GetComponent<GameManager>().EditLevel(ConfigManager.appConfig.GetInt("levelToEdit"), spawnIndexes, spawnCounts);                        
                //}

                if (ConfigManager.appConfig.GetBool("addLevel"))
                {
                    for (int i = 0; i < ConfigManager.appConfig.GetInt("levelLength"); i++)
                    {
                        spawnIndexes[i] = ConfigManager.appConfig.GetInt($"spawnIndex {i}");
                        spawnCounts[i] = ConfigManager.appConfig.GetInt($"spawnCount {i}");
                    }

                 //   GameObject.Find("Manager").GetComponent<GameManager>().AddLevel(spawnIndexes, spawnCounts);
                }

                break;
        }
    }
}
