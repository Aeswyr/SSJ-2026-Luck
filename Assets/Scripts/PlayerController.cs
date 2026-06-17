using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
	[SerializeField] private Animator animator;
    [SerializeField] private JumpHandler jump;
    [SerializeField] private MovementHandler move;
    [SerializeField] private GroundedCheck ground;

    private InputHandler input;
    private bool acting, cancellable, grounded;
    private int facing;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        input = InputHandler.Instance;
    }

    void FixedUpdate()
    {
        grounded = ground.CheckGrounded();

        HandleInputs();

		animator.SetBool("grounded", grounded);
		animator.SetBool("moving", input.move.down && !acting);

    }

    private void HandleInputs()
    {
        if (!acting && input.move.pressed)
		{
			move.StartAcceleration(input.dir);
			UpdateFacing(input.dir);
		}
		else if (!acting && input.move.down)
		{
			move.UpdateMovement(input.dir);
			UpdateFacing(input.dir);

			/*if (Time.time > nextFootstep && grounded)
			{
				nextFootstep = Time.time + 0.35f;
				SFXManager.Instance.PlaySound("step");
			}*/
		}
		else if (!acting && input.move.released)
		{
			move.StartDeceleration();
		}

        if (!acting && grounded && input.jump.pressed)
		{
			jump.StartJump();
			//animator.SetTrigger("jump");
			//SFXManager.Instance.PlaySound("jump");

            if (grounded)
			{	
				//VFXManager.Instance.SyncVFX(ParticleType.JUMP, transform.position, facing == -1);
			} else
			{
				//VFXManager.Instance.SyncVFX(ParticleType.AIR_JUMP, transform.position + 2 * Vector3.down, facing == -1);
			}
		}

		if ((!acting || cancellable) && input.attack.pressed)
		{
			acting = true;
			
			animator.SetInteger("attackId", cancellable ? 1 : 0);
			animator.SetTrigger("attack");

			move.StartDeceleration();

			cancellable = false;
		}
    }

    private void UpdateFacing(float dir)
    {
        int num = facing;
		if (dir > 0f)
		{
			facing = 1;
		}
		else if (dir < 0f)
		{
			facing = -1;
		}

        if (num != facing)
		{
			bool flipX = facing < 0;
			sprite.flipX = flipX;
		}
    }

	public void EndAction()
	{
		acting = false;
		cancellable = false;
	}

	public void SetCancellable()
	{
		cancellable = true;
	}
}
