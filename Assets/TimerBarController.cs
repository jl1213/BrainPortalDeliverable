using UnityEngine;
using UnityEngine.UI;

public class TimerBarController : MonoBehaviour
{
    public Slider timerSlider;
    public float totalTime = 10f;
    private float timeRemaining;
    private bool timerRunning = false;
    public static float ScoreMultiplier = 1.0f;
    float multiplier = TimerBarController.ScoreMultiplier;


    void Start()
    {
        if (SceneSwitcher.timerModeEnabled)
        {
            timeRemaining = totalTime;
            timerRunning = true;
            timerSlider.value = 0f;
        }
        
        
    }

    void Update()
    {
        if (SceneSwitcher.timerModeEnabled && timerRunning)
        {
            timeRemaining -= Time.deltaTime;

            float progress = Mathf.Clamp01(1f - (timeRemaining / totalTime));
            timerSlider.value = progress;

            if (timeRemaining <= 0f)
            {
                timerRunning = false;
                Debug.Log("[TIMER BAR] Time's up!");
                // Add timeout behavior if needed
            }
        }
    }

    public float GetScoreMultiplier()
    {
        return Mathf.Clamp01(timeRemaining / totalTime);
    }
}
