using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ParallaxManager : Singleton<ParallaxManager>
{
    [SerializeField] private GameObject layerTemplate;
    private List<ParallaxLayer> activeParallaxes = new();

    private Transform follow;

    void Start()
    {
        follow = transform.parent;
    }

    public void SetParallax(List<ParallaxInfo> info)
    {
        foreach (var active in activeParallaxes)
            Destroy(active.gameObject);
        activeParallaxes.Clear();

        foreach (var i in info)
        {
            ParallaxLayer layer = new()
            {
                gameObject = Instantiate(layerTemplate, transform),
                speedModifier = i.speedModifier
            };

            foreach (var render in layer.gameObject.GetComponentsInChildren<SpriteRenderer>()){
                render.sprite = i.layerSprite;
                render.sortingOrder = activeParallaxes.Count;
            }
            
            layer.gameObject.SetActive(true);

            activeParallaxes.Add(layer);
        }
    }

    public void Update()
    {
        foreach (var active in activeParallaxes)
        {
            active.gameObject.transform.localPosition = new (follow.position.x * -active.speedModifier, 0);
        }
    }

    private struct ParallaxLayer
    {
        public float speedModifier;
        public GameObject gameObject;
    }
}

[Serializable] public struct ParallaxInfo
{
    public Sprite layerSprite;
    public float speedModifier;
}