using UnityEngine;
using TMPro; // För TextMeshPro
using System.Collections;

public class MeteorController : MonoBehaviour
{
    private Rigidbody2D meteorRigidbody;
    private SpriteRenderer spriteRenderer;
    private LevelManager levelManager;
    private CameraEffects cameraEffects; // För screenshake & damage overlay

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
    public TextMeshProUGUI shotCounterText;
    private int shotCounter = 0;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip damageSound;
    public AudioClip impactSound;
    public AudioClip drawSound; // Ljud vid uppspänning
    public AudioClip releaseSound; // Ljud vid släpp

    [Header("Particle Effects")]
    public GameObject damageEffect;
    public GameObject impactEffect;

    void Start()
    {
        meteorRigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        meteorRigidbody.gravityScale = gravityScale;
        aimLine.enabled = false;

        levelManager = Object.FindFirstObjectByType<LevelManager>();
        cameraEffects = Object.FindFirstObjectByType<CameraEffects>();

        if (levelManager == null)
        {
            Debug.LogError("LevelManager not found in the scene!");
        }

        if (shotCounterText == null)
        {
            shotCounterText = GameObject.Find("ShotCounter")?.GetComponent<TextMeshProUGUI>();
        }
        if (shotCounterText == null)
        {
            Debug.LogError("ShotCounter TMP Text not found! Ensure it exists in the scene.");
        }

        if (damageEffect == null) Debug.LogError("Damage effect prefab is not assigned!");
        if (impactEffect == null) Debug.LogError("Impact effect prefab is not assigned!");

        UpdateShotCounter();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startDragPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            aimLine.enabled = true;

            // Spela upp ljud när pilen dras
            PlaySound(drawSound);
        }
        else if (Input.GetMouseButton(0))
        {
            endDragPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
            Vector2 direction = endDragPosition - startDragPosition;
            if (direction.magnitude > maxDragLength)
            {
                direction = direction.normalized * maxDragLength;
                endDragPosition = startDragPosition + direction;
            }

            ShootMeteor(startDragPosition, endDragPosition);
            aimLine.enabled = false;

            // Spela upp ljud när pilen släpps
            PlaySound(releaseSound);
        }
    }

    void FixedUpdate()
    {
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

        PlaySound(impactSound);

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

        PlaySound(damageSound);

        if (damageEffect != null)
        {
            Instantiate(damageEffect, transform.position, Quaternion.identity);
        }

        // Lägg till screenshake och damage overlay
        if (cameraEffects != null)
        {
            cameraEffects.TriggerScreenShake();
            cameraEffects.ShowDamageOverlay();
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
            StartCoroutine(AnimateShotCounter());
        }
    }

    private IEnumerator AnimateShotCounter()
    {
        Vector3 originalScale = shotCounterText.transform.localScale;
        shotCounterText.transform.localScale = originalScale * 1.3f; // Förstora
        yield return new WaitForSeconds(0.1f);
        shotCounterText.transform.localScale = originalScale; // Återgå
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f); // Random pitch
            audioSource.PlayOneShot(clip);
        }
    }
}
