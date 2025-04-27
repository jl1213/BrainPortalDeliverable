using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleLoader : MonoBehaviour
{
    public string nextSceneName = "v3AnatomyFinder";

    void Update()
    {
        // For keyboard input
        if (Input.anyKeyDown)
        {
            LoadNextScene();
        }

        // For mouse or screen tap
        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
        {
            LoadNextScene();
        }
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
