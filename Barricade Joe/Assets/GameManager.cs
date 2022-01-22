using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] RectTransform MainMenu;
    [SerializeField] RectTransform GamePlayUi;
    [SerializeField] RectTransform WinPanel;
    [SerializeField] RectTransform EndPanel;
    [SerializeField] Slider ProgressBar;
    [SerializeField] TextMeshProUGUI[] LevelText;

    [Header("Spawn Enemy AI")]
    [SerializeField] List<string> EnemyPoolTags = new List<string>();
    [SerializeField] Transform[] SpawnPoints;

    [Header("Levels")]
    public bool spawn = true;
    [SerializeField] Level[] levels;

    [Header("Audio Voice Over")]
    [SerializeField] Audio GameStart;
    [SerializeField] Audio GameWon;
    [SerializeField] Audio BarricadeDestroyedSound;
    public static GameManager instance;
    [HideInInspector] public bool GameEnded = false;
    [HideInInspector] public List<GameObject> Ais = new List<GameObject>();
    public event Action OnGameEnd = delegate { };
    public event Action OnCurrencyChange = delegate { };
    private int currency;

    public int Currency
    {
        get { return PlayerPrefs.GetInt("Coin", 0); }
        set { PlayerPrefs.SetInt("Coin", value); OnCurrencyChange?.Invoke(); }
    }
    public void AdjustCoins(int amount)
    {
        Currency += amount;
    }
    private void Awake()
    {
        instance = this;
    }
    public void GameEnd()
    {
        FindObjectOfType<UbhShotCtrl>().StopShotRoutine();
        GameEnded = true;
        OnGameEnd?.Invoke();

    }

    private void Start()
    {
        for (int i = 0; i < LevelText.Length; i++)
        {
            if (CurrentLevel + i <= levels.Length - 1)
                LevelText[i].text = (CurrentLevel + 1 + i).ToString();
            else
                LevelText[i].text = string.Empty;
        }
        if (CurrentLevel > levels.Length - 1)
        {
            MainMenu.gameObject.SetActive(false);
            EndPanel.gameObject.SetActive(true);
            CurrentLevel = 0;
        }
        Time.timeScale = 0;
    }


    IEnumerator SpawnEnemy()
    {
        if (CurrentLevel > levels.Length - 1)
            yield break;
        foreach (int item in levels[CurrentLevel].SpawnCount)
        {
            ProgressBar.maxValue += item;
        }
        bool First = true;
        for (int i = 0; i < levels[CurrentLevel].SpawnIndexes.Length; i++)
        {
            Transform sp = SpawnPoints.GetRandom();
            for (int j = 0; j < levels[CurrentLevel].SpawnCount[i]; j++)
            {
                if (First)
                {
                    Ais.Add(ObjectPooler.instance.SpawnFromPool(EnemyPoolTags[levels[CurrentLevel].SpawnIndexes[i]], SpawnPoints[0].position, Quaternion.identity));
                    First = false;
                }
                else
                    Ais.Add(ObjectPooler.instance.SpawnFromPool(EnemyPoolTags[levels[CurrentLevel].SpawnIndexes[i]], sp.position, Quaternion.identity));

            }
            yield return new WaitForSeconds(Random.Range(.1f, 3));
        }
        yield return null;
    }
    public void RegisterDeath(GameObject whoDied)
    {
        if (Ais.Contains(whoDied))
        {
            Ais.Remove(whoDied);
            ProgressBar.value++;
            if (ProgressBar.value == ProgressBar.maxValue)
            {
                Timer.Register(1f, () => PlayerWon());
            }
        }
    }

    private void PlayerWon()
    {
        if (GameEnded)
            return;
        GamePlayUi.gameObject.SetActive(false);
        WinPanel.gameObject.SetActive(true);
        AudioManager.instance.PlayAudio(GameWon);
        Currency += 10;
        CurrentLevel++;
        FindObjectOfType<UbhShotCtrl>().StopShotRoutine();
        OnGameEnd?.Invoke();
    }

    public void StartGame()
    {
        if (MainMenu != null)
        {
            MainMenu.gameObject.SetActive(false);
        }
        if (GamePlayUi != null)
            GamePlayUi.gameObject.SetActive(true);
        AudioManager.instance.ButtonSound();
        Timer.Register(1f, () => AudioManager.instance.PlayAudio(GameStart));
        if (spawn)
            StartCoroutine(SpawnEnemy());
    }
    public void PlaySound(AudioClip s)
    {
        AudioManager.instance.PlayAudio(s);
    }
    public void BarricadeDestroyed()
    {
        if (!GameEnded)
            AudioManager.instance.PlayAudio(BarricadeDestroyedSound);
    }

    public int CurrentLevel
    {
        get { return PlayerPrefs.GetInt("Level", 0); }
        set { PlayerPrefs.SetInt("Level", value); }
    }

    [ContextMenu("Clear Level key")]
    public void ClearLevelKey()
    {
        PlayerPrefs.DeleteKey("Level");
        Debug.Log("Level key is deleted");
    }

    [System.Serializable]
    public struct Level
    {
        public int[] SpawnIndexes;
        public int[] SpawnCount;
    }

    public void EditLevel(int level, int[] spawnIndexes, int[] spawnCounts)
    {
        for(int i = 0; i < spawnIndexes.Length; i++)
        {
            levels[level-1].SpawnIndexes[i] = spawnIndexes[i];
            levels[level-1].SpawnCount[i] = spawnCounts[i];
        }
    }

    public void AddLevel(int[] spawnIndexes, int[] spawnCounts)
    {
        for (int i = 0; i < spawnIndexes.Length; i++)
        {
            levels[levels.Length + 1].SpawnIndexes[i] = spawnIndexes[i];
            levels[levels.Length + 1].SpawnCount[i] = spawnCounts[i];
        }
    }
}

