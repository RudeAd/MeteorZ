using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public int playerLives = 3;
    public int currentLevel = 1;
    public int totalZombies = 0;
    public GameObject gameOverUI;
    public GameObject winUI;
    public GameObject countdownText;
    private CameraEffects cameraEffects;
    private bool isBossLevel = false;

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
        SceneManager.sceneLoaded += OnSceneLoaded;
        InitializeLevel();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeLevel();
    }

    private void InitializeLevel()
    {
        totalZombies = 0; // Återställ räknaren
        totalZombies = FindObjectsByType<ZombieController>(FindObjectsSortMode.None).Length;
        isBossLevel = (SceneManager.GetActiveScene().name == "Level5");
        
        Debug.Log("Zombies in level: " + totalZombies);

        if (gameOverUI != null) gameOverUI.SetActive(false);
        if (winUI != null) winUI.SetActive(false);

        // Försöker hitta countdownText om den inte är satt i Inspector
        if (countdownText == null)
        {
            countdownText = GameObject.Find("CountdownText");
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
        Debug.Log("Zombie killed! Remaining: " + totalZombies);
        if (totalZombies <= 0 && !isBossLevel)
        {
            LevelComplete();
        }
    }

    public void BossDefeated()
    {
        if (isBossLevel)
        {
            WinGame();
        }
    }

    public void GameOver()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
        StartCoroutine(RestartLevelCountdown());
    }

    private void FailState()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
        StartCoroutine(RestartLevelCountdown());
    }

    private IEnumerator RestartLevelCountdown()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        for (int i = 3; i > 0; i--)
        {
            if (countdownText != null)
            {
                countdownText.GetComponent<UnityEngine.UI.Text>().text = i.ToString();
                countdownText.SetActive(true);
            }
            yield return new WaitForSeconds(1f);
        }
        ReloadLevelWithFade();
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

    // I LevelManager.cs, ändra från private till public
public void WinGame()
{
    if (winUI != null)
    {
        winUI.SetActive(true);
    }
    Debug.Log("Spelet är slut! Du vann!");
}

    // I LevelManager.cs
    private void LevelComplete()
    {
        Debug.Log($"Level {currentLevel} completed");
        currentLevel++;
        
        // Specifik hantering för boss-level
        if (currentLevel == 5)
        {
            if (cameraEffects != null)
            {
                cameraEffects.FadeOutAndLoadScene("Level5");
            }
            else
            {
                SceneManager.LoadScene("Level5");
            }
            return;
        }
        
        // Om vi är på sista leveln eller över
        if (currentLevel > 5)
        {
            WinGame();
            return;
        }
        
        // Ladda nästa level
        string nextLevel = "Level" + currentLevel;
        if (cameraEffects != null)
        {
            cameraEffects.FadeOutAndLoadScene(nextLevel);
        }
        else
        {
            SceneManager.LoadScene(nextLevel);
        }
    }
}
