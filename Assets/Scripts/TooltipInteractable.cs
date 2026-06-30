using DG.Tweening;
using TMPro;
using UnityEngine;

public class TooltipInteractable : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;

    public void Start()
    {
        text.alpha = 0;
    }

    public void FadeIn()
    {
        text.DOFade(1, 0.25f);
    }

    public void FadeOut()
    {
        text.DOFade(0, 0.25f);
    }
}
