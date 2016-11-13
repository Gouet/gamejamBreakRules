﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuBehaviour : MonoBehaviour {
    public void onClickPlay()
    {
        SceneManager.LoadScene("Levels/level_1");
    }

    public void onClickHighscores()
    {
        SceneManager.LoadScene("scenes/Highscores"); 
    }

    public void onClickQuit()
    {
        Application.Quit();
    }

	public void onClickHome()
	{
		SceneManager.LoadScene("scenes/Menu");
	}

	public void onClickReset()
	{
		PlayerPrefs.DeleteAll();
		if (SceneManager.GetActiveScene ().name == "Highscores")
			GameObject.Find ("Main Camera").GetComponent<HighscoreTiles> ().initHighscore ();
	}
}
