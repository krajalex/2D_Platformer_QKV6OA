using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    //V�ltoz�, ami figyeli, hogy a j�t�k meg van-e �ll�tva.
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
    /// A pausemenu objektum elt�ntet�se, az id� m�l�s�nak vissza�ll�t�sa �s a gameispaused v�ltoz� �rt�ke false-ra v�ltozik.
    /// </summary>

    private void CheckInput()
    {
        //Ha a megnyomott gomb az Escape, akkor megn�zz�k, hogy a game �ppen megvan-e �ll�tva, ha igen akkor Unpause-oljuk, ellenkez� esetben meg meg�ll�tjuk.
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
                if (!GameIsPaused) //be�ll�tja a paneleket
                {
                    PauseMenu_level = 0;
                    PauseMenu_Last_Panel_lvl1 = PauseMenu_Settings_Panel;
                    PauseMenu_Last_Panel_lvl2 = PauseMenu_GameSettings_Panel;
                    Pause(); //megh�vja a Pause met�dust
                }
            }
        }
    }

    public void LoadCurrentPanel() //megn�zi, hogy melyik f�l�n vagyunk, �s aktiv�lja a megfelel� paneleket
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

    public void Back() //egy szinttel visszal�p
    {
        Debug.Log("Back");
        PauseMenu_level -= 1;
        PauseMenu_BackButtonPressed = false;
    }

    public void Resume() //kil�p a men�b�l, lockolja a kurzort, �s folytat�dik a j�t�k
    {
        PauseMenu_Panel.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        MouseCursorManager.CursorConfined_NotVisible();
    }

    // A PauseMenu akt�vra �ll�t�sa, az id�m�l�s meg�ll�t�sa �s a gameispaused v�ltoz� true �rt�kre �ll�t�sa.
    void Pause()
    {
        PauseMenu_Panel.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        MouseCursorManager.CursorConfined_Visible();
    }

    public void GeneralSettingsPanel() //itt ker�lnek defini�l�sra a men� paneljeinek el�r�si �tvonalai, adatai
    {
        Debug.Log("Settings");
        PauseMenu_level = 1;
        PauseMenu_Last_Panel_lvl1 = PauseMenu_Settings_Panel;
    }

    public void GameSettingsPanel() //itt ker�lnek defini�l�sra a men� paneljeinek el�r�si �tvonalai, adatai
    {
        Debug.Log("GameSettings");
        PauseMenu_level = 2;
        PauseMenu_Last_Panel_lvl1 = PauseMenu_Settings_Panel;
        PauseMenu_Last_Panel_lvl2 = PauseMenu_GameSettings_Panel;
    }
    public void GraphicsSettingsPanel() //itt ker�lnek defini�l�sra a men� paneljeinek el�r�si �tvonalai, adatai
    {
        PauseMenu_level = 2;
        PauseMenu_Last_Panel_lvl1 = PauseMenu_Settings_Panel;
        PauseMenu_Last_Panel_lvl2 = PauseMenu_GraphicsSettings_Panel;
    }
    public void SoundSettingsPanel() //itt ker�lnek defini�l�sra a men� paneljeinek el�r�si �tvonalai, adatai
    {
        PauseMenu_level = 2;
        PauseMenu_Last_Panel_lvl1 = PauseMenu_Settings_Panel;
        PauseMenu_Last_Panel_lvl2 = PauseMenu_SoundSettings_Panel;
    }

    public void RestartCheckPoint()
    {
        Resume();
    }

    public void RestartLevel() //�jraind�tja a p�ly�t
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //Az id�m�l�s elind�t�sa �s a f�men� bet�lt�se.
    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Mainmenu");
    }
    //A j�t�k bez�r�sa.
    public void QuitGame()
    {
        Application.Quit();
    }
}
