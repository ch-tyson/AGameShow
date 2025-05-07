using UnityEngine;

public class Animate : MonoBehaviour
{
    private Animator mAnimator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(mAnimator != null)
        {
            if(Input.GetKeyDown(KeyCode.H))
            {
                mAnimator.SetTrigger("TrigHapp");
            }
            if(Input.GetKeyDown(KeyCode.I))
            {
                mAnimator.SetTrigger("TrigIdle");
            }
            if(Input.GetKeyDown(KeyCode.D))
            {
                mAnimator.SetTrigger("TrigDiss");
            }
        }
    }
}
