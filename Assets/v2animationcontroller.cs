using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class V2AnimationController : MonoBehaviour
{
    private Animator animator;
    private float currentNormalizedTime;
    public float playbackSpeed;

    [Tooltip("Assign the Animator that controls the brain animations.")]
    public Animator brainAnimator;

    [Tooltip("Assign the TextMeshPro UI element to display frame number.")]
    public TextMeshProUGUI frameText;

    [Tooltip("Assign the Left button UI element.")]
    public Button leftButton;

    [Tooltip("Assign the Right button UI element.")]
    public Button rightButton;

    [Tooltip("Assign the Left x2 speed button UI element.")]
    public Button leftFastButton;

    [Tooltip("Assign the Right x2 speed button UI element.")]
    public Button rightFastButton;

    private bool isHoldingLeft = false;
    private bool isHoldingRight = false;
    private bool isHoldingLeftFast = false;
    private bool isHoldingRightFast = false;

    private string currentAnimationName;

    private int totalFrames = 0; // Store actual frame count from the animation clip
    private const float frameDuration = 0.08f; // Each frame should be on-screen for 0.08 seconds

    void Start()
    {
        if (brainAnimator == null)
        {
            Debug.LogError("No Animator assigned. Please assign one in the Inspector.");
            return;
        }

        currentNormalizedTime = 0;
        brainAnimator.speed = 0; // Ensure animation does not play automatically
        UpdateFrameDisplay();

        AddButtonListener(leftButton, () => isHoldingLeft = true, () => isHoldingLeft = false);
        AddButtonListener(rightButton, () => isHoldingRight = true, () => isHoldingRight = false);
        AddButtonListener(leftFastButton, () => isHoldingLeftFast = true, () => isHoldingLeftFast = false);
        AddButtonListener(rightFastButton, () => isHoldingRightFast = true, () => isHoldingRightFast = false);
    }

    void Update()
    {
        if (brainAnimator == null || string.IsNullOrEmpty(currentAnimationName)) return;

        float speedMultiplier = 1f;

        if (isHoldingRight || isHoldingLeft || isHoldingRightFast || isHoldingLeftFast)
        {
            if (isHoldingRightFast || isHoldingLeftFast)
            {
                speedMultiplier = 2f; // Double speed for fast buttons
            }

            if (isHoldingRight || isHoldingRightFast)
            {
                MoveForward(speedMultiplier);
            }
            else if (isHoldingLeft || isHoldingLeftFast)
            {
                MoveBackward(speedMultiplier);
            }
        }
        else
        {
            brainAnimator.speed = 0; // Stop animation when no buttons are pressed
        }
    }

    void MoveForward(float speedMultiplier)
    {
        brainAnimator.speed = 1;
        currentNormalizedTime += Time.deltaTime * playbackSpeed * speedMultiplier;
        if (currentNormalizedTime > 1f) currentNormalizedTime = 1f;
        brainAnimator.Play(currentAnimationName, 0, currentNormalizedTime);
        brainAnimator.Update(Time.deltaTime);
        UpdateFrameDisplay();
    }

    void MoveBackward(float speedMultiplier)
    {
        brainAnimator.speed = 1;
        currentNormalizedTime -= Time.deltaTime * playbackSpeed * speedMultiplier;
        if (currentNormalizedTime < 0f) currentNormalizedTime = 0f;
        brainAnimator.Play(currentAnimationName, 0, currentNormalizedTime);
        brainAnimator.Update(Time.deltaTime);
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

    public void PlayAnimation(string animationName)
    {
        if (brainAnimator == null || string.IsNullOrEmpty(animationName)) return;

        currentAnimationName = animationName;
        brainAnimator.Play(animationName, 0, 0);
        currentNormalizedTime = 0;
        brainAnimator.speed = 0; // Ensure animation starts paused

        // ✅ Restore Missing Components if Needed
        if (brainAnimator.gameObject.GetComponent<BoxCollider2D>() == null)
        {
            brainAnimator.gameObject.AddComponent<BoxCollider2D>();
        }
        if (brainAnimator.gameObject.GetComponent<ShowCoord>() == null)
        {
            brainAnimator.gameObject.AddComponent<ShowCoord>();
        }

        // Get total frame count from the animation clip
        totalFrames = GetAnimationFrameCount(animationName);

        // Dynamically adjust playback speed based on total frames
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

    int GetAnimationFrameCount(string animationName)
    {
        if (brainAnimator == null) return 0;

        RuntimeAnimatorController controller = brainAnimator.runtimeAnimatorController;
        foreach (var clip in controller.animationClips)
        {
            if (clip.name == animationName)
            {
                return Mathf.RoundToInt(clip.length * clip.frameRate);
            }
        }
        return 100; // Default fallback if animation is not found
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
