using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ZombieDinosaurBoss : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D bossRigidbody;
    
    [Header("Boss Settings")]
    public int health = 10;
    public Slider healthBar; // UI för bossens liv
    public float attackInterval = 5f;
    
    [Header("Shooting Attack")]
    public GameObject bulletPrefab;
    public Transform[] bulletSpawnPoints;
    public float bulletSpeed = 5f;
    public Sprite shootSprite;
    public Sprite idleSprite;

    [Header("Tail Sweep Attack")]
    public Transform tail;
    public float tailWindupTime = 1.5f;
    public float tailSweepTime = 0.5f;
    public float tailDamageForce = 5f;
    
    [Header("Damage Feedback")]
    public Color damageColor = Color.red;
    private Color originalColor;
    public float damageFlashTime = 0.2f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        bossRigidbody = GetComponent<Rigidbody2D>();
        originalColor = spriteRenderer.color;
        healthBar.maxValue = health;
        healthBar.value = health;
        
        StartCoroutine(AttackPattern());
    }

    IEnumerator AttackPattern()
    {
        while (health > 0)
        {
            yield return new WaitForSeconds(attackInterval);
            StartCoroutine(ShootingAttack());
            yield return new WaitForSeconds(attackInterval);
            StartCoroutine(TailSweepAttack());
        }
    }

    IEnumerator ShootingAttack()
    {
        spriteRenderer.sprite = shootSprite; // Byt sprite för att telegrafera attacken
        yield return new WaitForSeconds(0.5f); // Kort varningstid

        foreach (Transform spawnPoint in bulletSpawnPoints)
        {
            GameObject bullet = Instantiate(bulletPrefab, spawnPoint.position, Quaternion.identity);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.linearVelocity = (spawnPoint.up * bulletSpeed);
        }

        spriteRenderer.sprite = idleSprite; // Byt tillbaka till idle
    }

    IEnumerator TailSweepAttack()
    {
        // Förvarning genom att röra svansen åt vänster
        Vector3 startPos = tail.position;
        Vector3 windupPos = startPos + Vector3.left * 1f;
        float elapsed = 0;
        
        while (elapsed < tailWindupTime)
        {
            elapsed += Time.deltaTime;
            tail.position = Vector3.Lerp(startPos, windupPos, elapsed / tailWindupTime);
            yield return null;
        }

        // Svep åt höger (attack)
        elapsed = 0;
        Vector3 attackPos = startPos + Vector3.right * 3f;
        
        while (elapsed < tailSweepTime)
        {
            elapsed += Time.deltaTime;
            tail.position = Vector3.Lerp(windupPos, attackPos, elapsed / tailSweepTime);
            yield return null;
        }

        // Återställ svansen
        tail.position = startPos;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        healthBar.value = health;
        
        StartCoroutine(DamageFlash());

        if (health <= 0)
        {
            Die();
        }
    }

        void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Meteor")) // Kontrollera om det är meteoren
        {
            TakeDamage(1); // Bossen tar 1 skada per träff
        }
    }

    IEnumerator DamageFlash()
    {
        spriteRenderer.color = damageColor;
        yield return new WaitForSeconds(damageFlashTime);
        spriteRenderer.color = originalColor;
    }

    void Die()
    {
        Debug.Log("Boss defeated!");
        Destroy(gameObject);
    }
}
