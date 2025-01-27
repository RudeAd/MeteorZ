using System.Collections;
using UnityEngine;

public class ZombieController : MonoBehaviour
{
    private Rigidbody2D zombieRigidbody; // För att hantera fysik
    private SpriteRenderer spriteRenderer; // För att hantera utseende

    [Header("Zombie Settings")]
    public int health = 3; // Antal liv för zombien
    public float speed = 2f; // Rörelsehastighet
    public bool canShoot = false; // Om denna zombie kan skjuta
    public GameObject bulletPrefab; // Prefab för zombie-kulor
    public float shootInterval = 2f; // Tid mellan skott

    [Header("Patrol Settings")]
    public Transform[] patrolPoints; // Punkter zombien patrullerar mellan
    private int currentPointIndex = 0; // Nuvarande patrullpunkt

    private bool isMoving = true; // Om zombien rör sig eller står still
    private bool isHit = false; // Om zombien är mitt i "träff-effekt"

    [Header("Animation Settings")]
    public float hitScaleIncrease = 1.5f; // Hur mycket större zombien blir vid träff
    public float hitDuration = 0.2f; // Hur länge träff-effekten varar
    public float rotationAmount = 10f; // Rotation mellan -10 och 10 grader
    public float rotationSpeed = 5f; // Hastighet på rotationen

    [Header("Particle Systems")]
    public ParticleSystem hitEffect; // Partikelsystem vid träff
    public ParticleSystem deathEffect; // Partikelsystem vid död

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip idleSound;
    public AudioClip shootSound;

    void Start()
    {
        zombieRigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (canShoot)
        {
            StartCoroutine(ShootRoutine()); // Börja skjuta om zombien kan skjuta
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Spela idle-ljud i loop om det finns
        if (idleSound != null)
        {
            audioSource.clip = idleSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    void Update()
    {
        if (isMoving && patrolPoints.Length > 0)
        {
            MoveBetweenPoints();
        }

        // Rotationsanimation när zombien rör sig
        if (isMoving)
        {
            float rotation = Mathf.Sin(Time.time * rotationSpeed) * rotationAmount;
            transform.rotation = Quaternion.Euler(0, 0, rotation);
        }
    }

    void MoveBetweenPoints()
    {
        // Rör sig mot nästa patrullpunkt
        Transform targetPoint = patrolPoints[currentPointIndex];
        Vector2 direction = (targetPoint.position - transform.position).normalized;
        zombieRigidbody.linearVelocity = direction * speed;

        // Om zombien når punkten, gå vidare till nästa
        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }

        // Flippa spriten baserat på rörelseriktning
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = direction.x < 0; // Flippar åt vänster om zombien rör sig åt vänster
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Meteor"))
        {
            TakeDamage(1); // Ta skada om meteoren träffar
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        // Visa träffeffekt
        if (!isHit)
        {
            StartCoroutine(ShowHitEffect());
        }

        // Visa partikelsystem vid träff
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        if (health <= 0)
        {
            Die();
        }
    }

    IEnumerator ShowHitEffect()
    {
        isHit = true;

        // Spara ursprunglig skala
        Vector3 originalScale = transform.localScale;

        // Öka skalan tillfälligt
        transform.localScale = originalScale * hitScaleIncrease;

        // Vänta en stund
        yield return new WaitForSeconds(hitDuration);

        // Återställ skalan
        transform.localScale = originalScale;

        isHit = false;
    }

    void Die()
    {
        Debug.Log(gameObject.name + " is dead!");

        // Rapportera till LevelManager att en zombie har dödats
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.ZombieKilled();
        }

        // Visa partikelsystem vid död
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // Förstör objektet
        Destroy(gameObject);
    }

    IEnumerator ShootRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(shootInterval); // Vänta innan nästa skott
            ShootBullet();
        }
    }

    void ShootBullet()
    {
        if (bulletPrefab != null)
        {
            // Skapa en kula och ge den en rak rörelse
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
            bulletRigidbody.linearVelocity = Vector2.down * 5f; // Skjuter nedåt, anpassa riktningen om behövs
        }

        // Spela skjutljud
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }
}
