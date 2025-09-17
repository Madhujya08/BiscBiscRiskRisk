using UnityEngine;

public class FitToCameraWidth : MonoBehaviour
{
    private void Start()
    {
        var cam = Camera.main;
        var sr = GetComponent<SpriteRenderer>();
        if (!cam || !sr) return;

        float worldH = cam.orthographicSize * 2f;
        float worldW = worldH * cam.aspect;
        float spriteW = sr.bounds.size.x;

        if (spriteW > 0f)
        {
            float s = worldH / spriteW;
            transform.localScale = new Vector3(s, s, 1f);
        }
    }
}
