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
        Resolution[] resolutionsWithSmall = resolutionsWithoutCap.GroupBy(x => x.height).Select(x => x.First()).ToArray();
        Resolution[] resolutionsWithoutWeirdRatios = resolutionsWithSmall;

        finalResolutions = resolutionsWithSmall;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        string debugString = "The resolutions: ";

        for (int i = 0; i < resolutionsWithSmall.Length; i++)
        {
            if (resolutionsWithSmall[i].width < 1280 || resolutionsWithSmall[i].height < 720)
            {
                resolutionsWithoutWeirdRatios = resolutionsWithoutWeirdRatios.Where(val => val.width != resolutionsWithSmall[i].width && val.height != resolutionsWithSmall[i].height).ToArray();
                debugString += resolutionsWithSmall[i].width + " x " + resolutionsWithSmall[i].height + ", ";
            }
        }
        debugString += "were removed because they were too small.\nThese resolutions: ";


        finalResolutions = resolutionsWithoutWeirdRatios;

        for (int i = 0; i < resolutionsWithoutWeirdRatios.Length; i++)
        {
            if ((16/9f) / (resolutionsWithoutWeirdRatios[i].width / (float)resolutionsWithoutWeirdRatios[i].height) > 1.2)
            {
                finalResolutions = finalResolutions.Where(val => val.width != resolutionsWithoutWeirdRatios[i].width && val.height != resolutionsWithoutWeirdRatios[i].height).ToArray();
                debugString += resolutionsWithoutWeirdRatios[i].width + " x " + resolutionsWithoutWeirdRatios[i].height + ", ";
            }
        }
        debugString += "had weird ratios";
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
        if (PlayerPrefs.HasKey("resolutionIndex"))
        {
            resolutionDropdown.value = PlayerPrefs.GetInt("resolutionIndex");
        }
        else
        {
            resolutionDropdown.value = currentResolutionsIndex;
        }
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
        PlayerPrefs.SetInt("resolutionIndex", index);
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}
