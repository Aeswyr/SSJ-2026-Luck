using UnityEngine;
using UnityEngine.Events;

public class HurtboxController : MonoBehaviour
{
    [SerializeField] private UnityEvent<HitData> onHit;
    [SerializeField] private UnityEvent<HitData> onMiss;
    public bool intangible, invincible;
    public void OnHit(HitData hitData)
    {
        if (intangible || invincible) {
            onMiss?.Invoke(hitData);
            return;
        }

        onHit?.Invoke(hitData);
    }
}
