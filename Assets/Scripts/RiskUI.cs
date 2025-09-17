using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RiskUi : MonoBehaviour
{
    [SerializeField] Image fillImage;
    [SerializeField] TextMeshProUGUI riskText;
    [SerializeField] TextMeshProUGUI multText;

    private void OnEnable()
    {
        if (RiskManagement.Instance != null)
        {
            RiskManagement.Instance.OnRiskChanged += Refresh;
        }
    }

    private void OnDisable()
    {
        if (RiskManagement.Instance != null)
            RiskManagement.Instance.OnRiskChanged -= Refresh;
    }

    private void Update()
    {
        Refresh();
    }
    void Refresh()
    {
        if (RiskManagement.Instance == null || fillImage == null) return;

        float pct = Mathf.InverseLerp(0f, 100f, RiskManagement.Instance.Risk);
        fillImage.fillAmount = pct;

        if (riskText) 
            riskText.text = $"RISK {Mathf.RoundToInt(RiskManagement.Instance.Risk)}";

        if (multText)
            multText.text = $"x{RiskManagement.Instance.Multiplier:0.0}";
    }
}
