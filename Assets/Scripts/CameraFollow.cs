using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform followTarget;
    [SerializeField] private bool lockY;
    [SerializeField] private bool useBounds;
    [SerializeField] Vector2 bounds;
    Vector3 startPos;
    void Start()
    {
        startPos = transform.position;
    }
    void Update()
    {
        float targetX = followTarget.position.x;
        if (useBounds)
        {
            if (targetX > bounds.y)
                targetX = bounds.y;
            else if (targetX < bounds.x)
                targetX = bounds.x;
        }
        transform.position = new Vector3(targetX, lockY ? startPos.y : followTarget.position.y, startPos.z);
    }

    public void SetFollow(Transform follow)
    {
        this.followTarget = follow;
    }
}
