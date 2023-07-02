using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Internal;
using UnityEngine.UI;
using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using UnityEngine.XR;
using Microsoft;
using Debug = UnityEngine.Debug;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] public AudioMixer MainVolumeMixer;
    [SerializeField] public TMPro.TMP_Dropdown SetScreenDropdown;
    [SerializeField] public TMPro.TMP_Dropdown ResolutionDropdown;
    [SerializeField] public TMPro.TMP_Dropdown AspectRatioDropdown;
    [SerializeField] public TMPro.TMP_Dropdown RefreshRateDropdown;
    [SerializeField] public TMPro.TMP_Text CurrResText;
    [SerializeField] public TMPro.TMP_Text CurrRefrText;
    public static float MainVolume_Value_Out;
    public static float GameMusicVolume_Value_Out;
    public static float GameEffectsVolume_Value_Out;
    public static int qualityIndex_Out;
    public static List<Resolution> Finallysortedresolutions = new List<Resolution>();
    public static Resolution[] resolutions;
    public static FullScreenMode[] screenModes = { FullScreenMode.ExclusiveFullScreen, FullScreenMode.FullScreenWindow, FullScreenMode.MaximizedWindow, FullScreenMode.Windowed };
    public static List<Resolution> refreshRates = new List<Resolution>();
    public static List<int> refreshRatesint = new List<int>();

    public static double aspectRatio_16_9 = 1.8;
    public static double aspectRatio_16_10 = 1.7;
    public static double aspectRatio_4_3 = 1.3;
    public static List<double> aspectRatioDiv = new List<double>() { aspectRatio_16_9, aspectRatio_16_10, aspectRatio_4_3 };
    public static string AspectRatio_16_9 = "16:9";
    public static string AspectRatio_16_10 = "16:10";
    public static string AspectRatio_4_3 = "4:3";
    public static List<string> aspectRatioOptionsStr = new List<string>() { AspectRatio_16_9, AspectRatio_16_10, AspectRatio_4_3 };
    public static double lastAspectRatioIndex;
    public static double lastAspectRatio;
    public static int NewAspRatioValueFound;
    public static bool FromSetAspectRatio = false;
    public static bool FromSetResolution = false;

    void Update()
    {
        //CurrResText.text = "Current resolution: " + Screen.currentResolution.ToString() + "képfr: " + Screen.currentResolution.refreshRate;
    }

    void Start()
    {
        #region StartSetScreen
        SetScreenDropdown.ClearOptions();
        List<string> setScreenOptions = new List<string>();
        int currentScreenSetIndex = 0;
        for (int i = 0; i < screenModes.Length; i++)
        {
            string setScreenOption = screenModes[i].ToString();
            setScreenOptions.Add(setScreenOption);
            if (screenModes[i] == Screen.fullScreenMode)
            {
                currentScreenSetIndex = i;
            }
        }
        SetScreenDropdown.AddOptions(setScreenOptions);
        SetScreenDropdown.value = currentScreenSetIndex;
        SetScreenDropdown.RefreshShownValue();
        #endregion StartSetScreen

        #region StartResolution
        resolutions = Screen.resolutions;
        ResolutionDropdown.ClearOptions();
        for (int i = 0; i < resolutions.Length; i++)
        {
            double IResAsp = Convert.ToDouble(resolutions[i].width) / Convert.ToDouble(resolutions[i].height);
            double IResAspRounded = Math.Round(IResAsp, 1);
            bool IsRightAspectRatioResolution = false;
            for (int j = 0; j < aspectRatioDiv.Count; j++)
            {
                if (IResAspRounded == aspectRatioDiv[j])
                {
                    IsRightAspectRatioResolution = true;
                    break;
                }
            }
            bool GotThatRes = false;
            for (int j = 0; j < Finallysortedresolutions.Count; j++)
            {
                if ((Finallysortedresolutions[j].width == resolutions[i].width) && (Finallysortedresolutions[j].height == resolutions[i].height))
                {
                    GotThatRes = true;
                }
            }
            if (IsRightAspectRatioResolution && !GotThatRes)
            {
                Finallysortedresolutions.Add(resolutions[i]);
            }
        }
        List<string> resolutionOptions = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < Finallysortedresolutions.Count; i++)
        {
            string resolutionOption = Finallysortedresolutions[i].width.ToString() + " x " + Finallysortedresolutions[i].height.ToString() + " " + Finallysortedresolutions[i].refreshRate.ToString() + " Hz";
            resolutionOptions.Add(resolutionOption);
            if (Finallysortedresolutions[i].width == Screen.width && Finallysortedresolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }
        ResolutionDropdown.AddOptions(resolutionOptions);
        ResolutionDropdown.value = currentResolutionIndex;
        ResolutionDropdown.RefreshShownValue();
        #endregion StartResolution

        #region StartRefreshRate
        RefreshRateDropdown.ClearOptions();
        List<string> refreshRateOptions = new List<string>();
        int currentRefreshRate = 0;
        for (int i = 0; i < Finallysortedresolutions.Count; i++)
        {
            Debug.Log("finallysortedress count start refrate");
            bool GotRefreshRate = false;
            if (refreshRatesint.Count > 0)
            {
                //Debug.Log("van eltárolva refrate a listában");
                for (int j = 0; j < refreshRatesint.Count; j++)
                {
                    if (refreshRatesint[j] == Convert.ToInt32(Finallysortedresolutions[i].refreshRate))
                    {
                        GotRefreshRate = true;
                        break;
                    }
                }
            }
            else
            {
                Debug.Log("addrefrate, 0 elem volt");
                refreshRatesint.Add(Convert.ToInt32(Finallysortedresolutions[i].refreshRate));
                string refreshrateOption = refreshRatesint[i].ToString() + "Hz";
                refreshRateOptions.Add(refreshrateOption);
            }
            if (!GotRefreshRate)
            {
                Debug.Log("addrefrate");
                refreshRatesint.Add(Convert.ToInt32(Finallysortedresolutions[i].refreshRate));
                string refreshrateOption = refreshRatesint[i].ToString() + "Hz";
                refreshRateOptions.Add(refreshrateOption);
            }
        }
        for (int i = 0; i < refreshRatesint.Count; i++)
        {
            Debug.Log("eltárolt refrate db: " + refreshRatesint.Count);
            if (refreshRatesint[i] == Convert.ToInt32(Screen.currentResolution.refreshRate))
            {
                Debug.Log("megvan");
                currentRefreshRate = i;
            }
        }
        RefreshRateDropdown.AddOptions(refreshRateOptions);
        RefreshRateDropdown.value = currentRefreshRate;
        Debug.Log("jelenlegi refr: " + refreshRatesint[currentRefreshRate]);
        RefreshRateDropdown.RefreshShownValue();
        #endregion StartRefreshRate

        #region StartAspectRatio
        AspectRatioDropdown.ClearOptions();
        int currentAspectRatioIndex = 0;
        for (int i = 0; i < aspectRatioDiv.Count; i++)
        {
            double currentAspectRatioBoot = Convert.ToDouble(Screen.width) / Convert.ToDouble(Screen.width);
            double currentAspectRatioBootDIV = Math.Round(currentAspectRatioBoot, 1);
            if (aspectRatioDiv[i] == currentAspectRatioBootDIV)
            {
                currentAspectRatioIndex = i;
            }
        }
        AspectRatioDropdown.AddOptions(aspectRatioOptionsStr);
        AspectRatioDropdown.value = currentAspectRatioIndex;
        AspectRatioDropdown.RefreshShownValue();
        #endregion StartAspectRatio
    }

    #region VoiceSettings
    public void Set_MainVolume(float MainVolume_Value)
    {
        MainVolumeMixer.SetFloat("MainVolume", MainVolume_Value);
        MainVolume_Value_Out = MainVolume_Value;
    }
    public void Set_MainmenuMusicVolume(float MainmenuMusicVolume_Value)
    {
        MainVolumeMixer.SetFloat("MainmenuMusicVolume", MainmenuMusicVolume_Value);
    }
    public void Set_GameMusicVolume(float GameMusicVolume_Value)
    {
        MainVolumeMixer.SetFloat("GameMusicVolume", GameMusicVolume_Value);
        GameMusicVolume_Value_Out = GameMusicVolume_Value;
    }
    public void Set_GameEffectsVolume(float GameEffectsVolume_Value)
    {
        MainVolumeMixer.SetFloat("GameEffectsVolume", GameEffectsVolume_Value);
        GameEffectsVolume_Value_Out = GameEffectsVolume_Value;
    }
    #endregion VoiceSettings

    #region GraphicsSettings
    public void SetscreenMethod(int SETScreenIndex)
    {
        Screen.fullScreenMode = screenModes[SETScreenIndex];
    }

    public void SetAspectRatioMethod(int SETAspectRatioIndex)
    {
        AspectRatioDropdown.SetValueWithoutNotify(SETAspectRatioIndex);
        if (FromSetResolution == false)
        {
            double NewAspectRatio = aspectRatioDiv[SETAspectRatioIndex];
            double IFOldResolutionDiv = Convert.ToDouble(Screen.width) / Convert.ToDouble(Screen.height);
            double IFOldResolutionDivRounded = Math.Round(IFOldResolutionDiv, 1);
            if (IFOldResolutionDivRounded != NewAspectRatio)
            {
                bool GotIndex = false;
                int SETRES = 0;
                for (int i = Finallysortedresolutions.Count - 1; i > -1; i--)
                {
                    if (GotIndex)
                    {
                        FromSetAspectRatio = true;
                        SetResolutionMethod(SETRES);
                        break;
                    }
                    else
                    {
                        double SearchNewResolutionDiv = Convert.ToDouble(Finallysortedresolutions[i].width) / Convert.ToDouble(Finallysortedresolutions[i].height);
                        double SearchNewResolutionDivRounded = Math.Round(SearchNewResolutionDiv, 1);
                        if (SearchNewResolutionDivRounded == NewAspectRatio)
                        {
                            SETRES = i;
                            GotIndex = true;
                        }
                    }
                }
            }
        }
        if (FromSetResolution)
        {
            FromSetResolution = false;
        }
    }

    public void SetResolutionMethod(int SETResolutionIndex)
    {
        ResolutionDropdown.SetValueWithoutNotify(SETResolutionIndex);
        if (FromSetAspectRatio == false)
        {
            double OldAspectRatioDiv = Convert.ToDouble(Screen.width) / Convert.ToDouble(Screen.height);
            double OldAspectRatioDivRounded = Math.Round(OldAspectRatioDiv, 1);
            Resolution SETresolution = Finallysortedresolutions[SETResolutionIndex];
            bool CurrentFullScreen = Screen.fullScreen;
            Screen.SetResolution(SETresolution.width, SETresolution.height, CurrentFullScreen);
            double IFNewResolutionDiv = Convert.ToDouble(SETresolution.width) / Convert.ToDouble(SETresolution.height);
            double IFNewResolutionDivRounded = Math.Round(IFNewResolutionDiv, 1);
            if (IFNewResolutionDivRounded != OldAspectRatioDivRounded)
            {
                double SearchNewAspectRatio = 0;
                int i = 0;
                do
                {
                    SearchNewAspectRatio = aspectRatioDiv[i];
                    if (SearchNewAspectRatio == IFNewResolutionDivRounded)
                    {
                        NewAspRatioValueFound = i;
                        FromSetResolution = true;
                        SetAspectRatioMethod(NewAspRatioValueFound);
                        break;
                    }
                    if (SearchNewAspectRatio != IFNewResolutionDivRounded)
                    {
                        i++;
                        continue;
                    }
                } while (IFNewResolutionDivRounded != SearchNewAspectRatio);
            }
        }
        if (FromSetAspectRatio)
        {
            Resolution SETresolution = Finallysortedresolutions[SETResolutionIndex];
            bool CurrentFullScreen = Screen.fullScreen;
            Screen.SetResolution(SETresolution.width, SETresolution.height, CurrentFullScreen);
            FromSetAspectRatio = false;
        }
    }

    public void SetRefreshRateMethod(int SETRefreshRateIndex)
    {
        Debug.Log("setrefrate method");
        RefreshRateDropdown.SetValueWithoutNotify(SETRefreshRateIndex);
        Resolution CurrentRes = Screen.currentResolution;
        bool CurrentFullScreen = Screen.fullScreen;
        int SetRefreshRate = refreshRatesint[SETRefreshRateIndex];
        Screen.SetResolution(CurrentRes.width, CurrentRes.height, CurrentFullScreen, SetRefreshRate);
        Display.main.Activate(CurrentRes.width, CurrentRes.height, SetRefreshRate);
        Application.targetFrameRate = SetRefreshRate;
        //QualitySettings.vSyncCount = SetRefreshRate;
        //CurrRefrText.text = "Res: " + Screen.currentResolution.ToString() + "RefrR: " + Screen.currentResolution.refreshRate + "targetframerate: " + Application.targetFrameRate;
    }

    public void SetGraphicsQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        qualityIndex_Out = qualityIndex;
    }
    #endregion GraphicsSettings
}