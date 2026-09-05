using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHUDController : MonoBehaviour
{
    [SerializeField] private Transform cardHolder;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform lifeBarParent;
    [SerializeField] private Transform heartHolder;
    [SerializeField] private GameObject heartObject;
    [SerializeField] private GameObject hitAnimationObject;
    [SerializeField] private Transform lifeBarHolder;
    [SerializeField] private GameObject lifeBarObject;
    [SerializeField] private GameObject lifeCapObject;
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private Image bossHealth;

    [SerializeField] private TextMeshProUGUI deckCount, deckTotal;
    [SerializeField] private CardDisplay cardPreview;

    List<CardController> cards = new();
    int activeIndex;

    void Start()
    {
        deathScreen.SetActive(false);
        winScreen.SetActive(false);
        ToggleBossHealth(false);
        cardPreview.gameObject.SetActive(false);
    }

    public void ShowCardPreview(CardID card)
    {
        cardPreview.SetCard(card);
        cardPreview.gameObject.SetActive(true);
    }

    public void HideCardPreview()
    {
        cardPreview.gameObject.SetActive(false);
    }
    public void DrawCard(CardData cardData)
    {
        var card = Instantiate(cardPrefab, cardHolder).GetComponent<CardController>();
        card.Init(cardData);
        card.SetSelected(false);
        cards.Add(card);

        card.transform.localScale = new (0, 1, 1);
        card.transform.DOScaleX(1, 0.1f);
    }

    public void DiscardIndex(int index)
    {
        var card = cards[index];
        card.SetSelected(false);
        card.transform.DOScaleX(0, 0.25f).OnComplete(() =>
        {
            Destroy(card.gameObject);
        });
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
            for (int i = heartHolder.childCount - dif; i < heartHolder.childCount; i++)
            {
                lifeBarParent.DOShakePosition(0.5f);
                var hit = Instantiate(hitAnimationObject, transform);
                hit.SetActive(true);
                hit.transform.position = heartHolder.GetChild(i).transform.position;
                hit.transform.DOJump(hit.transform.position + 20 * Vector3.up, 20, 1, 0.5f).OnComplete(() =>
                {
                    Destroy(hit);
                });
                hit.transform.DOBlendableRotateBy(Random.Range(-45, 45) * Vector3.forward, 0.5f);
                hit.GetComponent<Image>().DOFade(0, 0.5f);
                Destroy(heartHolder.GetChild(i).gameObject);
            }
        }
    }

    public void UpdateMaxHealth(int maxHP)
    {
        foreach (Transform child in lifeBarHolder)
            Destroy(child.gameObject);

        for (int i = 0; i < maxHP; i++)
            Instantiate(lifeBarObject, lifeBarHolder).SetActive(true);

        Instantiate(lifeCapObject, lifeBarHolder).SetActive(true);
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
        ScreenWipeManager.Instance.PlayWipeOn(() =>
        {
            SceneManager.LoadScene("MenuScene");
            ScreenWipeManager.Instance.PlayWipeOff();
        });
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
