using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class ParallaxChunkRepeater : MonoBehaviour
{
    public Transform cam;
    [Range(0f, 1f)] public float parallax = 0.2f;
    public float overlap = -0.02f;

    readonly List<Transform> chunks = new List<Transform>();
    float tileWidth;
    Vector3 startPos, camStart;
    Camera camComp;
    bool ready;

    private void Awake()
    {
        if (!cam)
        {
            var main = Camera.main;
            if (main) cam = main.transform;
        }
        camComp = cam ? cam.GetComponent<Camera>() : null;

        chunks.Clear();
        foreach (Transform t in transform)
        {
            var sr = t.GetComponent<SpriteRenderer>();
            if (sr) chunks.Add(t);
        }

        if (chunks.Count < 2)
        {
            Debug.LogWarning($"{name} : need at least 2 child sprites with SpriteRenderer.");
            enabled = false; return;

        }
        var firstSR = chunks[0].GetComponent<SpriteRenderer>();
        tileWidth = firstSR.bounds.size.x + overlap;

        var anchorPos = chunks[0].position;
        for (int i = 0; i < chunks.Count; i++)
            chunks[i].position = new Vector3(anchorPos.x + i * tileWidth, anchorPos.y, anchorPos.z);

        startPos = transform.position;
        camStart = cam ? cam.position : Vector3.zero;
        ready = cam && camComp && tileWidth > 0f;
    } 


    private void LateUpdate()
    {
        if (!ready) return;
        Vector3 camDelta = cam.position - camStart;
        transform.position = new Vector3(
            startPos.x + camDelta.x * parallax,
            startPos.y + camDelta.y * parallax,
            startPos.z);


        float viewHalfWidth = camComp.orthographicSize * camComp.aspect;
        float leftEdge = cam.position.x - viewHalfWidth * 1.1f;
        float rightEdge = cam.position.x + viewHalfWidth * 1.1f;

        Transform leftMost = chunks[0], rightMost = chunks[0];
        for (int i = 1; i < chunks.Count; i++)
        {
            if (chunks[i].position.x < leftMost.position.x) leftMost = chunks[i];
            if (chunks[i].position.x > rightMost.position.x) rightMost = chunks[i];
        }

        if (leftMost.position.x + tileWidth < leftEdge)
        {
            leftMost.position = new Vector3(rightMost.position.x + tileWidth, leftMost.position.y, leftMost.position.z);
        }

        if (rightMost.position.x - tileWidth > rightEdge)
        {
            rightMost.position = new Vector3(leftMost.position.x - tileWidth, rightMost.position.y, rightMost.position.z);
        }
    }
}
