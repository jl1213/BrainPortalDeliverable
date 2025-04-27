using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingSlide : MonoBehaviour
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
        // Reset the question index to 0 before loading the new scene
        SceneSwitcher.currentQuestionIndex = 0;
        ScoreCounter.ResetScore();

        // Load the next scene
        SceneManager.LoadScene(nextSceneName);
    }
}
