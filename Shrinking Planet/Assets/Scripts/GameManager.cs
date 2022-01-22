using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	public GameObject gameOverUI;

	void Awake ()
	{
		instance = this;
	}

	public void EndGame ()
	{
		if(PlayerPrefs.GetFloat("HiScore") < Planet.Score)
		   PlayerPrefs.SetFloat("HiScore", Planet.Score);

		gameOverUI.SetActive(true);
	}

	public void Restart ()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

}
