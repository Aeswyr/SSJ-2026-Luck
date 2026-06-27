using UnityEngine;

public class BossController : MonoBehaviour
{
    [SerializeField] private string closingDialog;
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
        if (string.IsNullOrEmpty(closingDialog))
        {
            ShowVictory();
        } else
        {
            DialogManager.Instance.PlayConversation(closingDialog, () => ShowVictory());
        }

        void ShowVictory()
        {
            FindAnyObjectByType<PlayerController>().ToggleInputLock(true);
            hud.ShowWinScreen();
            Destroy(gameObject);
        }

        
    }
}
