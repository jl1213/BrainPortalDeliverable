using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class SceneSwitcher : MonoBehaviour

{
    public static string correctAnimationName;
    public static string infoAnatomy;
    public static string infoFunction;
    public static string infoPathology;
    public static int correctSliceNumber;
    public static string correctStructureName;
    public static string correctInfo;
    public static string correctCoords; // ✅ Store coordinate data
    public static int currentQuestionIndex = 0;
    public static bool timerModeEnabled = false;

    public SpriteRenderer brainSpriteRenderer;
    public TextMeshProUGUI sliceText;
    public TextMeshProUGUI structureText;
    public TextMeshProUGUI infoText;
    public GameObject okButton; // ✅ Ensure the OK button is referenced

    void Start()
    {
        if (!string.IsNullOrEmpty(correctAnimationName))
        {
            Sprite[] allSprites = Resources.LoadAll<Sprite>(correctAnimationName);
            Sprite matchingSprite = allSprites.FirstOrDefault(sprite =>
                sprite.name.Contains(correctSliceNumber.ToString())
            );

            if (matchingSprite != null)
            {
                brainSpriteRenderer.sprite = matchingSprite;
            }
        }

        if (sliceText != null)
        {
            sliceText.text = $"Slice {correctSliceNumber}";
        }

        if (structureText != null)
        {
            structureText.text = $"Tap the {correctStructureName}!";
        }

        if (infoText != null)
        {
            infoText.text = correctInfo;
        }

        if (okButton != null)
        {
            okButton.SetActive(true); // ✅ Ensure the OK button is enabled
        }
    }

    public void LoadMainScene()
    {
        ScoreCounter.SetScoreFrozen(false); // ✅ Unfreeze score
        currentQuestionIndex++;
        SceneManager.LoadScene("v3AnatomyFinder");
    }

    public static SceneSwitcher Instance;

    void Awake()
    {
        Instance = this;
    }

    public static void SwitchToFeedbackScene(string animationName, int sliceNumber, string structureName, string anatomy, string function, string pathology, string coord)
    {
        correctAnimationName = animationName;
        correctSliceNumber = sliceNumber;
        correctStructureName = structureName;
        correctCoords = coord;

        infoAnatomy = anatomy;
        infoFunction = function;
        infoPathology = pathology;

        SceneManager.LoadScene("SingleSlicer");
    }
    public static void ResetGameState()
    {
        currentQuestionIndex = 0;
        ScoreCounter.ResetScore(); // You’ll need to add this to ScoreCounter if it doesn’t exist
                                   // Add any other global resets here
    }
}
