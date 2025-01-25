using UnityEngine;

public class MeteorController : MonoBehaviour
{
    private Rigidbody2D meteorRigidbody; // Hanterar fysiken för meteoren
    private SpriteRenderer spriteRenderer; // För att byta sprite när meteoren tar skada
    private LevelManager levelManager; // Referens till LevelManager

    [Header("Drag-and-Shoot Settings")]
    public LineRenderer aimLine; // Linje som visar riktning och kraft
    private Vector2 startDragPosition; // Där musen klickades ner
    private Vector2 endDragPosition;   // Där musen släpptes
    public float shootForce = 10f;    // Hur hårt meteoren skjuts iväg

    [Header("Player Stats")]
    public int lives = 3;             // Meteorns liv
    public Sprite[] damageSprites;    // Sprites för olika skador

    [Header("Physics Settings")]
    public float gravityScale = 1f;   // Justerar gravitationens påverkan

    void Start()
    {
        // Hämta komponenter
        meteorRigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Ställ in gravitation
        meteorRigidbody.gravityScale = gravityScale;

        // Dölj siktlinjen i början
        aimLine.enabled = false;

        // Hitta LevelManager med den nya metoden
        levelManager = Object.FindFirstObjectByType<LevelManager>();

        if (levelManager == null)
        {
            Debug.LogError("LevelManager not found in the scene!");
        }
    }

    void Update()
    {
        // Hanterar musinput för drag-and-shoot
        if (Input.GetMouseButtonDown(0)) // När musen klickas ner
        {
            startDragPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            aimLine.enabled = true; // Visa siktlinjen
        }
        else if (Input.GetMouseButton(0)) // När musen dras
        {
            endDragPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            ShowAim(startDragPosition, endDragPosition); // Visa pilen
        }
        else if (Input.GetMouseButtonUp(0)) // När musen släpps
        {
            endDragPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            ShootMeteor(startDragPosition, endDragPosition); // Skjut meteoren
            aimLine.enabled = false; // Dölj siktlinjen
        }
    }

    void FixedUpdate()
    {
        // Friktion för att sakta ner meteoren över tid
        if (meteorRigidbody.linearVelocity.magnitude > 0.1f)
        {
            meteorRigidbody.linearVelocity *= 0.99f; // Minskar hastigheten långsamt
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Hantera kollision med zombies eller andra objekt
        if (collision.gameObject.CompareTag("Zombie"))
        {
            // Gör något, t.ex. minska zombie-liv (detta hanteras av zombie-scriptet)
        }
        else if (collision.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(); // Ta skada om meteoren träffas av en kula
        }
    }

    void TakeDamage()
    {
        // Minska liv och byt sprite
        lives--;
        if (lives > 0)
        {
            spriteRenderer.sprite = damageSprites[3 - lives]; // Byt sprite baserat på återstående liv
        }
        else
        {
            // Om liv är 0, starta om leveln eller spelet
            Debug.Log("Game Over triggered!");
            levelManager.GameOver(); // Anropa GameOver
        }
    }

    void ShowAim(Vector2 start, Vector2 end)
    {
        // Visa en linje från musens startposition till nuvarande position
        Vector2 direction = end - start; // Riktningen mellan punkterna
        aimLine.positionCount = 2; // Sätt två punkter i linjen
        aimLine.SetPosition(0, start); // Startpunkten
        aimLine.SetPosition(1, start + direction); // Slutpunkten
    }

    void ShootMeteor(Vector2 start, Vector2 end)
    {
        // Skjut meteoren baserat på drag-and-shoot-mekaniken
        Vector2 direction = start - end; // Beräkna riktningen
        meteorRigidbody.AddForce(direction * shootForce, ForceMode2D.Impulse); // Applicera kraft
    }
}
