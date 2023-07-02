using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainVolumeController : MonoBehaviour
{
    public Slider volumeSlider;
    public GameObject ObjectMusic;

    //Value from the slider, and it converts to volume level
    private float MusicVolume = 0f;
    private AudioSource AudioSource;

    // Start is called before the first frame update
    private void Start()
    {
        ObjectMusic = GameObject.FindWithTag("MainmenuMusic_Tag");
        AudioSource = ObjectMusic.GetComponent<AudioSource>();

        //Set Volume
        MusicVolume = PlayerPrefs.GetFloat("volumeabc");
        AudioSource.volume = MusicVolume;
        volumeSlider.value = MusicVolume;
    }

    // Update is called once per frame
    private void Update()
    {
        AudioSource.volume = MusicVolume;
        PlayerPrefs.SetFloat("volumeabc", MusicVolume);
    }

    public void VolumeUpdater(float volumeabc)
    {
        MusicVolume = volumeabc;
    }

    public void MusicReset()
    {
        PlayerPrefs.DeleteKey("volumeabc");
        AudioSource.volume = 1;
        volumeSlider.value = 1;
    }
}
