using UnityEngine;
using TMPro; // Import TextMeshPro namespace
using UnityEngine.UI; // Import UI namespace for button support
using UnityEngine.EventSystems; // Import for UI event handling

public class AnimationController : MonoBehaviour
{
    private Animator animator;
    private float currentNormalizedTime;
    public float playbackSpeed = 0.2f;

    [Tooltip("Assign the animation clip you want to play.")]
    public AnimationClip animationClip;

    private string animationName;

    [Tooltip("Assign the TextMeshPro UI element to display frame number.")]
    public TextMeshProUGUI frameText;

    [Tooltip("Set to 0 to automatically detect frames from the AnimationClip.")]
    public int totalFrames = 0;

    [Tooltip("Assign the Left button UI element.")]
    public Button leftButton;

    [Tooltip("Assign the Right button UI element.")]
    public Button rightButton;

    private bool isHoldingLeft = false;
    private bool isHoldingRight = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (animationClip == null)
        {
            Debug.LogError("No animation clip assigned. Please assign one in the Inspector.");
            return;
        }

        animationName = animationClip.name;
        animator.speed = 0; // Ensure the animation does not auto-play
        currentNormalizedTime = 0;
        animator.Play(animationName, 0, currentNormalizedTime);

        if (totalFrames <= 0)
        {
            totalFrames = Mathf.RoundToInt(animationClip.length * animationClip.frameRate);
            Debug.Log($"Detected {totalFrames} frames in animation: {animationName}");
        }

        UpdateFrameDisplay();

        // Attach press & hold event listeners
        AddButtonListener(leftButton, () => isHoldingLeft = true, () => isHoldingLeft = false);
        AddButtonListener(rightButton, () => isHoldingRight = true, () => isHoldingRight = false);
    }

    void Update()
    {
        if (animationClip == null) return;

        if (isHoldingRight)
        {
            MoveForward();
        }
        else if (isHoldingLeft)
        {
            MoveBackward();
        }
    }

    void MoveForward()
    {
        currentNormalizedTime += Time.deltaTime * playbackSpeed;
        if (currentNormalizedTime > 1f) currentNormalizedTime = 1f;
        animator.Play(animationName, 0, currentNormalizedTime);
        animator.Update(Time.deltaTime); // Force update the animator
        UpdateFrameDisplay();
    }

    void MoveBackward()
    {
        currentNormalizedTime -= Time.deltaTime * playbackSpeed;
        if (currentNormalizedTime < 0f) currentNormalizedTime = 0f;
        animator.Play(animationName, 0, currentNormalizedTime);
        animator.Update(Time.deltaTime); // Force update the animator
        UpdateFrameDisplay();
    }

    void UpdateFrameDisplay()
    {
        if (frameText != null && totalFrames > 0)
        {
            int frameNumber = Mathf.RoundToInt(currentNormalizedTime * (totalFrames - 1)) + 1;
            frameText.text = $"Slice {frameNumber}";
        }
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