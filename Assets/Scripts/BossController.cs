using UnityEngine;

public class BossController : MonoBehaviour
{
    [SerializeField] EntityController entity;
    PlayerHUDController hud;
    void Start()
    {
        hud = FindAnyObjectByType<PlayerHUDController>();

        hud.ToggleBossHealth(true);
        hud.updateBossHealth(entity.GetMaxHealth(), entity.GetMaxHealth());
    }

    public void OnHit(int hp)
    {
        hud.updateBossHealth(hp, entity.GetMaxHealth());
    }
    public void OnDeath()
    {
        FindAnyObjectByType<PlayerController>().ToggleInputLock(true);
        hud.ShowWinScreen();

        Destroy(gameObject);
    }
}
