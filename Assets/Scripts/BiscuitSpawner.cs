using System.Collections;
using UnityEngine;

public class BiscuitSpawner : MonoBehaviour
{
    [Header("Prefabs & Area")]
    [SerializeField] GameObject biscuitPrefab;
    [SerializeField] BoxCollider2D spawnArea;

    [Header("Timing")]
    [SerializeField] float spawnInterval = 7f;
    [SerializeField] int startCount = 2;
    [SerializeField] int incrementPerWave = 1;
    [SerializeField] int maxOnField = 4;

    [Header("Spawn Rules")]
    [SerializeField] float minSpacing = 0.4f;
    [SerializeField] LayerMask blockMask;

    int wave = 0;

    private void Start()
    {
        if (spawnArea == null) spawnArea = GetComponent<BoxCollider2D>();
        StartCoroutine(SpawnLoop());
        
    }

    IEnumerator SpawnLoop()
    {
        var wait = new WaitForSeconds(spawnInterval);

        while (true)
        {
            yield return wait;

            int active = CountActiveBiscuits();
            if (active >= maxOnField) continue;

            int toSpawn = Mathf.Max(0, startCount + wave * incrementPerWave);
            toSpawn = Mathf.Min(toSpawn, maxOnField - active);

            for (int i = 0; i < toSpawn; i++)
            {
                Vector2 pos;
                if (TryGetSpawnPos(out pos))
                {
                    Instantiate(biscuitPrefab, pos, Quaternion.identity);
                }
            }
        }
    }

    int CountActiveBiscuits()
    {
        return Object.FindObjectsByType<Biscuit>(FindObjectsSortMode.None).Length;
    }

    bool TryGetSpawnPos(out Vector2 pos)
    {
        var bound = spawnArea.bounds;
        for (int tries = 0; tries < 20; tries++)
        {
            float x = Random.Range(bound.min.x, bound.max.x);
            float y = Random.Range(bound.min.y, bound.max.y);
            pos = new Vector2(x, y);

            bool blocked = false;

            if (blockMask.value != 0)
            {
                blocked |= Physics2D.OverlapCircle(pos, minSpacing, blockMask);
            }

            //blocked |= Physics2D.OverlapCircle(pos, minSpacing, blockMask);

            if (!blocked) return true;
        }

        pos = Vector2.zero;
        return false;
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (spawnArea == null) spawnArea = GetComponent<BoxCollider2D>();
        if (spawnArea == null) return;

        Gizmos.color = Color.yellow;
        var bound = spawnArea.bounds;
        Gizmos.DrawWireCube(bound.center, bound.size);
    }
#endif
}
