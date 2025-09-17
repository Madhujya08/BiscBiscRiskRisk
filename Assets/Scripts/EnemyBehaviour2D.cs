using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBehaviour2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform player;
    Rigidbody2D rb;

    [Header("Risk threshold (inclusive")]
    [SerializeField] int tier2At = 20;
    [SerializeField] int tier3At = 40;
    [SerializeField] int tier4At = 60;
    [SerializeField] int tier5At = 80;
    [SerializeField] int tier6At = 100;

    [Header("PER TIER CHASE SPEED")]
    [SerializeField] float[] speedByTier = { 1.4f, 1.8f, 2.3f, 2.8f, 3.2f, 3.6f };

    [Header("PER-TIER DETECT RADIUS")]
    [SerializeField] float[] detectRadiusByTier = { 3f, 4f, 5f, 6f, 7f, 9f };

    [Header("Melee")]
    [SerializeField] float meleeWindup = 0.22f;
    [SerializeField] float meleeDashSpeed = 9f;
    [SerializeField] float meleeDashTime = 0.25f;
    [SerializeField] float meleeTriggerRange = 5.5f;
    [SerializeField] float meleeCooldown = 2.0f;

    [Header("Projectile")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] float projectileCooldown = 1.6f;
    [SerializeField] float projectileRange = 8f;

    [Header("Tier 6")]
    [SerializeField] float t6MeleeCooldown = 1.0f;
    [SerializeField] float t6ProejectileCooldown = 0.7f;
    [SerializeField] int t6ProjectileBurst = 3;
    [SerializeField] float t6BurstInterval = 0.08f;

    [SerializeField] float shootSpawnOffset = 0.6f;

    //runtime
    bool isAttacking;
    float meleeCd;
    float shootCD;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        rb.gravityScale = 0;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Update()
    {
        if (meleeCd > 0) meleeCd -= Time.deltaTime;
        if (shootCD > 0) shootCD -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        int tier = GetTier();
        float speed = speedByTier[Mathf.Clamp(tier, 0, speedByTier.Length - 1)];
        float detectR = detectRadiusByTier[Mathf.Clamp(tier, 0, detectRadiusByTier.Length - 1)];

        Vector2 toPlayer = (player.position - transform.position);
        float dist = toPlayer.magnitude;

        bool detected = dist <= detectR;

        if (isAttacking)
        {
            return;
        }

        if (detected)
        {
            if (tier >= 4) TryMelee(tier, dist, toPlayer);
            if (tier >= 5) TryShoot(tier, dist, toPlayer);
        }

        if (detected)
        {
            Vector2 dir = (dist > 0.001f) ? (toPlayer / dist) : Vector2.zero;
            rb.linearVelocity = dir * speed;
        }

        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    int GetTier()
    {
        float risk = RiskManagement.Instance != null ? RiskManagement.Instance.Risk : 0f;

        if (risk >= tier6At) return 5;
        if (risk >= tier5At) return 4;
        if (risk >= tier4At) return 3;
        if (risk >= tier3At) return 2;
        if (risk >= tier2At) return 1;
        return 0;
    }

    void TryMelee(int tier, float dist, Vector2 toPlayer)
    {
        float cd = (tier >= 5) ? t6MeleeCooldown : meleeCooldown;
        if (meleeCd > 0) return;
        if (dist > meleeTriggerRange) return;

        StartCoroutine(MeleeDash(toPlayer.normalized, tier));
        meleeCd = cd;
    }

    IEnumerator MeleeDash(Vector2 dir, int tier)
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(meleeWindup);

        float dashT = meleeDashTime;
        float speed = meleeDashSpeed * (tier >= 5 ? 1.15f : 1f);
        while (dashT > 0f)
        {
            rb.linearVelocity = dir * speed;
            dashT -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        rb.linearVelocity = Vector2.zero;
        isAttacking = false;
    }

    void TryShoot(int tier, float dist , Vector2 toPlayer)
    {
        if (projectilePrefab == null) return;
        if (dist > projectileRange) return;

        float cd = (tier >= 5) ? t6ProejectileCooldown : projectileCooldown;
        if (shootCD > 0) return;

        if (tier >= 5 && t6ProjectileBurst >1)
        {
            StartCoroutine(BurstShoot(toPlayer));
        }
        else
        {
            ShootOnce(toPlayer);
        }
        shootCD = cd;
    }

    IEnumerator BurstShoot(Vector2 toPlayer)
    {
        int n = t6ProjectileBurst;
        for (int i = 0; i < n; i++)
        {
            ShootOnce(toPlayer);
            if (i < n - 1) yield return new WaitForSeconds(t6BurstInterval);
        }
    }

    void ShootOnce(Vector2 toPlayer)
    {
        if (!projectilePrefab) return;

        Vector2 dir = toPlayer.sqrMagnitude > 0.001f ? toPlayer.normalized : Vector2.right;

        var myCol = GetComponent<Collider2D>();
        float push = shootSpawnOffset;
        if (myCol) push = Mathf.Max(push, myCol.bounds.extents.magnitude + 0.1f);

        Vector2 spawnPos = (Vector2)transform.position + dir * push;

        var go = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        var projCol = go.GetComponent<Collider2D>();
        if (projCol && myCol) Physics2D.IgnoreCollision(projCol, myCol, true);

        var proj = go.GetComponent<EnemyProjectile>();
        if (proj) proj.Init(dir);
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position.normalized , 3f);
            return;
        }

        int tier = GetTier();
        float r = detectRadiusByTier[Mathf.Clamp(tier, 0, detectRadiusByTier.Length - 1)];
        Gizmos.color = Color.Lerp(Color.green, Color.red, tier / 5);
        Gizmos.DrawWireSphere(transform.position, r);
    }
}
