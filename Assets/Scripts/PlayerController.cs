using System.Collections;
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
	
	[Space]
	[Header("Data Assets")]
	[Space]
	[SerializeField] private CardLibrary cardLibrary;
	[SerializeField] private string startingDeck;
    private InputHandler input;
    private bool acting, cancellable, grounded;
    private int facing, attackRepeat;
	private int selectedCardIndex, maxHand;
	private CardData readiedCard;
	private List<CardData> hand = new();
	private List<CardData> discard = new();
	private List<CardData> deck = new();
	private int handSize => hand.Count;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        input = InputHandler.Instance;

		foreach (var index in startingDeck.Split(','))
			deck.Add(cardLibrary.GetCard(int.Parse(index)));

		maxHand = 4;
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
			if (input.scrollDir >= 0)
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
		GameObject card = null;
		Vector2 rotation = default;

		switch (readiedCard.id)
		{
			case CardID.HIGH_CARD:
				card = Instantiate(cardPrefab, transform.position + new Vector3(facing * 1.5f, -1), Quaternion.identity);
				card.GetComponent<Rigidbody2D>().linearVelocityX = facing * 28;
				break;
			case CardID.PAIR:
				card = Instantiate(cardPrefab, transform.position + new Vector3(facing * 1.5f, -1.2f), Quaternion.identity);
				card.GetComponent<Rigidbody2D>().linearVelocityX = facing * 28;
				StartCoroutine(DelayCard());
				IEnumerator DelayCard()
				{
					yield return new WaitForSeconds(0.1f);
					card = Instantiate(cardPrefab, transform.position + new Vector3(facing * 1.5f, -0.8f), Quaternion.identity);
					card.GetComponent<Rigidbody2D>().linearVelocityX = facing * 28;
				}
				break;
			case CardID.FULL_HOUSE:
				rotation = new Vector2(facing, Random.Range(0.05f, 0.1f));
				card = Instantiate(cardPrefab, transform.position + new Vector3(facing * 1.5f, -1), Quaternion.FromToRotation(facing * Vector2.right, rotation.normalized));
				card.GetComponent<Rigidbody2D>().linearVelocity = (26 + Random.Range(0, 3)) * rotation.normalized;
				rotation = new Vector2(facing, Random.Range(-0.05f, -0.1f));
				card = Instantiate(cardPrefab, transform.position + new Vector3(facing * 1.5f, -1), Quaternion.FromToRotation(facing * Vector2.right, rotation.normalized));
				card.GetComponent<Rigidbody2D>().linearVelocity = (20 + Random.Range(0, 9)) * rotation.normalized;
				rotation = new Vector2(facing, Random.Range(0.01f, 0.04f));
				card = Instantiate(cardPrefab, transform.position + new Vector3(facing * 1.5f, -1), Quaternion.FromToRotation(facing * Vector2.right, rotation.normalized));
				card.GetComponent<Rigidbody2D>().linearVelocity = (22 + Random.Range(0, 7)) * rotation.normalized;
				rotation = new Vector2(facing, Random.Range(-0.01f, -0.04f));
				card = Instantiate(cardPrefab, transform.position + new Vector3(facing * 1.5f, -1), Quaternion.FromToRotation(facing * Vector2.right, rotation.normalized));
				card.GetComponent<Rigidbody2D>().linearVelocity = (24 + Random.Range(0, 5)) * rotation.normalized;
				rotation = new Vector2(facing, 0);
				card = Instantiate(cardPrefab, transform.position + new Vector3(facing * 1.5f, -1), Quaternion.FromToRotation(facing * Vector2.right, rotation.normalized));
				card.GetComponent<Rigidbody2D>().linearVelocity = 28 * rotation.normalized;
				break;
			case CardID.JOKER:
				card = Instantiate(cardPrefab, transform.position + new Vector3(facing * 1.5f, -1), Quaternion.identity);
				card.GetComponent<Rigidbody2D>().linearVelocityX = facing * 22;
				break;
			case CardID.HIDDEN_ACE:
				card = Instantiate(cardPrefab, transform.position + new Vector3(facing * 1.5f, -1), Quaternion.identity);
				card.GetComponent<Rigidbody2D>().linearVelocityX = facing * 36;
				break;
			default:
				break;
		}
	}

	public void CreateReload()
	{
		DrawHand();
	}

	public void DrawHand()
	{
		while (hand.Count > 0)
			DiscardCard(0);

		for (int i = 0; i < maxHand; i++)
		{
			DrawCard();
		}
	}

	public void DrawCard()
	{
		if (deck.Count == 0)
		{
			if (discard.Count == 0)
				return;
			deck.AddRange(discard);
			discard.Clear();
		
		}
		
		int index = Random.Range(0, deck.Count);

		CardData card = deck[index];
		deck.RemoveAt(index);
		hudController.DrawCard(card);
		hand.Add(card);

		if (selectedCardIndex == -1)
		{
			selectedCardIndex = 0;
			hudController.SetIndexSelected(selectedCardIndex);
		}
	}

	public void DiscardCard(int index)
	{
		discard.Add(cardLibrary.GetCard(hand[index].id));
		hand.RemoveAt(index);

		hudController.DiscardIndex(index);

		if (selectedCardIndex >= handSize)
			selectedCardIndex = handSize - 1;
		
		hudController.SetIndexSelected(selectedCardIndex);
	}

	public void UseSelectedCard()
	{
		var card = hand[selectedCardIndex];

		readiedCard = card;

		card.charges--;
		hudController.UseCharge();

		if (card.charges <= 0)
		{
			DiscardCard(selectedCardIndex);
		} else
		{
			hand[selectedCardIndex] = card;
		}
	}

}
