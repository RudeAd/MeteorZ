using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float lifetime = 5f;
    public float speed = 5f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Förstör kulan vid träff
        if (!collision.gameObject.CompareTag("Bullet")) // Undvik att förstöra vid kollision med andra kulor
        {
            Destroy(gameObject);
        }
    }
}