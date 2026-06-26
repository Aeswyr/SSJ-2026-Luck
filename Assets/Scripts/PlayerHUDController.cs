using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHUDController : MonoBehaviour
{
    [SerializeField] private Transform cardHolder;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform heartHolder;
    [SerializeField] private GameObject heartObject;
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private Image bossHealth;

    [SerializeField] private TextMeshProUGUI deckCount, deckTotal;

    List<CardController> cards = new();
    int activeIndex;

    void Start()
    {
        deathScreen.SetActive(false);
        winScreen.SetActive(false);
        ToggleBossHealth(false);
    }
    public void DrawCard(CardData cardData)
    {
        var card = Instantiate(cardPrefab, cardHolder).GetComponent<CardController>();
        card.Init(cardData);
        card.SetSelected(false);
        cards.Add(card);
    }

    public void DiscardIndex(int index)
    {
        Destroy(cards[index].gameObject);
        cards.RemoveAt(index);
    }

    public void DiscardAll()
    {
        foreach (var card in cards)
            Destroy(card.gameObject);
        
        cards.Clear();
    }

    public void SetIndexSelected(int index)
    {
        if (index < 0)
            return;

        foreach (var card in cards)
            card.SetSelected(false);
            
        activeIndex = index;
        cards[activeIndex].SetSelected(true);
    }

    public void UseCharge()
    {
        cards[activeIndex].RemoveCharge();
    }

    public void SetHealth(int health)
    {
        if (health > heartHolder.childCount)
        {
            int dif = health - heartHolder.childCount;
            for (int i = 0; i < dif; i++)
            {
                Instantiate(heartObject, heartHolder).SetActive(true);
            }
        } else if (health < heartHolder.childCount)
        {
            int dif = heartHolder.childCount - health;
            for (int i = 0; i < dif; i++)
            {
                Destroy(heartHolder.GetChild(i).gameObject);
            }
        }
    }

    public void ToggleBossHealth(bool toggle)
    {
        bossHealth.transform.parent.gameObject.SetActive(toggle);
    }

    public void updateBossHealth(int hp, int max)
    {
        bossHealth.fillAmount = (float)hp / max;
    }

    public void ShowDeathScreen()
    {
        deathScreen.SetActive(true);
    }

    public void ShowWinScreen()
    {
        winScreen.SetActive(true);
    }

    public void OnReturnPressed()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void OnDeckPressed()
    {
        DeckViewController.Instance.ShowDeckView(false);
    }

    public void SetDeckCount(int count, int total)
    {
        deckCount.text = count.ToString();
        deckTotal.text = total.ToString();
    }
}
