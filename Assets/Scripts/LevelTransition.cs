using UnityEngine;

public class LevelTransition : MonoBehaviour
{

    public void OnInteract()
    {
        GameManager.Instance.ToNextLevel();
    }
}
