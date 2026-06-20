using UnityEngine;
using UnityEngine.Events;

public class HurtboxController : MonoBehaviour
{
    [SerializeField] private UnityEvent<HitData> onHit;
    public void OnHit(HitData hitData)
    {
        onHit.Invoke(hitData);
    }
}
