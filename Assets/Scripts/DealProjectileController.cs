using System.Collections;
using UnityEngine;

public class DealProjectileController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rbody;
    private float velocity;
    public void Init(float fireDelay, float velocity, int facing, float yPos)
    {
        this.velocity = velocity;
        var look = new Vector3(transform.position.x + facing * 32, yPos) - transform.position;
        transform.rotation = Quaternion.FromToRotation(Vector3.right, look);
        StartCoroutine(FireSequence(fireDelay));
    }

    private IEnumerator FireSequence(float delay)
    {
        yield return new WaitForSeconds(delay);

        animator.SetTrigger("fire");
        rbody.linearVelocity = transform.rotation * Vector3.right * velocity;
    }

}
