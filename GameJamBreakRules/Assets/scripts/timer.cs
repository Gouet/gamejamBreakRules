using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class timer : MonoBehaviour {
    public float allowedTime;
    private Text comp;
    private float timeleft;

	// Use this for initialization
	void Start () {
        comp = gameObject.GetComponent<Text>();
        startTimer();
	}

    public void startTimer()
    {
        timeleft = allowedTime;
        displayTime();
    }

    void displayTime()
    {
        int minutes = Mathf.FloorToInt(timeleft / 60F);
        int seconds = Mathf.FloorToInt(timeleft - minutes * 60);
        comp.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }
	
	// Update is called once per frame
	void Update () {
        if (timeleft > 0)
        {
            timeleft -= Time.deltaTime;
            if (timeleft < 0)
                timeleft = 0;
            displayTime();
        }
	}
    
    public float getTimer()
    {
        return timeleft;
    }
}
