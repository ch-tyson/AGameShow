using UnityEngine;

public class Animate : MonoBehaviour
{
    private Animator mAnimator;

    void Start()
    {
        mAnimator = GetComponent<Animator>();
    }

    public void TriggerHapp()
    {
        if (mAnimator != null)
            mAnimator.SetTrigger("TrigHapp");
    }

    public void TriggerIdle()
    {
        if (mAnimator != null)
            mAnimator.SetTrigger("TrigIdle");
    }

    public void TriggerDiss()
    {
        if (mAnimator != null)
            mAnimator.SetTrigger("TrigDiss");
    }
}
