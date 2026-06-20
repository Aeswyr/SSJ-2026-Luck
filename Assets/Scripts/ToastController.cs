using System;
using TMPro;
using UnityEngine;

public class ToastController : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;
    [SerializeField] private AnimationCurve alpha;
    [SerializeField] private AnimationCurve drift;
    [SerializeField] private float maxDrift;

    private float startTime;
    private Vector2 startPos;

    void Start()
    {
        startTime = Time.time;
        startPos = transform.position;    
    }

    void FixedUpdate()
    {
        text.alpha = alpha.Evaluate(Time.time - startTime);
        transform.position = startPos + maxDrift * drift.Evaluate(Time.time - startTime) * Vector2.up;
    }
}
