using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InfoPanelController : MonoBehaviour
{
    public TextMeshProUGUI infoText;
    public Button anatomyButton;
    public Button functionButton;
    public Button pathologyButton;

    private Color selectedColor = new Color(0.6f, 0.6f, 0.6f); // dark gray
    private Color normalColor = Color.white;

    void Start()
    {
        // Pick default tab based on view type
        string viewType = SceneSwitcher.correctAnimationName.ToLower();

        if (viewType.Contains("coronal"))
        {
            ShowAnatomyInfo();
        }
        else if (viewType.Contains("axial"))
        {
            ShowFunctionInfo();
        }
        else if (viewType.Contains("sagittal"))
        {
            ShowPathologyInfo();
        }
        else
        {
            ShowAnatomyInfo(); // fallback
        }

        // Setup button listeners
        anatomyButton.onClick.AddListener(ShowAnatomyInfo);
        functionButton.onClick.AddListener(ShowFunctionInfo);
        pathologyButton.onClick.AddListener(ShowPathologyInfo);
    }

    void ShowAnatomyInfo()
    {
        infoText.text = SceneSwitcher.infoAnatomy;
        HighlightButton(anatomyButton);
    }

    void ShowFunctionInfo()
    {
        infoText.text = SceneSwitcher.infoFunction;
        HighlightButton(functionButton);
    }

    void ShowPathologyInfo()
    {
        infoText.text = SceneSwitcher.infoPathology;
        HighlightButton(pathologyButton);
    }

    void HighlightButton(Button active)
    {
        // Reset all button colors
        anatomyButton.GetComponent<Image>().color = normalColor;
        functionButton.GetComponent<Image>().color = normalColor;
        pathologyButton.GetComponent<Image>().color = normalColor;

        // Set selected one to darker color
        active.GetComponent<Image>().color = selectedColor;
    }
}
