using UnityEngine;
using UnityEngine.Events;

public class HurtboxController : MonoBehaviour
{
    [SerializeField] private UnityEvent<HitData> onHit;
    [SerializeField] private UnityEvent<HitData> onMiss;
    public bool intangible, invincible; // gameplay options for removing hitboxes. intangible does not count a collision, invincible does for things like sticks
    public bool disabled; // system option for removing hitboxes
    public void OnHit(HitData hitData)
    {
        if (disabled)
            return;
        if (intangible || invincible) {
            onMiss?.Invoke(hitData);
            return;
        }

        onHit?.Invoke(hitData);
    }

    public bool TouchDisabled()
    {
        return intangible || disabled;
    }
}
