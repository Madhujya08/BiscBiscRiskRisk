using UnityEngine;

[CreateAssetMenu(fileName = "SpeedBuff", menuName = "Scriptable Objects/SpeedBuff")]
public class SpeedBuff : ScriptableObject, IBuff
{
    public float speedMultiplier = 2f;
    public float duration = 5f;

    public void Apply(MovementScript target)
    {
        target.ApplyBiscuitBuff(speedMultiplier, duration);
    }
}
