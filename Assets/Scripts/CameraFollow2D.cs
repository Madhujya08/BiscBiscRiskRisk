using UnityEngine;

[ExecuteAlways]
public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    public float smoothSpeed = 5f;
    public float lookAheadY = 1.5f;
    public float zOffset = -10f;

    void LateUpdate()
    {
        Vector3 desired = target.position + new Vector3(0f, lookAheadY, 0f);
        desired.z = zOffset;

        float t = Time.deltaTime * smoothSpeed;
        transform.position = Vector3.Lerp(transform.position, desired, t);
    }
}
