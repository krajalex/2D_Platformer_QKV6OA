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
    void Start() //a pálya kezdetekor az idõszámláló alaphelyzetve kerül, és az elindul
    {
        currentTime = 0;
        stopwatchActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (stopwatchActive==true) 
        {
            currentTime = currentTime + Time.deltaTime; //folyamatosan adja hozzá az eltelt idõt
            timeScore = Mathf.RoundToInt(currentTime * multiplier); //ez a pontszám kerül majd levonásra a pálya végén az összpontszámból, ami alapból 1000 (a pontszám az eltelt idõ egy szorzata)
            scoreText.text = score.ToString(); //ez a pontszám a pálya során kerül kijelzésre, ellenben a timeScore-ral (az csak a Congrats scene-ben kerül felhasználásra)
            TimeSpan time = TimeSpan.FromSeconds(currentTime);
            currentTimeText.text = time.ToString(@"mm\:ss\:fff"); //az idõszámláló kiíratásának formázása
        }
    }
}
