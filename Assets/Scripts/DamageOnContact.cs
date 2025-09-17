using UnityEngine;

public class DamageOnContact : MonoBehaviour
{
    [SerializeField] int damage = 1;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
            collision.collider.GetComponent<PlayerHealth>()?.TakeHit(damage);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            collision.GetComponent<PlayerHealth>()?.TakeHit(damage);
    }
}
