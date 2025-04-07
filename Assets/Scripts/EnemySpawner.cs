using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnRate = 2f;
    public float spawnRadius = 10f;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 1f, spawnRate);
    }

    void SpawnEnemy()
    {
        Vector3 spawnPosition = transform.position + (Random.onUnitSphere * spawnRadius);
        spawnPosition.y = 0;

        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}
