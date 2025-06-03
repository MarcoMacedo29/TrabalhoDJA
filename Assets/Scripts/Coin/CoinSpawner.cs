using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [Header("Coin Settings")]
    public GameObject coinPrefab;
    public float spawnInterval = 2f;
    public int maxCoinsInScene = 10;

    [Header("Spawn Area (baseado no centro da arena)")]
    public Vector3 arenaCenter = new Vector3(0, 0, 0);
    public float spawnRangeX = 10f;
    public float spawnRangeZ = 10f;

    [HideInInspector]
    public int currentCoins = 0;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnCoin), 1f, spawnInterval);
    }

    private void SpawnCoin()
    {
        if (currentCoins >= maxCoinsInScene || coinPrefab == null)
            return;

        Vector3 spawnPos = new Vector3(
            Random.Range(arenaCenter.x - spawnRangeX, arenaCenter.x + spawnRangeX),
            arenaCenter.y + 0.5f, 
            Random.Range(arenaCenter.z - spawnRangeZ, arenaCenter.z + spawnRangeZ)
        );

        GameObject coin = Instantiate(coinPrefab, spawnPos, Quaternion.identity);
        currentCoins++;

        Coin coinScript = coin.GetComponent<Coin>();
        if (coinScript != null)
        {
            coinScript.spawner = this;
        }
    }

    public void OnCoinCollected()
    {
        currentCoins = Mathf.Max(0, currentCoins - 1);
    }
}
