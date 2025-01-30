using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public int playerLives = 3;
    public int currentLevel = 1;
    public int totalZombies = 0;
    public GameObject gameOverUI;
    public GameObject winUI;
    private CameraEffects cameraEffects;

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
        cameraEffects = Object.FindFirstObjectByType<CameraEffects>();
        InitializeLevel();
    }

    private void InitializeLevel()
    {
        totalZombies = FindObjectsByType<ZombieController>(FindObjectsSortMode.None).Length;
        Debug.Log("Zombies in level: " + totalZombies);

        if (gameOverUI != null) gameOverUI.SetActive(false);
        if (winUI != null) winUI.SetActive(false);
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
        Debug.Log("Zombie killed! Remaining: " + totalZombies);
        if (totalZombies <= 0)
        {
            LevelComplete();
        }
    }

    public void GameOver()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
        Invoke(nameof(ReloadLevelWithFade), 2f);
    }

    private void FailState()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
        Invoke(nameof(ReloadLevelWithFade), 2f);
    }

    private void ReloadLevelWithFade()
    {
        if (cameraEffects != null)
        {
            cameraEffects.FadeOutAndLoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        playerLives = 3;
    }

    private void LevelComplete()
    {
        currentLevel++;
        if (currentLevel >= SceneManager.sceneCountInBuildSettings)
        {
            if (winUI != null)
            {
                winUI.SetActive(true);
            }
            Debug.Log("Spelet är slut! Du vann!");
            return;
        }
        if (cameraEffects != null)
        {
            cameraEffects.FadeOutAndLoadScene("Level" + currentLevel);
        }
        else
        {
            SceneManager.LoadScene("Level" + currentLevel);
        }
    }
}
