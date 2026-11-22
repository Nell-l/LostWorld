using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsUI : MonoBehaviour
{
    [Serializable]
    public struct SettingsTab
    {
        public Button tabButton;
        public GameObject contentPanel;
    }

    [Header("Tabs")]
    [SerializeField] private List<SettingsTab> tabs;

    [Header("Audio")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Graphics")]
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown screenModeDropdown;

    [Header("Navigation")]
    [SerializeField] private Button resetButton;
    [SerializeField] private Button backButton;

    [Header("Reset Confirmation")]
    [SerializeField] private GameObject confirmResetPanel;
    [SerializeField] private Button confirmYesButton;
    [SerializeField] private Button confirmNoButton;

    private Resolution[] _resolutions;

    private readonly FullScreenMode[] _screenModes = new FullScreenMode[]
    {
        FullScreenMode.ExclusiveFullScreen,
        FullScreenMode.FullScreenWindow,
        FullScreenMode.Windowed
    };

    private void Start()
    {
        if (confirmResetPanel != null)
        {
            confirmResetPanel.SetActive(false);
        }

        SetupTabs();
        SetupResolutions();
        SetupQuality();
        SetupScreenModes();
        LoadValuesToUI();
        BindEvents();

        if (tabs.Count > 0)
        {
            OpenTab(0);
        }
    }

    private void SetupTabs()
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            int index = i;
            tabs[i].tabButton.onClick.AddListener(() => OpenTab(index));
        }
    }

    private void OpenTab(int index)
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            bool isSelected = i == index;
            tabs[i].contentPanel.SetActive(isSelected);
            tabs[i].tabButton.interactable = !isSelected;
        }
    }

    private void SetupResolutions()
    {
        _resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < _resolutions.Length; i++)
        {
            string option = $"{_resolutions[i].width} x {_resolutions[i].height} ({_resolutions[i].refreshRateRatio.value:0}Hz)";
            options.Add(option);

            if (_resolutions[i].width == Screen.currentResolution.width &&
                _resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);

        if (SettingsManager.Current.ResolutionIndex == -1)
        {
            SettingsManager.Current.ResolutionIndex = currentResolutionIndex;
        }
    }

    private void SetupQuality()
    {
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new List<string>(QualitySettings.names));
    }

    private void SetupScreenModes()
    {
        screenModeDropdown.ClearOptions();
        screenModeDropdown.AddOptions(new List<string>
        {
            "Fullscreen",
            "Borderless windowed",
            "Windowed"
        });
    }

    private void LoadValuesToUI()
    {
        masterSlider.SetValueWithoutNotify(SettingsManager.Current.MasterVolume);
        musicSlider.SetValueWithoutNotify(SettingsManager.Current.MusicVolume);
        sfxSlider.SetValueWithoutNotify(SettingsManager.Current.SFXVolume);

        qualityDropdown.SetValueWithoutNotify(SettingsManager.Current.QualityIndex);
        resolutionDropdown.SetValueWithoutNotify(SettingsManager.Current.ResolutionIndex);

        int modeIndex = Array.IndexOf(_screenModes, SettingsManager.Current.ScreenMode);
        screenModeDropdown.SetValueWithoutNotify(modeIndex >= 0 ? modeIndex : 1);

        qualityDropdown.RefreshShownValue();
        resolutionDropdown.RefreshShownValue();
        screenModeDropdown.RefreshShownValue();
    }

    private void BindEvents()
    {
        masterSlider.onValueChanged.AddListener(OnMasterChanged);
        musicSlider.onValueChanged.AddListener(OnMusicChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXChanged);
        qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        screenModeDropdown.onValueChanged.AddListener(OnScreenModeChanged);

        resetButton.onClick.AddListener(OnResetClicked);
        backButton.onClick.AddListener(OnBackClicked);

        if (confirmYesButton != null) confirmYesButton.onClick.AddListener(OnConfirmResetYes);
        if (confirmNoButton != null) confirmNoButton.onClick.AddListener(OnConfirmResetNo);
    }

    private void OnMasterChanged(float value)
    {
        SettingsManager.Current.MasterVolume = value;
        SettingsManager.ApplyAll();
    }

    private void OnMusicChanged(float value)
    {
        SettingsManager.Current.MusicVolume = value;
        SettingsManager.ApplyAll();
    }

    private void OnSFXChanged(float value)
    {
        SettingsManager.Current.SFXVolume = value;
        SettingsManager.ApplyAll();
    }

    private void OnQualityChanged(int index)
    {
        SettingsManager.Current.QualityIndex = index;
        SettingsManager.ApplyAll();
    }

    private void OnResolutionChanged(int index)
    {
        SettingsManager.Current.ResolutionIndex = index;
        SettingsManager.ApplyAll();
    }

    private void OnScreenModeChanged(int index)
    {
        SettingsManager.Current.ScreenMode = _screenModes[index];
        SettingsManager.ApplyAll();
    }

    private void OnResetClicked()
    {
        if (confirmResetPanel != null)
        {
            confirmResetPanel.SetActive(true);
        }
        else
        {
            OnConfirmResetYes();
        }
    }

    private void OnConfirmResetYes()
    {
        SettingsManager.ResetToDefaults();
        if (SettingsManager.Current.ResolutionIndex == -1)
        {
            SetupResolutions();
        }
        LoadValuesToUI();

        if (confirmResetPanel != null)
        {
            confirmResetPanel.SetActive(false);
        }
    }

    private void OnConfirmResetNo()
    {
        if (confirmResetPanel != null)
        {
            confirmResetPanel.SetActive(false);
        }
    }

    private void OnBackClicked()
    {
        SettingsManager.CloseSettings();
    }
}
