using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ColorChanger : MonoBehaviour
{
    [Tooltip("The Image component behind the score counter.")]
    public Image backgroundImage;

    [Tooltip("Color to flash when user gets it right.")]
    public Color correctColor = Color.green;

    [Tooltip("Color to flash when user gets it wrong.")]
    public Color wrongColor = Color.red;

    [Tooltip("How long to flash before returning to original color.")]
    public float flashDuration = 0.3f;

    private Color originalColor;
    private int lastKnownScore;

    void Start()
    {
        if (backgroundImage == null)
        {
            Debug.LogError("❌ Background Image not assigned in ColorChanger!");
            return;
        }

        originalColor = backgroundImage.color;
        lastKnownScore = ScoreCounter.GetScore();
    }

    void Update()
    {
        int currentScore = ScoreCounter.GetScore();

        if (currentScore > lastKnownScore)
        {
            FlashColor(correctColor); // ✅ Flash green on score increase
        }
        else if (currentScore < lastKnownScore)
        {
            FlashColor(wrongColor); // ❌ Flash red on score decrease
        }

        lastKnownScore = currentScore;
    }

    private void FlashColor(Color color)
    {
        if (backgroundImage == null) return;
        StopAllCoroutines();
        StartCoroutine(FlashRoutine(color));
    }

    private IEnumerator FlashRoutine(Color flash)
    {
        backgroundImage.color = flash;
        yield return new WaitForSeconds(flashDuration);
        backgroundImage.color = originalColor;
    }
}
