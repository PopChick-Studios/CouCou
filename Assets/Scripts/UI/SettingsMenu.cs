using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using System;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown qualityDropdown;

    Resolution[] finalResolutions;

    private void Start()
    {
        Resolution[] resolutionsWithoutCap = Screen.resolutions;

        string debugString = "Before: ";
        foreach (Resolution resolution in resolutionsWithoutCap)
        {
            debugString += resolution + ", ";
        }
        Debug.Log(debugString);
        debugString = "After: ";

        Resolution[] resolutionsWithSmall = resolutionsWithoutCap.GroupBy(x => x.height).Select(x => x.First()).ToArray();

        foreach (Resolution resolution in resolutionsWithSmall)
        {
            debugString += resolution + ", ";
        }
        Debug.Log(debugString);

        finalResolutions = resolutionsWithSmall;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        debugString = "The resolutions: ";

        for (int i = 0; i < resolutionsWithSmall.Length; i++)
        {
            if (resolutionsWithSmall[i].width < 1280 || resolutionsWithSmall[i].height < 720)
            {
                finalResolutions = finalResolutions.Where(val => val.width != resolutionsWithSmall[i].width && val.height != resolutionsWithSmall[i].height).ToArray();
                debugString += resolutionsWithSmall[i].width + " x " + resolutionsWithSmall[i].height + ", ";
            }
        }

        debugString += "were removed because they were too small.";
        Debug.Log(debugString);

        debugString = "Avaliable resolutions: ";

        int currentResolutionsIndex = 0;
        for (int i = 0; i < finalResolutions.Length; i++)
        {
            string option = finalResolutions[i].width + " x " + finalResolutions[i].height;
            options.Add(option);
            debugString += option + ", ";

            if (finalResolutions[i].width == Screen.currentResolution.width && finalResolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionsIndex = i;
            }
        }

        Debug.Log(debugString);

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionsIndex;
        resolutionDropdown.RefreshShownValue();

        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
    }

    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int index)
    {
        Resolution resolution = finalResolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}
