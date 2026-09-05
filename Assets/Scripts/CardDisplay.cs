using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
public class CardDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI cardBody;
    [SerializeField] private Image cardIcon;
    [SerializeField] private Image cardFrame;
    [SerializeField] private GameObject tooltipDisplay;
    [SerializeField] private Transform tooltipHolder;
    [SerializeField] private CardLibrary library;

    void Start()
    {
        tooltipDisplay.SetActive(false);
    }

    public void SetCard(CardID id) {
        var card = library.GetCard(id);
        var cardDesc = library.GetCardDescription(id);

        cardName.text = card.name;
        cardBody.text = cardDesc.description;
        cardIcon.sprite = card.icon;
        cardFrame.sprite = library.GetFrame(card.rarity);
        
        for (int i = 1; i < tooltipHolder.childCount; i++)
        {
            Destroy(tooltipHolder.GetChild(i).gameObject);
        }

        foreach (var keyword in cardDesc.keywords)
        {
            var tip = Instantiate(tooltipDisplay, tooltipHolder);
            tip.transform.Find("KeywordName").GetComponent<TextMeshProUGUI>().text = library.GetKeywordDisplayName(keyword);
            tip.transform.Find("KeywordText").GetComponent<TextMeshProUGUI>().text = library.GetKeywordText(keyword);
            tip.SetActive(true);
        }
    }
}
