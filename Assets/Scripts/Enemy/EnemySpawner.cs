using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnRate = 2f;
    public float minSpawnRate = 0.5f;
    public float spawnRadius = 10f;
    public int maxEnemiesInArena = 5;

    public float difficultyRampInterval = 30f;
    public float spawnRateDecrease = 0.2f;
    public int maxEnemiesIncrease = 2;
    public int maxEnemiesCap = 20;

    private int totalEnemiesSpawned = 0;
    private float timeSinceLastRamp = 0f;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 1f, spawnRate);
    }

    private void Update()
    {
        timeSinceLastRamp += Time.deltaTime;

        if (timeSinceLastRamp >= difficultyRampInterval)
        {
            timeSinceLastRamp = 0f;


            spawnRate = Mathf.Max(minSpawnRate, spawnRate - spawnRateDecrease);


            maxEnemiesInArena = Mathf.Min(maxEnemiesInArena + maxEnemiesIncrease, maxEnemiesCap);


            CancelInvoke(nameof(SpawnEnemy));
            InvokeRepeating(nameof(SpawnEnemy), 0f, spawnRate);

            Debug.Log($"Difficulty ramped up! New spawnRate: {spawnRate}, Max enemies: {maxEnemiesInArena}");
        }
    }


    void SpawnEnemy()
    {
        if (totalEnemiesSpawned >= maxEnemiesInArena)
            return;

        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab is null or was destroyed!");
            return;
        }

        Vector3 spawnPosition = transform.position + (Random.onUnitSphere * spawnRadius);
        spawnPosition.y = 0;

        Instantiate(enemyPrefab, spawnPosition, enemyPrefab.transform.rotation);

        totalEnemiesSpawned++;
    }

}
