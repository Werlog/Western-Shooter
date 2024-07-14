using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemotePlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float walkingBlendTime = 0.3f;

    private LowerAnimation previousAnimation;

    private void Awake()
    {
        previousAnimation = LowerAnimation.Idle;
    }

    public void SetLowerBodyAnimation(LowerAnimation animation)
    {
        if (previousAnimation == animation) return;

        if (animation == LowerAnimation.Falling)
        {
            animator.Play("Falling");
        }else
        {
            StopAllCoroutines();
            StartCoroutine(BlendWalkingAnimation(animation == LowerAnimation.Walking));
        }

        previousAnimation = animation;
    }

    private IEnumerator BlendWalkingAnimation(bool toWalking)
    {
        float timeElapsed = 0f;
        float startValue = animator.GetFloat("Blend");

        animator.Play("WalkIdleTree");

        while (timeElapsed < walkingBlendTime)
        {
            float percentage = timeElapsed / walkingBlendTime;
            animator.SetFloat("Blend", Mathf.Lerp(startValue, toWalking ? 1f : 0f, percentage));

            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
}
public enum LowerAnimation : byte
{
    Idle = 0,
    Walking = 1,
    Falling = 2,
}