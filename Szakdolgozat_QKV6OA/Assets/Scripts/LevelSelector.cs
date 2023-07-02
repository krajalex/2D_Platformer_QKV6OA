using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    public Button[] levelButtons;
    public GameObject loadingScreen;
    public Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        int levelAt = PlayerPrefs.GetInt("levelAt", 2); //a jelenlegi p�lya sz�m�nak elt�rol�sa

        for (int i = 0; i < levelButtons.Length; i++) //a gombok sz�m�nak n�z�se
        {
            if (i + 2 > levelAt) //megn�zi, hogy a gomb nagyobb-e, mint ami el�rhet�
            {
                levelButtons[i].interactable = false; //inakt�vv� teszi a gombot, ha teljes�l a fel�tel
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LoadMenu();
        }
    }

    public void ResetProgress() //vissza�ll�tja az alap�llapotot (ne legyen mentett, teljes�tett p�lya)
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("LevelSelector");
    }
    //public void PlayLevel_1() //bet�lti az els� p�ly�t
    //{
    //    MainmenuMusicPause();
    //    SceneManager.LoadScene("Level_1");
    //}
    //public void PlayLevel_2()
    //{
    //    MainmenuMusicPause();
    //    SceneManager.LoadScene("Level_2");
    //}
    //public void PlayLevel_3()
    //{
    //    MainmenuMusicPause();
    //    SceneManager.LoadScene("Level_3");
    //}
    public void LoadLevel(string name)
    {
        MainmenuMusicPause();
        StartCoroutine(LoadAsync(name));
    }

    IEnumerator LoadAsync(string name)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(name);

        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);

            slider.value = progress;

            yield return null;
        }    
    }

    public void LoadMenu() //bet�lti a men�t
    {
        SceneCounter.FromLevelSelector = true;
        SceneManager.LoadScene(0);
    }

    public static void MainmenuMusicPause()
    {
        DoNotDestroyAudioSource.instance.GetComponent<AudioSource>().Pause();
        SceneCounter.FromLevelSelector = false;
    }
}