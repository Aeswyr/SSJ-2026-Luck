using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
	[SerializeField] private Animator animator;
    [SerializeField] private JumpHandler jump;
    [SerializeField] private MovementHandler move;
    [SerializeField] private GroundedCheck ground;
	[SerializeField] private UnitVFXController vfxController;
	[SerializeField] private PlayerHUDController hudController;

	[Space]
	[Header("Spawnables")]
	[Space]
	[SerializeField] private GameObject cardPrefab;

	[Space]
	[Header("Action Data")]
	[Space]
	[SerializeField] private AnimationCurve attackCurve;
	[SerializeField] private float attackSpeed;
	[SerializeField] private AnimationCurve attack2Curve;
	[SerializeField] private float attack2Speed;
	[SerializeField] private AnimationCurve dodgeCurve;
	[SerializeField] private float dodgeSpeed;
    private InputHandler input;
    private bool acting, cancellable, grounded;
    private int facing, attackRepeat;
	private int selectedCardIndex, maxHand;
	private List<CardData> hand = new();
	private int handSize => hand.Count;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        input = InputHandler.Instance;

		maxHand = 5;
		DrawHand();
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
			UpdateFacing();
		}
		else if (!acting && input.move.down)
		{
			move.UpdateMovement(input.dir);
			UpdateFacing();

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

		if ((!acting || cancellable) && handSize > 0 && input.attack.pressed)
		{
			UpdateFacing();

			UseSelectedCard();

			animator.SetInteger("attackId", cancellable ? 1 + attackRepeat : 0);
			animator.SetTrigger("attack");

			if (cancellable)
			{
				move.OverrideCurve(attack2Speed, attack2Curve, facing);
			} else {
				move.OverrideCurve(attackSpeed, attackCurve, facing);
				attackRepeat = 1;
			}


			attackRepeat = (attackRepeat + 1) % 2;
			StartAction();
		}

		if ((!acting || cancellable) && input.dodge.pressed) {
			UpdateFacing();

			vfxController.StartAfterImageChain(0.6f, 0.1f);

			animator.SetTrigger("dodge");

			move.OverrideCurve(dodgeSpeed, dodgeCurve, facing);

			StartAction();
		}

		if ((!acting || cancellable) && (input.reload.pressed || (handSize == 0 && input.attack.pressed))) {
			UpdateFacing();

			animator.SetTrigger("reload");

			move.StartDeceleration();
			StartAction();
		}

		if (input.scroll.pressed)
		{
			if (input.scrollDir < 0)
				selectedCardIndex = (handSize + selectedCardIndex - 1) % handSize;
			else
				selectedCardIndex = (selectedCardIndex + 1) % handSize;
			hudController.SetIndexSelected(selectedCardIndex);
		}
    }

    private void UpdateFacing()
    {
		float dir = input.dir;
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

	public void StartAction()
	{
		acting = true;
		cancellable = false;
	}

	public void EndAction()
	{
		if (acting)
		{
			move.ResetCurves();
			move.StartDeceleration();
		}

		acting = false;
		cancellable = false;
	}

	public void SetCancellable()
	{
		cancellable = true;
	}

	public void CreateAttack()
	{
		var card = Instantiate(cardPrefab, transform.position + new Vector3(facing * 1.5f, -1), Quaternion.identity);
		card.GetComponent<Rigidbody2D>().linearVelocityX = facing * 28;
	}

	public void CreateReload()
	{
		DrawHand();
	}

	public void DrawHand()
	{
		hudController.DiscardAll();
		hand.Clear();

		for (int i = 0; i < maxHand; i++)
		{
			CardData card = new()
			{
				charges = Random.Range(1, 8)
			};
			hudController.DrawCard(card);
			hand.Add(card);
		}
		selectedCardIndex = 0;
		hudController.SetIndexSelected(selectedCardIndex);
	}

	public void UseSelectedCard()
	{
		var card = hand[selectedCardIndex];
		card.charges--;
		hudController.UseCharge();

		if (card.charges <= 0)
		{
			hand.RemoveAt(selectedCardIndex);
			hudController.DiscardIndex(selectedCardIndex);

			if (selectedCardIndex >= handSize)
				selectedCardIndex = handSize - 1;
			
			hudController.SetIndexSelected(selectedCardIndex);
		} else
		{
			hand[selectedCardIndex] = card;
		}
	}

}
