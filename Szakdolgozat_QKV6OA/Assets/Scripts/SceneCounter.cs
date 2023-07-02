using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneCounter : MonoBehaviour
{
    public TMPro.TMP_Text CurrResText;
    public static SceneCounter instance;
    public static bool FromLevelSelector = false;
    public static int MainmenuLoaded = 0;
    public static string CurrentLoadedScene;
    private bool ToggleCurrentResText;
    private bool ToggleLeftShift;

    void Awake()
    {
        //Screen.SetResolution(1920, 1080, true);
        ToggleCurrentResText = false;
        ToggleLeftShift = false;

        if (instance != null)
            Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        CurrentLoadedScene = SceneManager.GetActiveScene().name;
        if (CurrentLoadedScene == "Main menu")
        {
            MainmenuLoaded++;
        }
    }

    void Update()
    {
        ToggleCurrentResTextMethod();
    }

    private void ToggleCurrentResTextMethod()
    {
        if (ToggleCurrentResText) CurrResText.text = "Current resolution: " + Screen.currentResolution.ToString();
        else CurrResText.text = "";
        if (Input.GetKeyDown(KeyCode.LeftShift)) ToggleLeftShift = true;
        if (Input.GetKeyUp(KeyCode.LeftShift)) ToggleLeftShift = false;
        if (Input.GetKeyDown(KeyCode.D) && ToggleLeftShift)
        {
            if (ToggleCurrentResText) ToggleCurrentResText = false;
            else ToggleCurrentResText = true;
        }
    }
}