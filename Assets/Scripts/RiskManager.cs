using System;
using UnityEngine;

public class RiskManagement : MonoBehaviour
{
    public static RiskManagement Instance { get; private set; }

    [Header("Tuning")]
    [SerializeField] float maxRisk = 100f;
    [SerializeField] float riskPerBiscuit = 0.5f;
    [SerializeField] float passiveRiskPerSec = 0.5f;
    [SerializeField] float bankResetRisk = 10f;

    [Header("Live Read Only ")]
    [SerializeField] float risk = 0f;
    [SerializeField] int carriedBiscuit = 0;
    [SerializeField] int unbankedValue = 0;
    [SerializeField] int bankedScore = 0;

    public float Risk => risk;
    public int UnbankedValue => unbankedValue;
    public int BankedValue => bankedScore;

    public float Multiplier => 1f + (risk / 100f) * 2f;

    public event Action OnRiskChanged;
    public event Action OnScoreChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Update()
    {
        if (carriedBiscuit > 0 && risk < maxRisk)
        {
            risk = Mathf.Min(maxRisk, risk + passiveRiskPerSec * Time.deltaTime);
            OnRiskChanged?.Invoke();
        }
    }

    public void OnBiscuitPicked( int biscuitPoints = 10)
    {
        carriedBiscuit++;
        unbankedValue += biscuitPoints;
        AddRisk(riskPerBiscuit);

    }

    public void AddRisk(float amount)
    {
        float before = risk;
        risk = Mathf.Clamp(before + amount, 0f, maxRisk);
        Debug.Log($"[Risk] AddRisk {amount}: {before} -> {risk}");
        OnRiskChanged?.Invoke();
    }

    public void Bank()
    {
        int gained = Mathf.RoundToInt(unbankedValue * Multiplier);
        bankedScore += gained;

        unbankedValue = 0;
        carriedBiscuit = 0;
        risk = Mathf.Clamp(bankResetRisk, 0f, maxRisk);

        OnRiskChanged?.Invoke();
        OnScoreChanged?.Invoke();

    }
    public void OnCaught()
    {
        unbankedValue = 0;
        carriedBiscuit = 0;
        OnScoreChanged?.Invoke();
    }
}
