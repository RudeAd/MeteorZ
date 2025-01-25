using UnityEngine;
using UnityEngine.SceneManagement; // För att ladda/scener

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance; // Singleton
    public int playerLives = 3;          // Spelarens liv
    public int currentLevel = 1;         // Nuvarande nivå
    public int totalZombies = 0;         // Hur många zombies på banan
    public GameObject gameOverUI;        // UI för "Game Over" (valfritt)

    private void Awake()
    {
        // Singleton-mönster: bara en LevelManager får existera
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Behåll LevelManager mellan scener
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Kolla om vi behöver initialisera något
        InitializeLevel();
    }

    private void InitializeLevel()
    {
        // Resetta zombies på banan
        totalZombies = FindObjectsByType<ZombieController>(FindObjectsSortMode.None).Length;

        // Se till att eventuellt Game Over UI är gömt
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
    }

    public void PlayerHit()
    {
        // Hantera spelarens liv om meteoren träffas
        playerLives--;

        if (playerLives <= 0)
        {
            FailState(); // Spelaren dog
        }
    }

    public void ZombieKilled()
    {
        // Minska antalet zombies på banan
        totalZombies--;

        // Om alla zombies är döda, avsluta nivån
        if (totalZombies <= 0)
        {
            LevelComplete();
        }
    }

    public void GameOver()
    {
        // Gör GameOver tillgänglig för andra skript
        FailState();
    }

    private void FailState()
    {
        // Visa Game Over UI och ladda om nivån
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        // Ladda om samma nivå efter 2 sekunder
        Invoke(nameof(ReloadLevel), 2f);
    }

    private void ReloadLevel()
    {
        // Ladda om den aktuella nivån
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        playerLives = 3; // Återställ spelarens liv (valfritt)
    }

    private void LevelComplete()
    {
        // Avsluta nivå och ladda nästa nivå
        currentLevel++;
        if (currentLevel >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log("Spelet är slut!");
            return;
        }

        // Ladda nästa nivå
        SceneManager.LoadScene(currentLevel);
    }
}
