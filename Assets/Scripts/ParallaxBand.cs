using UnityEngine;

public class ParallaxBand : MonoBehaviour
{
    public Transform cam;                     // leave empty, auto-uses Main Camera
    public Vector2 multiplier = new(0.2f, 0); // smaller x = farther layer

    float width;              // world width of the sprite
    Vector3 startPos, camStart;
    SpriteRenderer sr;

    void OnEnable()
    {
        if (!cam) cam = Camera.main ? Camera.main.transform : null;
        sr = GetComponent<SpriteRenderer>();

        if (!cam || !sr)
        {
            Debug.LogWarning($"{name}: Missing {(cam ? "" : "camera ")}{(sr ? "" : "SpriteRenderer")}");
            enabled = false;
            return;
        }

        width = sr.bounds.size.x;          // includes scale & tiled size
        startPos = transform.position;
        camStart = cam.position;
    }

    void LateUpdate()
    {
        var camDelta = cam.position - camStart;

        // regular parallax target
        Vector3 target = new(
            startPos.x + camDelta.x * multiplier.x,
            startPos.y + camDelta.y * multiplier.y,
            startPos.z
        );

        // pick the sprite copy (k * width) that sits nearest to the camera
        if (width > 0f)
        {
            float k = Mathf.Round((cam.position.x - target.x) / width);
            target.x += k * width;
        }

        transform.position = target;
    }
    }
