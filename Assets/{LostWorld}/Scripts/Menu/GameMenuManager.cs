using UnityEngine;
using UnityEngine.UI;

public class GameMenuManager : MonoBehaviour
{
    public void OnContinueBtnClicked()
    {
        
    }

    public void OnSettingsBtnClicked()
    {
        SettingsManager.OpenSettings();
    }

    public void OnMenuBtnClicked()
    {
        SceneLoader.LoadScene("MainMenu");
    }
}
