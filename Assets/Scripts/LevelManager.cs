using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public int playerLives = 3;
    public int currentLevel = 1;
    public int totalZombies = 0;
    public GameObject gameOverUI;
    public GameObject winUI; // UI för vinst

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeLevel();
    }

    private void InitializeLevel()
    {
        totalZombies = FindObjectsByType<ZombieController>(FindObjectsSortMode.None).Length;

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }

        if (winUI != null)
        {
            winUI.SetActive(false);
        }
    }

    public void PlayerHit()
    {
        playerLives--;

        if (playerLives <= 0)
        {
            FailState();
        }
    }

    public void ZombieKilled()
    {
        totalZombies--;

        if (totalZombies <= 0)
        {
            LevelComplete();
        }
    }
        public void GameOver()
{
    if (gameOverUI != null)
    {
        gameOverUI.SetActive(true); // Visa Game Over UI
    }

    // Ladda om samma nivå efter 2 sekunder
    Invoke(nameof(ReloadLevel), 2f);
}

    private void FailState()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        Invoke(nameof(ReloadLevel), 2f);
    }

    private void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        playerLives = 3;
    }

    private void LevelComplete()
    {
        currentLevel++;
        if (currentLevel >= SceneManager.sceneCountInBuildSettings)
        {
            // Spelaren har klarat alla nivåer
            if (winUI != null)
            {
                winUI.SetActive(true); // Visa vinstmeddelande
            }

            Debug.Log("Spelet är slut! Du vann!");
            return;
        }

        // Ladda nästa nivå
        SceneManager.LoadScene(currentLevel);
    }
}
