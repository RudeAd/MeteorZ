using UnityEngine;

public class MeteorController : MonoBehaviour
{
    private Rigidbody2D meteorRigidbody; // Hanterar fysiken f�r meteoren
    private SpriteRenderer spriteRenderer; // F�r att byta sprite n�r meteoren tar skada

    [Header("Drag-and-Shoot Settings")]
    public LineRenderer aimLine; // Linje som visar riktning och kraft
    private Vector2 startDragPosition; // D�r musen klickades ner
    private Vector2 endDragPosition;   // D�r musen sl�pptes
    public float shootForce = 10f;    // Hur h�rt meteoren skjuts iv�g

    [Header("Player Stats")]
    public int lives = 3;             // Meteorns liv
    public Sprite[] damageSprites;    // Sprites f�r olika skador

    [Header("Physics Settings")]
    public float gravityScale = 1f;   // Justerar gravitationens p�verkan

    void Start()
    {
        // H�mta komponenter
        meteorRigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // St�ll in gravitation
        meteorRigidbody.gravityScale = gravityScale;

        // D�lj siktlinjen i b�rjan
        aimLine.enabled = false;
    }

    void Update()
    {
        // Hanterar musinput f�r drag-and-shoot
        if (Input.GetMouseButtonDown(0)) // N�r musen klickas ner
        {
            startDragPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            aimLine.enabled = true; // Visa siktlinjen
        }
        else if (Input.GetMouseButton(0)) // N�r musen dras
        {
            endDragPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            ShowAim(startDragPosition, endDragPosition); // Visa pilen
        }
        else if (Input.GetMouseButtonUp(0)) // N�r musen sl�pps
        {
            endDragPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            ShootMeteor(startDragPosition, endDragPosition); // Skjut meteoren
            aimLine.enabled = false; // D�lj siktlinjen
        }
    }

    void FixedUpdate()
    {
        // Friktion f�r att sakta ner meteoren �ver tid
        if (meteorRigidbody.linearVelocity.magnitude > 0.1f)
        {
            meteorRigidbody.linearVelocity *= 0.99f; // Minskar hastigheten l�ngsamt
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Hantera kollision med zombies eller andra objekt
        if (collision.gameObject.CompareTag("Zombie"))
        {
            // G�r n�got, t.ex. minska zombie-liv (detta hanteras av zombie-scriptet)
        }
        else if (collision.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(); // Ta skada om meteoren tr�ffas av en kula
        }
    }

    void TakeDamage()
    {
        // Minska liv och byt sprite
        lives--;
        if (lives > 0)
        {
            spriteRenderer.sprite = damageSprites[3 - lives]; // Byt sprite baserat p� �terst�ende liv
        }
        else
        {
            // Om liv �r 0, starta om leveln eller spelet
            Debug.Log("Game Over!");
        }
    }

    void ShowAim(Vector2 start, Vector2 end)
    {
        // Visa en linje fr�n musens startposition till nuvarande position
        Vector2 direction = end - start; // Riktningen mellan punkterna
        aimLine.positionCount = 2; // S�tt tv� punkter i linjen
        aimLine.SetPosition(0, start); // Startpunkten
        aimLine.SetPosition(1, start + direction); // Slutpunkten
    }

    void ShootMeteor(Vector2 start, Vector2 end)
    {
        // Skjut meteoren baserat p� drag-and-shoot-mekaniken
        Vector2 direction = start - end; // Ber�kna riktningen
        meteorRigidbody.AddForce(direction * shootForce, ForceMode2D.Impulse); // Applicera kraft
    }
}
