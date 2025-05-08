using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform defaultView;
    public Transform thinkingView;
    public float transitionSpeed = 2f;

    private Transform target;
    private bool isTransitioning = false;

    void Start()
    {
        target = defaultView;
    }

    void Update()
    {
        if (isTransitioning)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * transitionSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, Time.deltaTime * transitionSpeed);

            if (Quaternion.Angle(transform.rotation, target.rotation) < 0.1f)
            {
                isTransitioning = false;
            }
        }
    }

    public void MoveToThinking()
    {
        target = thinkingView;
        isTransitioning = true;
    }

    public void MoveToDefault()
    {
        target = defaultView;
        isTransitioning = true;
    }
}
