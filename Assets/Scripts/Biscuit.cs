using UnityEngine;

public class Biscuit : MonoBehaviour
{
    [SerializeField] ScriptableObject buffObject;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        Debug.Log("[Biscuit] picked");

        var player = collision.GetComponent<MovementScript>();
        if (player != null && buffObject is IBuff buff)
        {
            buff.Apply(player);
        }

        if (RiskManagement.Instance != null)
        {
            float before = RiskManagement.Instance.Risk;
            RiskManagement.Instance.OnBiscuitPicked(10);
            Debug.Log($"[Biscuit] Risk {before} -> {RiskManagement.Instance.Risk}");
        }
        else
        {
            Debug.LogWarning("[Biscuit] RiskManagement.I is NULL");
        }

        Destroy(gameObject);
    }
}