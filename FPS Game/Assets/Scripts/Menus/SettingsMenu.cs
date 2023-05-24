using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using System;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] TMP_Dropdown resolutionDropdown;

    List<Resolution> resolutions;

    private void Start()
    {
        resolutions = GetUniqueResolutions();

        resolutionDropdown.ClearOptions();

        List<string> resolutionList = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Count; i++)
        {
            string resolution = resolutions[i].width + " x " + resolutions[i].height;
            resolutionList.Add(resolution);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(resolutionList);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public List<Resolution> GetUniqueResolutions()
    {
        Resolution[] resolutions = Screen.resolutions;

        // Filters out all low refresh rate resolutions
        HashSet<Tuple<int, int>> differentResolutions = new HashSet<Tuple<int, int>>();
        Dictionary<Tuple<int, int>, int> maxRefreshRates = new Dictionary<Tuple<int, int>, int>();

        for (int i = 0; i < resolutions.Length; i++) 
        {
            // Add resolutions (if they are not already contained)
            Tuple<int, int> resolution = new Tuple<int, int>(resolutions[i].width, resolutions[i].height);
            differentResolutions.Add(resolution);

            // Get highest framerate:
            if (!maxRefreshRates.ContainsKey(resolution))
                maxRefreshRates.Add(resolution, resolutions[i].refreshRate);
            else
                maxRefreshRates[resolution] = resolutions[i].refreshRate;
        }

        // Build list of different resolutions
        List<Resolution> differentResolutionList = new List<Resolution>();
        foreach (Tuple<int, int> resolution in differentResolutions)
        {
            Resolution newResolution = new Resolution();
            newResolution.width = resolution.Item1;
            newResolution.height = resolution.Item2;
            if (maxRefreshRates.TryGetValue(resolution, out int refreshRate))
                newResolution.refreshRate = refreshRate;
            differentResolutionList.Add(newResolution);
        }
        return differentResolutionList;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", volume);
    }
    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
    }
    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}
