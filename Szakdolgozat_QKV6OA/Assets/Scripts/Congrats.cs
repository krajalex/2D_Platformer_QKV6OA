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
        //CongratsText.text = "Gratul�lunk! Teljes�tetted a(z) " + ((PlayerMovement.lastSceneNumber) - 1).ToString() + ". p�ly�t!";
        //ScoreText.text = "A(z) " + ((PlayerMovement.lastSceneNumber) - 1).ToString() + ". p�ly�n el�rt v�gleges pontsz�mod:"; //ki�rja a p�ly�n teljes�tett v�gleges pontsz�mot
        //FinalScore.text = PlayerMovement.finalScore.ToString(); //megkapja a p�ly�n teljes�tett pontnak az �rt�k�t
    }

    //A k�vetkez� p�ly�t t�lti be.
    public void NextLevel()
    {
        //SceneManager.LoadScene(PlayerMovement.lastSceneNumber + 1);
    }

    //A f�men�t t�lti be.
    public void MainMenu()
    {
        SceneManager.LoadScene("Mainmenu");
    }
}