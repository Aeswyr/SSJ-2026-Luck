using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckViewController : Singleton<DeckViewController>
{
    [SerializeField] private CardLibrary cardLibrary;
    [SerializeField] private GameObject viewParent;
    [SerializeField] private GameObject removeButton;
    [SerializeField] private Transform cardParent;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI cardBody;
    [SerializeField] private TextMeshProUGUI cardTips;


    private bool isRemoval;
    private CardID selectedCard;


    void Start()
    {
        viewParent.SetActive(false);
        removeButton.SetActive(false);
    }

    public void ShowDeckView(bool canRemoveCard)
    {
        Time.timeScale = 0;

        isRemoval = canRemoveCard;
        title.text = isRemoval ? "Choose a Card to Remove" : "Your Deck";

        PopulateDeck();

        removeButton.SetActive(false);
        viewParent.SetActive(true);
    }

    private void PopulateDeck()
    {
        for (int i = cardParent.childCount - 1; i >= 0; i--)
        {
            Destroy(cardParent.GetChild(i).gameObject);
        }

        var player = FindAnyObjectByType<PlayerController>();
        foreach (var id in player.GetBaselineDeck()) {
            var card = Instantiate(cardPrefab, cardParent);
            card.GetComponent<CardController>().Init(cardLibrary.GetCard(id));
            card.GetComponent<Button>().onClick.AddListener(delegate {OnCardPressed(id);});
        }
    }

    private void OnCardPressed(CardID id)
    {
        selectedCard = id;
        
        if (isRemoval)
        {
            removeButton.SetActive(true);
        }

        cardName.text = cardLibrary.GetCard(id).name;
        cardBody.text = cardLibrary.GetCardDescription(id);
        cardTips.text = "";
    }

    public void OnRemovePressed()
    {
        var player = FindAnyObjectByType<PlayerController>();

        player.RemoveCardFromDeck(selectedCard);

        Close();
    }

    public void Close()
    {
        viewParent.SetActive(false);
        Time.timeScale = 1;

        InputHandler.Instance.FlushBuffer();
    }
}
