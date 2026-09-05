using System;
using DG.Tweening;
using UnityEngine;

public class CardSelectVFX : MonoBehaviour
{
    [SerializeField] private SpriteRenderer cardIcon;
    [SerializeField] private SpriteRenderer cardFrame;
    [SerializeField] private CardLibrary cardLibrary;
    [SerializeField] private GameObject sparkleVFX;
    
    public void Init(CardID card, bool selected)
    {
        var cardData = cardLibrary.GetCard(card);
        cardIcon.sprite = cardData.icon;
        cardFrame.sprite = cardLibrary.GetFrame(cardData.rarity);

        if (selected)
        {
            sparkleVFX.SetActive(true);
            Sequence tween = DOTween.Sequence(gameObject);
            tween.SetDelay(1f).OnComplete(() => {
                cardFrame.DOFade(0, 1f);
                cardIcon.DOFade(0, 1f).OnComplete(() =>
                {
                    Destroy(gameObject);
                });
            }).Play();
        } else
        {
            sparkleVFX.SetActive(false);
            transform.DOMoveY(transform.localPosition.y + 1.5f, 0.5f).OnComplete(() =>
            {
                Destroy(gameObject);
            });
            cardFrame.DOFade(0, 0.5f);
            cardIcon.DOFade(0, 0.5f);
        }
    }
}
