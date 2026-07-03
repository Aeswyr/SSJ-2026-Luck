using DG.Tweening;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform followTarget;
    [SerializeField] private bool lockY;
    [SerializeField] private bool useBounds;
    [SerializeField] Vector2 bounds;
    [SerializeField] Transform shakeTarget;
    Vector3 startPos;
    void Start()
    {
        startPos = transform.position;
    }
    void Update()
    {
        if (followTarget == null)
            return;
        float targetX = followTarget.position.x;
        if (useBounds)
        {
            if (targetX > bounds.y)
                targetX = bounds.y;
            else if (targetX < bounds.x)
                targetX = bounds.x;
        }
        transform.position = new Vector3(targetX, lockY ? startPos.y : followTarget.position.y, startPos.z) + shakeTarget.localPosition;
    }

    public void SetFollow(Transform follow)
    {
        this.followTarget = follow;
    }

	public void Screenshake(float intensity, float duration)
	{
		shakeTarget.DOShakePosition(duration, new Vector3(intensity, 0.025f)).onComplete += () =>
		{
			shakeTarget.transform.localPosition = Vector3.zero;
		};
	}

    public Transform GetFollow()
    {
        return followTarget;
    }
    public void SetBounds(Vector2 bounds)
    {
        this.bounds = bounds;
    }
}
