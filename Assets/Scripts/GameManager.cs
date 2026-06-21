using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject playerPrefab;
    [Space]
    [SerializeField] private GameObject[] levels;
    private LevelController currentLevel;
    private PlayerController player;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentLevel = FindAnyObjectByType<LevelController>();
    
        player = Instantiate(playerPrefab, currentLevel.GetSpawn(), Quaternion.identity).GetComponent<PlayerController>();
    }


    public void ToNextLevel()
    {
        Destroy(currentLevel.gameObject);

        player.ResetToBaseline();

        currentLevel = Instantiate(levels[0]).GetComponent<LevelController>();
    }

    public LevelController GetCurrentLevel()
    {
        return currentLevel;
    }
}
