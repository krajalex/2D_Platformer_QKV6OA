using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject MainMenu_Panel;
    [SerializeField] private GameObject MainMenu_Settings_Panel;
    [SerializeField] private GameObject MainMenu_GameSettings_Panel;
    [SerializeField] private GameObject MainMenu_GraphicsSettings_Panel;
    [SerializeField] private GameObject MainMenu_SoundSettings_Panel;
    [SerializeField] private GameObject MainMenu_Info_Panel;
    [SerializeField] private GameObject MainMenu_KinectInfo_Panel;

    [SerializeField] private Slider Main_MainVolumeSlider;
    [SerializeField] private Slider Main_GameMusicVolumeSlider;
    [SerializeField] private Slider Main_GameEffectsVolumeSlider;

    [SerializeField] private GameObject Last_Panel_lvl1;
    [SerializeField] private GameObject Last_Panel_lvl2;

    private int MainMenu_level;

    private bool BackButtonPressed;
    public static bool EscMainmenu = false;

    void Start()
    {
        //if (SceneCounter.MainmenuLoaded == 1)
        //{
        //    DoNotDestroyAudioSource.instance.GetComponent<AudioSource>().Play();
        //}
        //if (SceneCounter.MainmenuLoaded > 1 && SceneCounter.FromLevelSelector == false)
        //{
        //    DoNotDestroyAudioSource.instance.GetComponent<AudioSource>().Play();
        //}
        if (SceneCounter.FromLevelSelector == false)
        {
            DoNotDestroyAudioSource.instance.GetComponent<AudioSource>().Play();
        }

        Main_MainVolumeSlider.value = SettingsMenu.MainVolume_Value_Out;
        Main_GameMusicVolumeSlider.value = SettingsMenu.GameMusicVolume_Value_Out;
        Main_GameEffectsVolumeSlider.value = SettingsMenu.GameEffectsVolume_Value_Out;
        QualitySettings.SetQualityLevel(SettingsMenu.qualityIndex_Out);

        MouseCursorManager.CursorConfined_Visible();
        MainMenu_level = 0;
        BackButtonPressed = false;
        Last_Panel_lvl1 = MainMenu_Settings_Panel;
        Last_Panel_lvl2 = MainMenu_GameSettings_Panel;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            if (EscMainmenu)
            {
                MouseCursorManager.CursorConfined_Visible();
            }
            else
            {
                MouseCursorManager.CursorNone_Visible();
            }
        }
        CheckInput();
    }

    private void FixedUpdate()
    {
        if (BackButtonPressed) Back();
        LoadCurrentPanel();
    }

    private void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && MainMenu_level != 0)
        {
            BackButtonPressed = true;
        }
    }

    public void LoadCurrentPanel()
    {        
        if (MainMenu_level == 0)
        {
            MainMenu_Panel.SetActive(true);
            Last_Panel_lvl1.SetActive(false);
            Last_Panel_lvl2.SetActive(false);
        }
        if (MainMenu_level == 1)
        {
            MainMenu_Panel.SetActive(false);
            Last_Panel_lvl1.SetActive(true);
            Last_Panel_lvl2.SetActive(false);
        }
        if (MainMenu_level == 2)
        {
            MainMenu_Panel.SetActive(false);
            Last_Panel_lvl1.SetActive(false);
            Last_Panel_lvl2.SetActive(true);
        }
    }

    public void Back()
    {
        MainMenu_level -= 1;
        BackButtonPressed = false;
    }

    //Az elsõ pálya betöltése, a játék indítása.
    public void PlayGame()
    {
        //SceneManager.SetActiveScene(SceneManagerOwn.scene2);
        SceneManager.LoadScene(1);
    }

    public void GeneralSettingsPanel()
    {
        MainMenu_level = 1;
        Last_Panel_lvl1 = MainMenu_Settings_Panel;
    }

    public void GameSettingsPanel()
    {
        MainMenu_level = 2;
        Last_Panel_lvl1 = MainMenu_Settings_Panel;
        Last_Panel_lvl2 = MainMenu_GameSettings_Panel;
    }

    public void GraphicsSettingsPanel()
    {
        MainMenu_level = 2;
        Last_Panel_lvl1 = MainMenu_Settings_Panel;
        Last_Panel_lvl2 = MainMenu_GraphicsSettings_Panel;
    }

    public void SoundSettingsPanel()
    {
        MainMenu_level = 2;
        Last_Panel_lvl1 = MainMenu_Settings_Panel;
        Last_Panel_lvl2 = MainMenu_SoundSettings_Panel;
    }

    public void InfoPanel()
    {
        MainMenu_level = 1;
        Last_Panel_lvl1 = MainMenu_Info_Panel;
    }

    public void KinectInfoPanel()
    {
        MainMenu_level = 2;
        Last_Panel_lvl1 = MainMenu_Info_Panel;
        Last_Panel_lvl2 = MainMenu_KinectInfo_Panel;
    }

    //A játék bezárása.
    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayKinect()
    {
        LevelSelector.MainmenuMusicPause();
        SceneManager.LoadScene("KinectDemo");
    }
}
