using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    public static ScoreCounter Instance;

    public static int CurrentScore = 0;
    private static bool isScoreFrozen = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist through scene loads
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public static void AddScore(int amount)
    {
        if (isScoreFrozen) return;

        CurrentScore += amount;
        if (CurrentScore < 0)
        {
            CurrentScore = 0;
        }
    }

    public static void SetScoreFrozen(bool freeze)
    {
        isScoreFrozen = freeze;
    }

    public static void ResetScore()
    {
        CurrentScore = 0;
        isScoreFrozen = false;
    }

    public static int GetScore()
    {
        return CurrentScore;
    }

    public static bool IsFrozen()
    {
        return isScoreFrozen;
    }
}
