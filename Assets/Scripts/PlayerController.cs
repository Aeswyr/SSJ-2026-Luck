using System.Collections;
using System.Collections.Generic;
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
	[SerializeField] private HurtboxController hurtbox;
	[SerializeField] private BuffController buffController;
	private PlayerHUDController hudController;

	[Space]
	[Header("Spawnables")]
	[Space]
	[SerializeField] private GameObject cardPrefab;
	[SerializeField] private GameObject dealPrefab;

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
    private int facing = 1, attackRepeat;
	private int selectedCardIndex;
	private int maxHand = 4;
	private CardData readiedCard;
	private List<CardData> hand = new();
	private List<CardData> discard = new();
	private List<CardData> deck = new();
	private int handSize => hand.Count;

	private UnityEvent onDrawHand = new();
	private UnityEvent onDodge = new();
	private UnityEvent onThrow = new();

	private UnityEvent<CardData> onUseCard = new();

	private bool inputsLocked;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		InputHandler.Instance.FlushBuffer();
		
		foreach (var cam in FindObjectsByType<CameraFollow>(FindObjectsSortMode.None))
			cam.SetFollow(transform);

		hudController = FindAnyObjectByType<PlayerHUDController>();
        input = InputHandler.Instance;

		ResetToBaseline();

		hudController.SetHealth(3);
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

			hurtbox.intangible = true;
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
		hurtbox.intangible = false;
	}

	public void EndAction()
	{
		if (acting)
		{
			move.ResetCurves();
			move.StartDeceleration();
		}

		hurtbox.intangible = false;
		acting = false;
		cancellable = false;
	}

	public void SetCancellable()
	{
		cancellable = true;
	}

	public void CreateAttack()
	{
		Dictionary<CardID, int> cardCounts = new();
		foreach (var cd in hand)
		{
			if (!cardCounts.ContainsKey(cd.id))
				cardCounts.Add(cd.id, 0);
			cardCounts[cd.id]++;
		}

		HitData hitData = new()
		{
			baseDamage = readiedCard.baseDamage,
			bonusDamage = buffController.GetBuffCount(BuffType.EMPOWER),
			shouldStick = readiedCard.shouldStick
		};

		switch (readiedCard.id)
		{
			case CardID.HIGH_CARD:
				ThrowCard(hitData);
				break;
			case CardID.PAIR:
				ThrowCard(hitData, -1.2f);
				StartCoroutine(DelayCard());
				IEnumerator DelayCard()
				{
					yield return new WaitForSeconds(0.1f);
					ThrowCard(hitData, -0.8f);
				}
				break;
			case CardID.FULL_HOUSE:
				float shotgunLifetime = 0.2f;
				float shotgunVariance = 0.25f;
				ThrowCard(hitData, speed: 26 + Random.Range(0, 3), rotX: facing, rotY: Random.Range(0.05f, 0.1f), lifetime: shotgunLifetime + Random.Range(0, shotgunVariance));
				ThrowCard(hitData, speed: 20 + Random.Range(0, 9), rotX: facing, rotY: Random.Range(-0.05f, -0.1f), lifetime: shotgunLifetime + Random.Range(0, shotgunVariance));
				ThrowCard(hitData, speed: 22 + Random.Range(0, 7), rotX: facing, rotY: Random.Range(0.01f, 0.04f), lifetime: shotgunLifetime + Random.Range(0, shotgunVariance));
				ThrowCard(hitData, speed: 24 + Random.Range(0, 5), rotX: facing, rotY: Random.Range(-0.01f, -0.04f), lifetime: shotgunLifetime + Random.Range(0, shotgunVariance));
				ThrowCard(hitData, speed: 28, rotX: facing, rotY: 0, lifetime: shotgunLifetime + Random.Range(0, shotgunVariance));
				break;
			case CardID.JOKER:
				ThrowCard(hitData, speed: 22);
				break;
			case CardID.HIDDEN_ACE:
				ThrowCard(hitData, speed: 36, pierce: true);
				break;
			case CardID.FINISHING_STROKE:
				ThrowCard(hitData, speed: 42);
				break;
			case CardID.REND:
				foreach (var stick in FindObjectsByType<CardStickable>(FindObjectsSortMode.None))
				{
					var hurtbox = stick.GetComponentInChildren<HurtboxController>();
					if (hurtbox != null) {
						HitData hit = hitData;
						hit.baseDamage = hit.baseDamage * stick.cards;
						hit.bonusDamage = hit.bonusDamage * stick.cards;
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
					AddCardToHand(CardID.JOKER, true);}
				break;
			case CardID.ALL_IN:
				for (int handSize = hand.Count; handSize > 0; handSize--){
					ThrowCard(hitData, speed: 25, rotX: facing, rotY: Random.Range(-0.13f, 0.13f));
					ThrowCard(hitData, speed: 25, rotX: -facing, rotY: Random.Range(-0.13f, 0.13f));
					ThrowCard(hitData, speed: 25, rotX: Random.Range(-3f, 3f), rotY: 1);
					ThrowCard(hitData, speed: 25, rotX: Random.Range(-3f, 3f), rotY: 1);
				}

				while (hand.Count > 0)
					DiscardCard(0);
				break;
			case CardID.CALLED_SHOT:
				{
					var ray = Physics2D.Raycast(transform.position + new Vector3(facing * 1.5f, -1), facing * Vector2.right, 128, LayerMask.GetMask(new string[] {"Entity"}));
					if (ray && ray.collider.transform.parent != null)
					{
						BuffController buff = ray.collider.transform.parent.GetComponentInChildren<BuffController>();
						if (buff != null)
						{
							buff.AddBuff(BuffType.MARK);
						}
					}
				}
				break;
			case CardID.CALLBACK:
				{
					var ray = Physics2D.Raycast(transform.position + new Vector3(facing * 1.5f, -1), facing * Vector2.right, 128, LayerMask.GetMask(new string[] {"Entity"}));
					if (ray && ray.collider.transform.parent != null)
					{
						BuffController buff = ray.collider.transform.parent.GetComponentInChildren<BuffController>();
						if (buff != null)
						{
							buff.AddBuff(BuffType.COMEDY);
						}
					}
				}
				break;
			case CardID.PEERLESS_FOCUS:
				{
					var ray = Physics2D.Raycast(transform.position + new Vector3(facing * 1.5f, -1), facing * Vector2.right, 128, LayerMask.GetMask(new string[] {"Entity"}));
					if (ray && ray.collider.transform.parent != null)
					{
						BuffController buff = ray.collider.transform.parent.GetComponentInChildren<BuffController>();
						if (buff != null)
						{
							buff.AddBuff(BuffType.LOOT);
						}
					}
				}
				break;
			case CardID.BLEED_OUT:
				{
					var ray = Physics2D.Raycast(transform.position + new Vector3(facing * 1.5f, -1), facing * Vector2.right, 128, LayerMask.GetMask(new string[] {"Entity"}));
					if (ray && ray.collider.transform.parent != null)
					{
						BuffController buff = ray.collider.transform.parent.GetComponentInChildren<BuffController>();
						if (buff != null)
						{
							buff.AddBuff(BuffType.BLEED);
						}
					}
				}
				break;
			case CardID.THOUSAND_CUTS:
				{
					var ray = Physics2D.Raycast(transform.position + new Vector3(facing * 1.5f, -1), facing * Vector2.right, 128, LayerMask.GetMask(new string[] {"Entity"}));
					if (ray && ray.collider.transform.parent != null)
					{
						BuffController buff = ray.collider.transform.parent.GetComponentInChildren<BuffController>();
						if (buff != null)
						{
							buff.AddBuff(BuffType.PAIN);
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

					AddCardToHand(CardID.JOKER, true);
					AddCardToHand(CardID.JOKER, true);
					AddCardToHand(CardID.JOKER, true);
				}
				break;
			case CardID.MISSED_CONNECTION:
				onDodge.AddListener(CardAction_MissedConnection);
				void CardAction_MissedConnection()
				{
					AddCardToHand(CardID.JOKER, true);
				}
				break;
    		case CardID.INSIGHT:
				DrawCard();
				DrawCard();
				break;
			case CardID.FOOLIN_AROUND:
				AddCardToHand(CardID.JOKER, true);
				AddCardToHand(CardID.JOKER, true);
				AddCardToHand(CardID.JOKER, true);
				break;
			case CardID.DOUBLE_DOWN:
				onUseCard.AddListener(CardAction_DoubleDown);
				void CardAction_DoubleDown(CardData card)
				{
					AddCardToHand(card, true);
					AddCardToHand(card, true);

					onUseCard.RemoveListener(CardAction_DoubleDown);
				}
				break;
			case CardID.SECOND_CHANCE:
				GetComponent<EntityController>().ApplyHealing(1);
				break;
			case CardID.JACKPOT:
				RemoveCardFromDeck(CardID.JACKPOT);
				List<CardData> rarePool = cardLibrary.GetAllCardsOfRarity(CardRarity.RARE);
				AddCardToDeck(rarePool[Random.Range(0, rarePool.Count)].id);
				AddCardToDeck(rarePool[Random.Range(0, rarePool.Count)].id);
				AddCardToDeck(rarePool[Random.Range(0, rarePool.Count)].id);
				break;
			case CardID.LUCKY_SEVENS:
				RemoveCardFromDeck(CardID.LUCKY_SEVENS);
				List<CardData> uncommonPool = cardLibrary.GetAllCardsOfRarity(CardRarity.UNCOMMON);
				AddCardToDeck(uncommonPool[Random.Range(0, uncommonPool.Count)].id);
				AddCardToDeck(uncommonPool[Random.Range(0, uncommonPool.Count)].id);
				AddCardToDeck(uncommonPool[Random.Range(0, uncommonPool.Count)].id);
				AddCardToDeck(uncommonPool[Random.Range(0, uncommonPool.Count)].id);
				AddCardToDeck(uncommonPool[Random.Range(0, uncommonPool.Count)].id);
				AddCardToDeck(uncommonPool[Random.Range(0, uncommonPool.Count)].id);
				AddCardToDeck(uncommonPool[Random.Range(0, uncommonPool.Count)].id);
				break;
			case CardID.FLUSH:
				hitData.baseDamage *= hand.Count;

				int highest = 0;
				foreach (var count in cardCounts)
					if (count.Value > highest)
						highest = count.Value;
				
				if (highest >= 5)
					hitData.baseDamage = (int)(hitData.baseDamage * 1.5f);

				ThrowCard(hitData);
				break;
			case CardID.INNER_STRENGTH:
				buffController.AddBuff(BuffType.EMPOWER);
				break;
			case CardID.RAISE_THE_STAKES:
				buffController.AddBuff(BuffType.EMPOWER);
				buffController.AddBuff(BuffType.PAIN);
				break;
			case CardID.DEALERS_ADVANTAGE:
				onUseCard.AddListener(CardAction_DEALERADV);
				void CardAction_DEALERADV(CardData card)
				{
					DealCard(0.5f + 0.1f * Random.Range(0, 7));
				}
				break;
			case CardID.TRIPLE_DEAL:
				onUseCard.AddListener(CardAction_TRIPLEDEAL);
				void CardAction_TRIPLEDEAL(CardData card)
				{
					DealCard(0.5f);
					DealCard(0.7f);
					DealCard(0.9f);

					onUseCard.RemoveListener(CardAction_TRIPLEDEAL);
				}
				break;
			case CardID.LUCKY_DEAL:
				onUseCard.AddListener(CardAction_LUCKYDEAL);
				void CardAction_LUCKYDEAL(CardData card)
				{
					DealCard(0.5f);
					DealCard(0.7f);
					DealCard(0.9f);
					DealCard(1.1f);
					DealCard(1.3f);
					DealCard(1.5f);
					DealCard(1.7f);

					onUseCard.RemoveListener(CardAction_LUCKYDEAL);
				}
				break;
			default:
				break;
		}
	}

	private void ThrowCard(HitData hitData, float yPos = -1, float speed = 28, float rotX = 0, float rotY = 0, float lifetime = 0, bool pierce = false)
	{
		if (rotX == 0 && rotY == 0) {
			var card = Instantiate(cardPrefab, transform.position + new Vector3(facing * 1.5f, yPos), Quaternion.identity, GameManager.Instance.GetCurrentLevel().GetObjectParent())
				.GetComponent<ProjectileController>()
					.SetVelocity(facing * speed);
			if (lifetime > 0)
				card.SetLifetime(lifetime);
			if (pierce)
				card.SetEntityPierce();
			card.Init(transform, hitData);
		
		} else
		{
			Vector2 rotation = new (rotX, rotY);
			var card = Instantiate(cardPrefab, transform.position + (Vector3)(rotation.normalized * new Vector3(1.5f, 0)) + new Vector3(0, yPos), Quaternion.FromToRotation(rotation.x * Vector2.right, rotation.normalized), GameManager.Instance.GetCurrentLevel().GetObjectParent())
				.GetComponent<ProjectileController>()
					.SetVelocity(speed * rotation.normalized);
			if (lifetime > 0)
				card.SetLifetime(lifetime);
			if (pierce)
				card.SetEntityPierce();
			card.Init(transform, hitData);
		}
	}

	private void DealCard(float delay)
	{
		HitData hitData = new()
		{
			baseDamage = 1,
			bonusDamage = buffController.GetBuffCount(BuffType.EMPOWER),
			shouldStick = false
		};

		var deal = Instantiate(dealPrefab, transform.position + new Vector3(facing * -(4 + Random.Range(0f, 3f)), Random.Range(-1f, 4f)), Quaternion.identity, GameManager.Instance.GetCurrentLevel().GetObjectParent());
		deal.GetComponent<ProjectileController>()
				.SetVelocity(0)
				.Init(transform, hitData);
		deal.GetComponent<DealProjectileController>()
			.Init(delay, 28, facing, transform.position.y);
	}

	public void OnRecieveHit(int hp)
	{
		
	}

	public void OnHealthChange(int hp)
	{
		hudController.SetHealth(hp);
	}

	public void OnDeath()
	{
		ToggleInputLock(true);
		animator.SetTrigger("dead");

		hudController.ShowDeathScreen();
	}

	public void OnDodge()
	{
		VFXManager.Instance.CreateToast("miss", transform.position + new Vector3(Random.Range(-0.75f, 0.75f), 1.5f + Random.Range(0, 0.75f)), Color.lightGray);
		onDodge?.Invoke();
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

		hudController.SetDeckCount(deck.Count, startingDeck.Split(',').Length);
	}

	public void AddCardToHand(CardData card, bool manifested = false)
	{

		if (manifested)
		{
			card.noDiscard = true;
			card.exhaust = true;
		}

		hudController.DrawCard(card);
		hand.Add(card);




		if (selectedCardIndex == -1)
		{
			selectedCardIndex = 0;
			hudController.SetIndexSelected(selectedCardIndex);
		}
	}

	public void AddCardToHand(CardID id, bool manifested = false)
	{
		var card = cardLibrary.GetCard(id);

		AddCardToHand(card, manifested);
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

		onUseCard?.Invoke(card);

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
		onDodge.RemoveAllListeners();
		onUseCard.RemoveAllListeners();
		hand.Clear();
		discard.Clear();
		deck.Clear();

		buffController.RemoveAllBuff();

		hudController.DiscardAll();
		
		foreach (var index in startingDeck.Split(','))
			deck.Add(cardLibrary.GetCard(int.Parse(index)));
			
		DrawHand();
	}

	public void AddCardToDeck(CardID card)
	{
		startingDeck += $",{(int)card}";
		hudController.SetDeckCount(deck.Count, startingDeck.Split(',').Length);
		ResetToBaseline();
	}

	public void RemoveCardFromDeck(CardID card)
	{
		List<string> deck = new(startingDeck.Split(','));
		for (int i = 0; i < deck.Count; i++)
		{
			if (int.Parse(deck[i]) == (int)card)
			{
				deck.RemoveAt(i);
				break;
			}
		}

		startingDeck = deck[0];
		for (int i = 1; i < deck.Count; i++) {
			startingDeck += ',' + deck[i]; 
		}

		ResetToBaseline();
	}

	public List<CardID> GetBaselineDeck()
	{
		List<CardID> deck = new();

		foreach (var index in startingDeck.Split(','))
			deck.Add((CardID)int.Parse(index));

		return deck;
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
