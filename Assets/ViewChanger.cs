using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ViewChanger : MonoBehaviour
{
    public Animator brainAnimator;
    public Button sagittalButton, coronalButton, axialButton;
    public Button leftButton, rightButton;
    public TextMeshProUGUI frameText; // ✅ Frame Counter UI

    private string currentView = "coronalview"; // Default view
    private float currentNormalizedTime = 0f;
    private float playbackSpeed = 0.2f; // Default fallback
    private bool isHoldingLeft = false;
    private bool isHoldingRight = false;
    private int totalFrames = 100; // Default fallback frame count
    private const float frameDuration = 0.08f; // ✅ Standardized frame duration

    void Start()
    {
        // ✅ Prevent unwanted button text changes
        sagittalButton.GetComponentInChildren<TextMeshProUGUI>().text = "Sagittal";
        coronalButton.GetComponentInChildren<TextMeshProUGUI>().text = "Coronal";
        axialButton.GetComponentInChildren<TextMeshProUGUI>().text = "Axial";

        // ✅ Assign click events for view buttons
        sagittalButton.onClick.AddListener(() => ChangeView("sagittalview"));
        coronalButton.onClick.AddListener(() => ChangeView("coronalview"));
        axialButton.onClick.AddListener(() => ChangeView("axialview"));

        // ✅ Assign hold events for arrows
        AddButtonListener(leftButton, () => isHoldingLeft = true, () => isHoldingLeft = false);
        AddButtonListener(rightButton, () => isHoldingRight = true, () => isHoldingRight = false);

        PlayAnimation(currentView);
    }

    void Update()
    {
        if (brainAnimator == null || string.IsNullOrEmpty(currentView)) return;

        if (isHoldingRight)
        {
            MoveForward();
        }
        else if (isHoldingLeft)
        {
            MoveBackward();
        }
        else
        {
            brainAnimator.speed = 0; // Stop when no buttons are pressed
        }

        UpdateFrameDisplay(); // ✅ Update frame counter dynamically
    }

    void ChangeView(string viewName)
    {
        if (brainAnimator == null) return;

        currentView = viewName;
        PlayAnimation(viewName);
    }

    void PlayAnimation(string animationName)
    {
        if (brainAnimator != null && !string.IsNullOrEmpty(animationName))
        {
            brainAnimator.Play(animationName, 0, 0);
            currentNormalizedTime = 0;
            brainAnimator.speed = 0; // Start animation paused

            // ✅ Get total frames from animation
            totalFrames = GetAnimationFrameCount(animationName);

            // ✅ Standardized playback speed
            if (totalFrames > 0)
            {
                playbackSpeed = 1f / (totalFrames * frameDuration);
            }
            else
            {
                playbackSpeed = 0.2f; // Default fallback
            }

            UpdateFrameDisplay();
        }
    }

    void MoveForward()
    {
        currentNormalizedTime += Time.deltaTime * playbackSpeed;
        if (currentNormalizedTime > 1f) currentNormalizedTime = 1f;
        brainAnimator.Play(currentView, 0, currentNormalizedTime);
        brainAnimator.speed = 1;
    }

    void MoveBackward()
    {
        currentNormalizedTime -= Time.deltaTime * playbackSpeed;
        if (currentNormalizedTime < 0f) currentNormalizedTime = 0f;
        brainAnimator.Play(currentView, 0, currentNormalizedTime);
        brainAnimator.speed = 1;
    }

    void UpdateFrameDisplay()
    {
        if (frameText != null && totalFrames > 0)
        {
            int frameNumber = Mathf.RoundToInt(currentNormalizedTime * (totalFrames - 1)) + 1;
            frameText.text = $"Slice {frameNumber}";
        }
    }

    int GetAnimationFrameCount(string animationName)
    {
        if (brainAnimator == null) return 100; // Default to 100 frames

        RuntimeAnimatorController controller = brainAnimator.runtimeAnimatorController;
        foreach (var clip in controller.animationClips)
        {
            if (clip.name == animationName)
            {
                return Mathf.RoundToInt(clip.length * clip.frameRate);
            }
        }
        return 100; // Default if animation is not found
    }

    void AddButtonListener(Button button, System.Action onPress, System.Action onRelease)
    {
        if (button == null) return;

        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry pointerDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        pointerDown.callback.AddListener((data) => { onPress(); });

        EventTrigger.Entry pointerUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        pointerUp.callback.AddListener((data) => { onRelease(); });

        trigger.triggers.Add(pointerDown);
        trigger.triggers.Add(pointerUp);
    }
}
