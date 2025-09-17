using UnityEngine;

public class ExitZone : MonoBehaviour
{
    [SerializeField] float bankHoldTime = 1.5f;
    [SerializeField] GameObject prompt;
    float hold;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        hold = 0f;
        
        if (prompt)
        {
            prompt.SetActive(true); 
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        hold += Time.deltaTime;

        if (hold >= bankHoldTime)
        {
            RiskManagement.Instance?.Bank();
            hold = 0f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        hold = 0f;

        if(prompt)
        {
            prompt.SetActive(false);
        }
    }


}
