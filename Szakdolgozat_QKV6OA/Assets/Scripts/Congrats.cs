using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class Congrats : MonoBehaviour
{
    [SerializeField] public Text CongratsText;
    [SerializeField] public Text ScoreText;
    [SerializeField] public Text FinalScore;

    public static bool EscCongrats = false;
    
    void Start()
    {
        MouseCursorManager.CursorConfined_Visible();
        ChangeText();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (EscCongrats)
            {
                MouseCursorManager.CursorConfined_Visible();
            }
            else
            {
                MouseCursorManager.CursorNone_Visible();
            }
        }
    }

    public void ChangeText()
    {
        //CongratsText.text = "Gratulálunk! Teljesítetted a(z) " + ((PlayerMovement.lastSceneNumber) - 1).ToString() + ". pályát!";
        //ScoreText.text = "A(z) " + ((PlayerMovement.lastSceneNumber) - 1).ToString() + ". pályán elért végleges pontszámod:"; //kiírja a pályán teljesített végleges pontszámot
        //FinalScore.text = PlayerMovement.finalScore.ToString(); //megkapja a pályán teljesített pontnak az értékét
    }

    //A következõ pályát tölti be.
    public void NextLevel()
    {
        //SceneManager.LoadScene(PlayerMovement.lastSceneNumber + 1);
    }

    //A fõmenüt tölti be.
    public void MainMenu()
    {
        SceneManager.LoadScene("Mainmenu");
    }
}