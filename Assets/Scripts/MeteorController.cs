using UnityEngine;
using TMPro; // För TextMeshPro

public class MeteorController : MonoBehaviour
{
    private Rigidbody2D meteorRigidbody;
    private SpriteRenderer spriteRenderer;
    private LevelManager levelManager;

    [Header("Drag-and-Shoot Settings")]
    public LineRenderer aimLine;
    private Vector2 startDragPosition;
    private Vector2 endDragPosition;
    public float shootForce = 10f;
    public float maxDragLength = 5f;

    [Header("Player Stats")]
    public int lives = 3;
    public Sprite[] damageSprites;

    [Header("Physics Settings")]
    public float gravityScale = 0f; // Ingen gravitation
    public float spaceFriction = 0.99f; // Friktion för att sakta ner meteoren

    [Header("UI Settings")]
    public TextMeshProUGUI shotCounterText; // För TextMeshPro
    private int shotCounter = 0;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip damageSound;
    public AudioClip impactSound;

    [Header("Particle Effects")]
    public GameObject damageEffect;
    public GameObject impactEffect;

    void Start()
    {
        // Hämta komponenter
        meteorRigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Ingen gravitation för rymd
        meteorRigidbody.gravityScale = gravityScale;

        // Dölj siktlinjen i början
        aimLine.enabled = false;

        // Hitta LevelManager
        levelManager = Object.FindFirstObjectByType<LevelManager>();
        if (levelManager == null)
        {
            Debug.LogError("LevelManager not found in the scene!");
        }

        // Försök hitta ShotCounter-texten
        if (shotCounterText == null)
        {
            shotCounterText = GameObject.Find("ShotCounter")?.GetComponent<TextMeshProUGUI>();
        }
        if (shotCounterText == null)
        {
            Debug.LogError("ShotCounter TMP Text not found! Ensure it exists in the scene.");
        }

        // Kontrollera partikeleffekter
        if (damageEffect == null) Debug.LogError("Damage effect prefab is not assigned!");
        if (impactEffect == null) Debug.LogError("Impact effect prefab is not assigned!");

        // Uppdatera skottcounter
        UpdateShotCounter();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startDragPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            aimLine.enabled = true;
        }
        else if (Input.GetMouseButton(0))
        {
            endDragPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Begränsa draglängden
            Vector2 direction = endDragPosition - startDragPosition;
            if (direction.magnitude > maxDragLength)
            {
                direction = direction.normalized * maxDragLength;
                endDragPosition = startDragPosition + direction;
            }

            ShowAim(startDragPosition, endDragPosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            endDragPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Begränsa draglängden innan skjutning
            Vector2 direction = endDragPosition - startDragPosition;
            if (direction.magnitude > maxDragLength)
            {
                direction = direction.normalized * maxDragLength;
                endDragPosition = startDragPosition + direction;
            }

            ShootMeteor(startDragPosition, endDragPosition);
            aimLine.enabled = false;
        }
    }

    void FixedUpdate()
    {
        // Simulera inbromsning i rymden
        if (meteorRigidbody.linearVelocity.magnitude > 0.01f)
        {
            meteorRigidbody.linearVelocity *= spaceFriction;
        }
        else
        {
            meteorRigidbody.linearVelocity = Vector2.zero;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            TakeDamage();
        }

        if (audioSource != null && impactSound != null)
        {
            audioSource.PlayOneShot(impactSound);
        }

        // Skapa partikeleffekt vid kollision/studs
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }
    }

    void TakeDamage()
    {
        lives--;
        if (lives > 0)
        {
            spriteRenderer.sprite = damageSprites[3 - lives];
        }
        else
        {
            Debug.Log("Game Over triggered!");
            levelManager.GameOver();
        }

        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }

        // Skapa partikeleffekt vid skada
        if (damageEffect != null)
        {
            Instantiate(damageEffect, transform.position, Quaternion.identity);
        }
    }

    void ShowAim(Vector2 start, Vector2 end)
    {
        Vector2 direction = end - start;
        aimLine.positionCount = 2;
        aimLine.SetPosition(0, start);
        aimLine.SetPosition(1, start + direction);
    }

    void ShootMeteor(Vector2 start, Vector2 end)
    {
        Vector2 direction = start - end;
        meteorRigidbody.AddForce(direction * shootForce, ForceMode2D.Impulse);

        shotCounter++;
        UpdateShotCounter();
    }

    void UpdateShotCounter()
    {
        if (shotCounterText != null)
        {
            shotCounterText.text = "Shots: " + shotCounter;
        }
    }
}
