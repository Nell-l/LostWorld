using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public void Start()
    {
    }

    public void OnPlayBtnClicked()
    {
        SceneLoader.LoadScene("GameScene");
    }

    public void OnSettingsBtnClicked()
    {
        SettingsManager.OpenSettings();
    }

    public void OnQuitBtnClicked()
    {
        Application.Quit();
    }
}
