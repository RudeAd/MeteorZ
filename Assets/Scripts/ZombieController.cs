using System.Collections;
using UnityEngine;

public class ZombieController : MonoBehaviour
{
    private Rigidbody2D zombieRigidbody;
    private SpriteRenderer spriteRenderer;

    [Header("Zombie Settings")]
    public int health = 3;
    public float speed = 2f;
    public bool canShoot = false;
    public GameObject bulletPrefab;
    public float shootInterval = 2f;
    
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    private int currentPointIndex = 0;
    private bool isMoving = true;
    private bool isHit = false;

    [Header("Animation Settings")]
    public float hitScaleIncrease = 1.5f;
    public float hitDuration = 0.2f;
    public float rotationAmount = 10f;
    public float rotationSpeed = 5f;
    public float shakeIntensity = 0.1f;
    public float shakeDuration = 0.1f;

    [Header("Particle Systems")]
    public ParticleSystem hitEffect;
    public ParticleSystem deathEffect;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip idleSound;
    public AudioClip shootSound;

    private Color originalColor;
    
    void Start()
    {
        zombieRigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        if (canShoot)
        {
            StartCoroutine(ShootRoutine());
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

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

        if (isMoving)
        {
            float rotation = Mathf.Sin(Time.time * rotationSpeed) * rotationAmount;
            transform.rotation = Quaternion.Euler(0, 0, rotation);
        }
    }

    void MoveBetweenPoints()
    {
        Transform targetPoint = patrolPoints[currentPointIndex];
        Vector2 direction = (targetPoint.position - transform.position).normalized;
        zombieRigidbody.linearVelocity = direction * speed;

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = direction.x > 0;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Meteor"))
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (!isHit)
        {
            StartCoroutine(ShowHitEffect());
        }

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
        Vector3 originalScale = transform.localScale;
        transform.localScale = originalScale * hitScaleIncrease;
        spriteRenderer.color = Color.red;
        StartCoroutine(ShakeEffect());
        
        yield return new WaitForSeconds(hitDuration);
        
        transform.localScale = originalScale;
        spriteRenderer.color = originalColor;
        isHit = false;
    }

    IEnumerator ShakeEffect()
    {
        Vector3 originalPosition = transform.position;
        for (float t = 0; t < shakeDuration; t += Time.deltaTime)
        {
            transform.position = originalPosition + (Vector3)Random.insideUnitCircle * shakeIntensity;
            yield return null;
        }
        transform.position = originalPosition;
    }

    void Die()
    {
        Debug.Log(gameObject.name + " is dead!");
        
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.ZombieKilled();
        }
        
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        
        Destroy(gameObject);
    }

    IEnumerator ShootRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(shootInterval - 0.5f);
            StartCoroutine(TelegraphShoot());
            yield return new WaitForSeconds(0.5f);
            ShootBullet();
        }
    }

    IEnumerator TelegraphShoot()
    {
        spriteRenderer.color = Color.yellow;
        yield return new WaitForSeconds(0.5f);
        spriteRenderer.color = originalColor;
    }

    // I ZombieController.cs, uppdatera ShootBullet()
    void ShootBullet()
    {
        if (bulletPrefab != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();
            BulletController bulletController = bullet.GetComponent<BulletController>();
            
            if (bulletController != null)
            {
                bulletController.lifetime = 5f; // Sätt livstiden
            }
            
            bulletRigidbody.linearVelocity = Vector2.down * 5f;
        }
        // ... resten av koden
    }
}
