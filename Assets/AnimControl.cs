using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class AnimControl : MonoBehaviour
{
    private Animator brainAnimator;

    void Start()
    {
        brainAnimator = GetComponent<Animator>(); // Cache the Animator component
    }

    void Update()
    {
        if (brainAnimator != null)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                brainAnimator.SetTrigger("Right"); // Trigger animation for right arrow
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                brainAnimator.SetTrigger("Left"); // Trigger animation for left arrow
            }
        }
    }
}