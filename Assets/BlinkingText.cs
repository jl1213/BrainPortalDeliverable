using TMPro;
using UnityEngine;

public class SmoothBlinkingText : MonoBehaviour
{
    public TextMeshProUGUI textElement;
    public float blinkSpeed = 2f;

    private Color originalColor;

    void Start()
    {
        originalColor = textElement.color;
    }

    void Update()
    {
        float alpha = Mathf.PingPong(Time.time * blinkSpeed, 1f);
        textElement.color = new Color(1f, 1f, 1f, alpha); // force white RGB with fading alpha
    }
}
