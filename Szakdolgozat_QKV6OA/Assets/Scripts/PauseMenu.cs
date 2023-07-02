using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    //Változó, ami figyeli, hogy a játék meg van-e állítva.
    public bool GameIsPaused = false;

    [SerializeField] private GameObject PauseMenu_Panel;
    [SerializeField] private GameObject PauseMenu_Settings_Panel;
    [SerializeField] private GameObject PauseMenu_GameSettings_Panel;
    [SerializeField] private GameObject PauseMenu_GraphicsSettings_Panel;
    [SerializeField] private GameObject PauseMenu_SoundSettings_Panel;
    [SerializeField] private Slider Pause_MainVolumeSlider;
    [SerializeField] private Slider Pause_GameMusicVolumeSlider;
    [SerializeField] private Slider Pause_GameEffectsVolumeSlider;

    private GameObject PauseMenu_Last_Panel_lvl1;
    private GameObject PauseMenu_Last_Panel_lvl2;

    private int PauseMenu_level;

    private bool PauseMenu_BackButtonPressed;

    private void Start()
    {
        QualitySettings.SetQualityLevel(SettingsMenu.qualityIndex_Out);
        Pause_MainVolumeSlider.value = SettingsMenu.MainVolume_Value_Out;
        Pause_GameMusicVolumeSlider.value = SettingsMenu.GameMusicVolume_Value_Out;
        Pause_GameEffectsVolumeSlider.value = SettingsMenu.GameEffectsVolume_Value_Out;
        Resume();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        if (GameIsPaused)
        {
            if (PauseMenu_BackButtonPressed) Back();
            LoadCurrentPanel();
        }
    }

    /// <summary>
    /// A pausemenu objektum eltüntetése, az idõ múlásának visszaállítása és a gameispaused változó értéke false-ra változik.
    /// </summary>

    private void CheckInput()
    {
        //Ha a megnyomott gomb az Escape, akkor megnézzük, hogy a game éppen megvan-e állítva, ha igen akkor Unpause-oljuk, ellenkezõ esetben meg megállítjuk.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if ((PauseMenu_level == 1 || PauseMenu_level == 2) && GameIsPaused)
            {
                PauseMenu_BackButtonPressed = true;
            }

            if (GameIsPaused && PauseMenu_level == 0)
            {
                PauseMenu_level = -1;
                Resume();
            }
            else
            {
                if (!GameIsPaused) //beállítja a paneleket
                {
                    PauseMenu_level = 0;
                    PauseMenu_Last_Panel_lvl1 = PauseMenu_Settings_Panel;
                    PauseMenu_Last_Panel_lvl2 = PauseMenu_GameSettings_Panel;
                    Pause(); //meghívja a Pause metódust
                }
            }
        }
    }

    public void LoadCurrentPanel() //megnézi, hogy melyik fülön vagyunk, és aktiválja a megfelelõ paneleket
    {
        if (PauseMenu_level == 0)
        {
            PauseMenu_Panel.SetActive(true);
            PauseMenu_Last_Panel_lvl1.SetActive(false);
            PauseMenu_Last_Panel_lvl2.SetActive(false);
        }
        if (PauseMenu_level == 1)
        {
            PauseMenu_Panel.SetActive(false);
            PauseMenu_Last_Panel_lvl1.SetActive(true);
            PauseMenu_Last_Panel_lvl2.SetActive(false);
        }
        if (PauseMenu_level == 2)
        {
            PauseMenu_Panel.SetActive(false);
            PauseMenu_Last_Panel_lvl1.SetActive(false);
            PauseMenu_Last_Panel_lvl2.SetActive(true);
        }
    }

    public void Back() //egy szinttel visszalép
    {
        Debug.Log("Back");
        PauseMenu_level -= 1;
        PauseMenu_BackButtonPressed = false;
    }

    public void Resume() //kilép a menübõl, lockolja a kurzort, és folytatódik a játék
    {
        PauseMenu_Panel.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        MouseCursorManager.CursorConfined_NotVisible();
    }

    // A PauseMenu aktívra állítása, az idõmúlás megállítása és a gameispaused változó true értékre állítása.
    void Pause()
    {
        PauseMenu_Panel.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        MouseCursorManager.CursorConfined_Visible();
    }

    public void GeneralSettingsPanel() //itt kerülnek definiálásra a menü paneljeinek elérési útvonalai, adatai
    {
        Debug.Log("Settings");
        PauseMenu_level = 1;
        PauseMenu_Last_Panel_lvl1 = PauseMenu_Settings_Panel;
    }

    public void GameSettingsPanel() //itt kerülnek definiálásra a menü paneljeinek elérési útvonalai, adatai
    {
        Debug.Log("GameSettings");
        PauseMenu_level = 2;
        PauseMenu_Last_Panel_lvl1 = PauseMenu_Settings_Panel;
        PauseMenu_Last_Panel_lvl2 = PauseMenu_GameSettings_Panel;
    }
    public void GraphicsSettingsPanel() //itt kerülnek definiálásra a menü paneljeinek elérési útvonalai, adatai
    {
        PauseMenu_level = 2;
        PauseMenu_Last_Panel_lvl1 = PauseMenu_Settings_Panel;
        PauseMenu_Last_Panel_lvl2 = PauseMenu_GraphicsSettings_Panel;
    }
    public void SoundSettingsPanel() //itt kerülnek definiálásra a menü paneljeinek elérési útvonalai, adatai
    {
        PauseMenu_level = 2;
        PauseMenu_Last_Panel_lvl1 = PauseMenu_Settings_Panel;
        PauseMenu_Last_Panel_lvl2 = PauseMenu_SoundSettings_Panel;
    }

    public void RestartCheckPoint()
    {
        Resume();
    }

    public void RestartLevel() //újraindítja a pályát
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //Az idõmúlás elindítása és a fõmenü betöltése.
    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Mainmenu");
    }
    //A játék bezárása.
    public void QuitGame()
    {
        Application.Quit();
    }
}
