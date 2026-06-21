using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
	[SerializeField] private Animator animator;
    [SerializeField] private JumpHandler jump;
    [SerializeField] private MovementHandler move;
    [SerializeField] private GroundedCheck ground;
	[SerializeField] private UnitVFXController vfxController;
	[SerializeField] private InteractboxController interactBox;
	private PlayerHUDController hudController;

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
	private int selectedCardIndex;
	private int maxHand = 4;
	private CardData readiedCard;
	private List<CardData> hand = new();
	private List<CardData> discard = new();
	private List<CardData> deck = new();
	private int handSize => hand.Count;

	private UnityEvent onDrawHand = new();

	private bool inputsLocked;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		foreach (var cam in FindObjectsByType<CameraFollow>(FindObjectsSortMode.None))
			cam.SetFollow(transform);

		hudController = FindAnyObjectByType<PlayerHUDController>();
        input = InputHandler.Instance;

		ResetToBaseline();
    }

    void FixedUpdate()
    {
        grounded = ground.CheckGrounded();

		if (!inputsLocked)
        	HandleInputs();

		animator.SetBool("grounded", grounded);
		animator.SetBool("moving", input.move.down && !acting && !inputsLocked);
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

		if (handSize > 0 && input.scroll.pressed)
		{
			if (input.scrollDir >= 0)
				selectedCardIndex = (handSize + selectedCardIndex - 1) % handSize;
			else
				selectedCardIndex = (selectedCardIndex + 1) % handSize;
			hudController.SetIndexSelected(selectedCardIndex);
		}

		if (input.interact.pressed)
		{
			interactBox.FireInteraction();
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

		HitData hitData = new()
		{
			baseDamage = readiedCard.baseDamage,
			shouldStick = readiedCard.shouldStick
		};

		switch (readiedCard.id)
		{
			case CardID.HIGH_CARD:
				card = Instantiate(cardPrefab, transform.position + new Vector3(facing * 1.5f, -1), Quaternion.identity);
				card.GetComponent<ProjectileController>()
					.SetVelocity(facing * 28)
					.Init(transform, hitData);
				break;
			case CardID.PAIR:
				card = Instantiate(cardPrefab, transform.position + new Vector3(facing * 1.5f, -1.2f), Quaternion.identity);
				card.GetComponent<ProjectileController>()
					.SetVelocity(facing * 28)
					.Init(transform, hitData);
				StartCoroutine(DelayCard());
				IEnumerator DelayCard()
				{
					yield return new WaitForSeconds(0.1f);
					card = Instantiate(cardPrefab, transform.position + new Vector3(facing * 1.5f, -0.8f), Quaternion.identity);
					card.GetComponent<ProjectileController>()
						.SetVelocity(facing * 28)
						.Init(transform, hitData);
				}
				break;
			case CardID.FULL_HOUSE:
				float shotgunLifetime = 0.2f;
				float shotgunVariance = 0.2f;
				rotation = new Vector2(facing, Random.Range(0.05f, 0.1f));
				card = Instantiate(cardPrefab, transform.position + new Vector3(facing * 1.5f, -1), Quaternion.FromToRotation(facing * Vector2.right, rotation.normalized));
				card.GetComponent<ProjectileController>()
					.SetVelocity((26 + Random.Range(0, 3)) * rotation.normalized)
					.SetLifetime(shotgunLifetime + Random.Range(0, shotgunVariance))
					.Init(transform, hitData);
				rotation = new Vector2(facing, Random.Range(-0.05f, -0.1f));
				card = Instantiate(cardPrefab, transform.position + new Vector3(facing * 1.5f, -1), Quaternion.FromToRotation(facing * Vector2.right, rotation.normalized));
				card.GetComponent<ProjectileController>()
					.SetVelocity((20 + Random.Range(0, 9)) * rotation.normalized)
					.SetLifetime(shotgunLifetime + Random.Range(0, shotgunVariance))
					.Init(transform, hitData);
				rotation = new Vector2(facing, Random.Range(0.01f, 0.04f));
				card = Instantiate(cardPrefab, transform.position + new Vector3(facing * 1.5f, -1), Quaternion.FromToRotation(facing * Vector2.right, rotation.normalized));
				card.GetComponent<ProjectileController>()
					.SetVelocity((22 + Random.Range(0, 7)) * rotation.normalized)
					.SetLifetime(shotgunLifetime + Random.Range(0, shotgunVariance))
					.Init(transform, hitData);
				rotation = new Vector2(facing, Random.Range(-0.01f, -0.04f));
				card = Instantiate(cardPrefab, transform.position + new Vector3(facing * 1.5f, -1), Quaternion.FromToRotation(facing * Vector2.right, rotation.normalized));
				card.GetComponent<ProjectileController>()
					.SetVelocity((24 + Random.Range(0, 5)) * rotation.normalized)
					.SetLifetime(shotgunLifetime + Random.Range(0, shotgunVariance))
					.Init(transform, hitData);
				rotation = new Vector2(facing, 0);
				card = Instantiate(cardPrefab, transform.position + new Vector3(facing * 1.5f, -1), Quaternion.FromToRotation(facing * Vector2.right, rotation.normalized));
				card.GetComponent<ProjectileController>()
					.SetVelocity(28 * rotation.normalized)
					.SetLifetime(shotgunLifetime + Random.Range(0, 0.25f))
					.Init(transform, hitData);
				break;
			case CardID.JOKER:
				card = Instantiate(cardPrefab, transform.position + new Vector3(facing * 1.5f, -1), Quaternion.identity);
				card.GetComponent<ProjectileController>()
					.SetVelocity(facing * 22)
					.Init(transform, hitData);
				break;
			case CardID.HIDDEN_ACE:
				card = Instantiate(cardPrefab, transform.position + new Vector3(facing * 1.5f, -1), Quaternion.identity);
				card.GetComponent<ProjectileController>()
					.SetVelocity(facing * 36)
					.SetEntityPierce()
					.Init(transform, hitData);
				break;
			case CardID.FINISHING_STROKE:
				card = Instantiate(cardPrefab, transform.position + new Vector3(facing * 1.5f, -1), Quaternion.identity);
				card.GetComponent<ProjectileController>()
					.SetVelocity(facing * 42)
					.Init(transform, hitData);
				break;
			case CardID.REND:
				foreach (var stick in FindObjectsByType<CardStickable>(FindObjectsSortMode.None))
				{
					var hurtbox = stick.GetComponentInChildren<HurtboxController>();
					if (hurtbox != null) {
						HitData hit = hitData;
						hit.baseDamage = hit.baseDamage * stick.cards;
						hurtbox.OnHit(hit);
					}
					stick.ClearCards();
				}
				break;
			case CardID.RECALL:
				{int totalStuck = 0;
				foreach (var stick in FindObjectsByType<CardStickable>(FindObjectsSortMode.None))
				{
					totalStuck = stick.cards;
					stick.ClearCards();
				}

				for (int i = 0; i < 1 + totalStuck / 3; i++)
					DrawCard();}
				break;
			case CardID.HOMECOMING:
				{int totalStuck = 0;
				foreach (var stick in FindObjectsByType<CardStickable>(FindObjectsSortMode.None))
				{
					totalStuck = stick.cards;
					stick.ClearCards();
				}

				for (int i = 0; i < totalStuck / 2; i++)
					AddCardToHand(CardID.JOKER);}
				break;
			case CardID.ALL_IN:
				for (int handSize = hand.Count; handSize > 0; handSize--){
					rotation = new Vector2(facing, Random.Range(-0.13f, 0.13f));
					card = Instantiate(cardPrefab, transform.position + new Vector3(facing * 1.5f, -1), Quaternion.FromToRotation(facing * Vector2.right, rotation.normalized));
					card.GetComponent<ProjectileController>()
						.SetVelocity(25 * rotation.normalized)
						.Init(transform, hitData);
					rotation = new Vector2(-facing, Random.Range(-0.13f, 0.13f));
					card = Instantiate(cardPrefab, transform.position + new Vector3(-facing * 1.5f, -1), Quaternion.FromToRotation(-facing * Vector2.right, rotation.normalized));
					card.GetComponent<ProjectileController>()
						.SetVelocity(25 * rotation.normalized)
						.Init(transform, hitData);
					rotation = new Vector2(Random.Range(-3f, 3f), 1);
					card = Instantiate(cardPrefab, transform.position + (Vector3)(rotation.normalized * new Vector3(1.5f, 0)) + Vector3.down, Quaternion.FromToRotation(rotation.x * Vector2.right, rotation.normalized));
					card.GetComponent<ProjectileController>()
						.SetVelocity(25 * rotation.normalized)
						.Init(transform, hitData);
					rotation = new Vector2(Random.Range(-3f, 3f), 1);
					card = Instantiate(cardPrefab, transform.position + (Vector3)(rotation.normalized * new Vector3(1.5f, 0)) + Vector3.down, Quaternion.FromToRotation(rotation.x * Vector2.right, rotation.normalized));
					card.GetComponent<ProjectileController>()
						.SetVelocity(25 * rotation.normalized)
						.Init(transform, hitData);
				}

				while (hand.Count > 0)
					DiscardCard(0);
				break;
			case CardID.CALLED_SHOT:
				{
					var ray = Physics2D.Raycast(transform.position + new Vector3(facing * 1.5f, -1), facing * Vector2.right, 128, LayerMask.GetMask(new string[] {"Entity"}));
					if (ray && ray.collider.transform.parent != null)
					{
						DebuffController debuff = ray.collider.transform.parent.GetComponentInChildren<DebuffController>();
						if (debuff != null)
						{
							debuff.AddDebuff(DebuffType.MARK);
						}
					}
				}
				break;
			case CardID.CALLBACK:
				{
					var ray = Physics2D.Raycast(transform.position + new Vector3(facing * 1.5f, -1), facing * Vector2.right, 128, LayerMask.GetMask(new string[] {"Entity"}));
					if (ray && ray.collider.transform.parent != null)
					{
						DebuffController debuff = ray.collider.transform.parent.GetComponentInChildren<DebuffController>();
						if (debuff != null)
						{
							debuff.AddDebuff(DebuffType.COMEDY);
						}
					}
				}
				break;
			case CardID.PEERLESS_FOCUS:
				{
					var ray = Physics2D.Raycast(transform.position + new Vector3(facing * 1.5f, -1), facing * Vector2.right, 128, LayerMask.GetMask(new string[] {"Entity"}));
					if (ray && ray.collider.transform.parent != null)
					{
						DebuffController debuff = ray.collider.transform.parent.GetComponentInChildren<DebuffController>();
						if (debuff != null)
						{
							debuff.AddDebuff(DebuffType.LOOT);
						}
					}
				}
				break;
			case CardID.DOUBLE_DOWN:
				{
					var ray = Physics2D.Raycast(transform.position + new Vector3(facing * 1.5f, -1), facing * Vector2.right, 128, LayerMask.GetMask(new string[] {"Entity"}));
					if (ray && ray.collider.transform.parent != null)
					{
						DebuffController debuff = ray.collider.transform.parent.GetComponentInChildren<DebuffController>();
						if (debuff != null)
						{
							debuff.AddDebuff(DebuffType.BLEED);
						}
					}
				}
				break;
			case CardID.THOUSAND_CUTS:
				{
					var ray = Physics2D.Raycast(transform.position + new Vector3(facing * 1.5f, -1), facing * Vector2.right, 128, LayerMask.GetMask(new string[] {"Entity"}));
					if (ray && ray.collider.transform.parent != null)
					{
						DebuffController debuff = ray.collider.transform.parent.GetComponentInChildren<DebuffController>();
						if (debuff != null)
						{
							debuff.AddDebuff(DebuffType.PAIN);
						}
					}
				}
				break;
    		case CardID.DECK_FIXING:
				onDrawHand.AddListener(CardAction_FixingTheDeck);
				void CardAction_FixingTheDeck()
				{
					DrawCard();
					DrawCard();
				}
				break;
			case CardID.CLOWN_CAR:
				onDrawHand.AddListener(CardAction_ClownCar);
				void CardAction_ClownCar()
				{

					AddCardToHand(CardID.JOKER);
					AddCardToHand(CardID.JOKER);
					AddCardToHand(CardID.JOKER);
				}
				break;
			case CardID.MISSED_CONNECTION:
				break;
    		case CardID.INSIGHT:
				DrawCard();
				DrawCard();
				break;
			case CardID.FOOLIN_AROUND:
				AddCardToHand(CardID.JOKER);
				AddCardToHand(CardID.JOKER);
				AddCardToHand(CardID.JOKER);
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

		onDrawHand?.Invoke();
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
		AddCardToHand(card);
	}

	public void AddCardToHand(CardData card)
	{
		hudController.DrawCard(card);
		hand.Add(card);

		if (selectedCardIndex == -1)
		{
			selectedCardIndex = 0;
			hudController.SetIndexSelected(selectedCardIndex);
		}
	}

	public void AddCardToHand(CardID id)
	{
		var card = cardLibrary.GetCard(id);
		hudController.DrawCard(card);
		hand.Add(card);

		if (selectedCardIndex == -1)
		{
			selectedCardIndex = 0;
			hudController.SetIndexSelected(selectedCardIndex);
		}
	}

	public void DiscardCard(int index, bool shouldDiscard = true)
	{
		if (shouldDiscard && !hand[index].noDiscard)
			discard.Add(cardLibrary.GetCard(hand[index].id));
		hand.RemoveAt(index);

		hudController.DiscardIndex(index);

		if (selectedCardIndex >= handSize)
			selectedCardIndex = handSize - 1;
		
		hudController.SetIndexSelected(selectedCardIndex);

		for (int i = 0; i < hand.Count; i++)
		{
			if (hand[i].id == CardID.FINISHING_STROKE)
			{
				var cardData = hand[i];
				cardData.baseDamage += 3;
				hand[i] = cardData;
			}
			
		}
	}

	public void UseSelectedCard()
	{
		var card = hand[selectedCardIndex];

		readiedCard = card;

		card.charges--;
		hudController.UseCharge();

		if (card.charges <= 0)
		{
			DiscardCard(selectedCardIndex, !card.exhaust);
		} else
		{
			hand[selectedCardIndex] = card;
		}
	}

	public void ResetToBaseline()
	{
		onDrawHand.RemoveAllListeners();
		hand.Clear();
		discard.Clear();
		deck.Clear();

		hudController.DiscardAll();

		
		foreach (var index in startingDeck.Split(','))
			deck.Add(cardLibrary.GetCard(int.Parse(index)));
			
		DrawHand();
	}

	public void AddCardToDeck(CardID card)
	{
		startingDeck += $",{(int)card}";
		ResetToBaseline();
	}

	public void ToggleInputLock(bool locked)
	{
		inputsLocked = locked;

		if (inputsLocked)
		{
			move.StartDeceleration();
		}
	}

}
