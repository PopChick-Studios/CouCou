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

        foreach (Resolution resolution in resolutionsWithoutCap)
        {
            Debug.Log("Before: " + resolution);
        }

        Resolution[] resolutionsWithSmall = resolutionsWithoutCap.GroupBy(x => x.height).Select(x => x.First()).ToArray();

        foreach (Resolution resolution in resolutionsWithSmall)
        {
            Debug.Log("After: " + resolution);
        }

        finalResolutions = resolutionsWithSmall;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        
        for (int i = 0; i < resolutionsWithSmall.Length; i++)
        {
            if (resolutionsWithSmall[i].width < 1280 || resolutionsWithSmall[i].height < 720)
            {
                finalResolutions = finalResolutions.Where(val => val.width != resolutionsWithSmall[i].width && val.height != resolutionsWithSmall[i].height).ToArray();
                Debug.Log("The resolution " + resolutionsWithSmall[i].width + " x " + resolutionsWithSmall[i].height + " was removed because it's too small");
            }
        }

        Debug.Log("Avaliable resolutions:");

        int currentResolutionsIndex = 0;
        for (int i = 0; i < finalResolutions.Length; i++)
        {
            string option = finalResolutions[i].width + " x " + finalResolutions[i].height;
            options.Add(option);
            Debug.Log(option);

            if (finalResolutions[i].width == Screen.currentResolution.width && finalResolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionsIndex = i;
            }
        }

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
