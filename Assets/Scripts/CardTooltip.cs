using TMPro;
using UnityEngine;

public class CardTooltip : MonoBehaviour
{
    [SerializeField] private CardLibrary cardLibrary;
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI cardBody;
    [SerializeField] private TextMeshProUGUI cardTips;

    public void LoadCard(CardID id)
    {
        cardName.text = cardLibrary.GetCard(id).name;
        cardBody.text = cardLibrary.GetCardDescription(id);
        cardTips.text = "";
    }
}
