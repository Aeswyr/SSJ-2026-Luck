using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] private Transform followTarget;
    [SerializeField] private List<Transform> followList;
    [SerializeField] private bool lockY;

    [SerializeField] private bool useBounds;
    [SerializeField] Vector2 bounds;
    [SerializeField] Transform shakeTarget;
    private List<FollowObject> followers = new();
    void Start()
    {
        foreach (var link in followList)
            followers.Add(new()
            {
                transform = link,
                startPos = link.position,
                isMainCamera = link.TryGetComponent<Camera>(out var cam) && cam == Camera.main
            });
    }
    void Update()
    {
        if (followTarget == null)
            return;


        float targetX = GetFollowX();

        foreach (var follower in followers)
        {
            Vector3 pos = new Vector3(targetX, lockY ? follower.startPos.y : followTarget.position.y, follower.startPos.z);
            if (follower.isMainCamera)
                pos += shakeTarget.localPosition;
            follower.transform.position = pos;
        }
    }

    public void SetFollow(Transform follow)
    {
        this.followTarget = follow;
    }

	public void Screenshake(float intensity, float duration)
	{
		shakeTarget.DOShakePosition(duration, intensity).onComplete += () =>
		{
			shakeTarget.transform.localPosition = Vector3.zero;
		};
	}

    public float GetFollowX()
    {
        if (followTarget == null)
            return 0;
        float targetX = followTarget.position.x;

        if (useBounds)
        {
            if (targetX > bounds.y)
                targetX = bounds.y;
            else if (targetX < bounds.x)
                targetX = bounds.x;
        }

        return targetX;
    }

    public Transform GetFollow()
    {
        return followTarget;
    }
    public void SetBounds(Vector2 bounds)
    {
        this.bounds = bounds;
    }

    private struct FollowObject
    {
        public Transform transform;
        public Vector3 startPos;
        public bool isMainCamera;
    }
}
