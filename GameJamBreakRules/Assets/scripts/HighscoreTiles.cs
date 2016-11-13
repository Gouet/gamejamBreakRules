using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class HighscoreTiles : MonoBehaviour {

    public Text highscores;

	// Use this for initialization
	public void initHighscore() {
		string txt = "";
		string baselvl = "level_";
		int i = 1;

		DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Levels");
		int max = dir.GetFiles("level_*.unity").Length;
		while (i <= max)
		{
			txt += "Niveau " + i.ToString() + ": ";
			for (int j = 1; j <= 3; ++j)
			{
				txt += PlayerPrefs.GetFloat(baselvl + i.ToString() + "_" + j.ToString()).ToString();
				if (j != 3)
					txt += " - ";
			}
			txt += "\r\n";
			++i;
		}
		if (max > 20)
			highscores.rectTransform.sizeDelta = new Vector2(highscores.rectTransform.rect.width, highscores.rectTransform.rect.height + (max - 20) * 20);
		highscores.text = txt;
	}

	void Start () {
		initHighscore ();
	}
}
