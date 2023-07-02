using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Timer : MonoBehaviour
{
    public bool stopwatchActive = false;
    float currentTime;
    public Text currentTimeText;

    public int timeScore;
    public int score = 1000;
    public Text scoreText;
    public float multiplier = 3;
    
    // Start is called before the first frame update
    void Start() //a p�lya kezdetekor az id�sz�ml�l� alaphelyzetve ker�l, �s az elindul
    {
        currentTime = 0;
        stopwatchActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (stopwatchActive==true) 
        {
            currentTime = currentTime + Time.deltaTime; //folyamatosan adja hozz� az eltelt id�t
            timeScore = Mathf.RoundToInt(currentTime * multiplier); //ez a pontsz�m ker�l majd levon�sra a p�lya v�g�n az �sszpontsz�mb�l, ami alapb�l 1000 (a pontsz�m az eltelt id� egy szorzata)
            scoreText.text = score.ToString(); //ez a pontsz�m a p�lya sor�n ker�l kijelz�sre, ellenben a timeScore-ral (az csak a Congrats scene-ben ker�l felhaszn�l�sra)
            TimeSpan time = TimeSpan.FromSeconds(currentTime);
            currentTimeText.text = time.ToString(@"mm\:ss\:fff"); //az id�sz�ml�l� ki�rat�s�nak form�z�sa
        }
    }
}
