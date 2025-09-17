using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] float lifeTime = 3f;
    [SerializeField] float speed = 12f;

    private Vector2 dir = Vector2.right;
    
    void OnEnable()
    {
        if (lifeTime > 0) Destroy(gameObject, lifeTime);
    }

    public void Init(Vector2 direction)
    {
        dir = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
        transform.position += (Vector3)(dir * 0.05f);

        float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(ang, Vector3.forward);
        //transform.rotation = Mathf.Lerp(ang,  1.2f);                                                        ***** chain of thought for later *********
    }

    private void Update()
    {
        transform.position += (Vector3)(dir * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger) return;

        if (collision.CompareTag("Player"))
            collision.GetComponent<PlayerHealth>()?.TakeHit(1);

        Destroy(gameObject);
    }
}
