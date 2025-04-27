using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsController : MonoBehaviour
{
    public GameObject settingsPanel;

    public void ToggleSettingsPanel()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    public void RestartScene()
    {
        SceneSwitcher.ResetGameState();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void ToggleTimerMode(bool isOn)
    {
        SceneSwitcher.timerModeEnabled = isOn;
    }

    public void ExitToMainMenu()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    // Add more as needed: ToggleAudio(), ChangeTheme(), etc.
}
