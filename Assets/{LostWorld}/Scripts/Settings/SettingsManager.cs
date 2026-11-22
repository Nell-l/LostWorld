using System;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public static class SettingsManager
{
    private static SettingsData _current;
    public static SettingsData Current
    {
        get
        {
            if (_current == null) Initialize();
            return _current;
        }
        private set => _current = value;
    }

    public static event Action OnSettingsChanged;
    public static bool IsSettingsOpen => SceneManager.GetSceneByName(SettingsSceneName).isLoaded;

    private static AudioMixer _mixer;
    private const string SettingsSceneName = "SettingsScene";
    private static string FilePath => Path.Combine(Application.persistentDataPath, "settings.json");

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticData()
    {
        _current = null;
        _mixer = null;
        OnSettingsChanged = null;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        _mixer = Resources.Load<AudioMixer>("MainMixer");
        Load();
        ApplyAll();
    }

    public static void OpenSettings()
    {
        if (IsSettingsOpen) return;

        SceneManager.LoadSceneAsync(SettingsSceneName, LoadSceneMode.Additive);
    }

    public static void CloseSettings()
    {
        if (!IsSettingsOpen) return;

        Save();
        SceneManager.UnloadSceneAsync(SettingsSceneName);
    }

    public static void ResetToDefaults()
    {
        _current = new SettingsData();
        _current.QualityIndex = QualitySettings.GetQualityLevel();
        Save();
    }

    public static void Save()
    {
        string json = JsonUtility.ToJson(Current, true);
        File.WriteAllText(FilePath, json);
        ApplyAll();
        OnSettingsChanged?.Invoke();
    }

    public static void Load()
    {
        if (File.Exists(FilePath))
        {
            string json = File.ReadAllText(FilePath);
            _current = JsonUtility.FromJson<SettingsData>(json);
        }
        else
        {
            _current = new SettingsData();
            _current.QualityIndex = QualitySettings.GetQualityLevel();
        }
    }

    public static void ApplyAll()
    {
        SetMixerVolume("MasterVolume", Current.MasterVolume);
        SetMixerVolume("MusicVolume", Current.MusicVolume);
        SetMixerVolume("SFXVolume", Current.SFXVolume);

        QualitySettings.SetQualityLevel(Current.QualityIndex, true);

        Resolution[] resolutions = Screen.resolutions;
        if (Current.ResolutionIndex >= 0 && Current.ResolutionIndex < resolutions.Length)
        {
            Resolution res = resolutions[Current.ResolutionIndex];
            Screen.SetResolution(res.width, res.height, Current.ScreenMode);
        }
        else
        {
            Screen.fullScreenMode = Current.ScreenMode;
        }
    }

    private static void SetMixerVolume(string parameterName, float value)
    {
        if (_mixer == null) return;
        float db = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        _mixer.SetFloat(parameterName, db);
    }
}
