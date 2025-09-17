using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] int maxHP = 3;
    [SerializeField] float invulnTime = 0.8f;

    [Header("Visuals")]
    [SerializeField] SpriteRenderer sprite;

    int hp;
    bool invuln;
    bool dead;

    private void Awake()
    {
        hp = maxHP;
        if (!sprite) sprite = GetComponentInChildren<SpriteRenderer>();
    }

    public void TakeHit(int dmg = 1)
    {
        if (dead || invuln) return;

        if (RiskManagement.Instance && RiskManagement.Instance.Risk >= 100f)
            dmg = hp;

        hp = Mathf.Max(0, hp - dmg);

        if (hp <= 0) Die();
        else StartCoroutine(IFrames());
    }

    IEnumerator IFrames()
    {
        invuln = false;

        float t = 0;
        const float blink = 0.1f;
        while (t < invulnTime)
        {
            if (sprite) sprite.enabled = !sprite.enabled;
            yield return new WaitForSeconds(blink);
            t += blink;
        }
        if (sprite) sprite.enabled = true;
        invuln = false;
    }

    void Die()
    {
        dead = true;
        var move = GetComponent<MovementScript>();
        if (move) move.enabled = false;

        var rb = GetComponent<Rigidbody>();
        if (rb) rb.linearVelocity = Vector2.zero;

        GameOverUI.ui?.ShowGameOver();
    }
}
