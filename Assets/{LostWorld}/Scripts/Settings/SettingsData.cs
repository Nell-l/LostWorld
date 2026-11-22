using System;
using UnityEngine;

[Serializable]
public class SettingsData
{
    public float MasterVolume = 1f;
    public float MusicVolume = 1f;
    public float SFXVolume = 1f;

    public int QualityIndex = 2;
    public int ResolutionIndex = -1;
    public FullScreenMode ScreenMode = FullScreenMode.FullScreenWindow;

}
