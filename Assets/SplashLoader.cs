using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SplashLoader : MonoBehaviour
{
    public Image whiteFadeImage;             // Drag your white image here
    public string nextSceneName = "MainScene"; // Replace with actual scene name
    public float fadeDuration = 1f;          // How fast to fade out white
    public float totalDelay = 3f;            // Total time before switching scenes

    void Start()
    {
        StartCoroutine(FadeAndLoad());
    }

    IEnumerator FadeAndLoad()
    {
        // Wait briefly before starting fade (optional)
        yield return new WaitForSeconds(0.2f);

        // Fade from opaque white to transparent
        float elapsed = 0f;
        Color color = whiteFadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            whiteFadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        whiteFadeImage.color = new Color(color.r, color.g, color.b, 0f);

        // Wait remaining time before changing scene
        yield return new WaitForSeconds(totalDelay - fadeDuration);
        SceneManager.LoadScene(nextSceneName);
    }
}
