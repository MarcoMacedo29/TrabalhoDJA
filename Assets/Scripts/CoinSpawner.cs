using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public GameObject coinPrefab;
    public float spawnInterval = 2f;
    public int maxCoinsInScene = 10;
    public float spawnRangeX = 5f;
    public float spawnRangeZ = 5f;
    public Transform playerTransform;

    [HideInInspector]
    public int currentCoins = 0;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnCoin), 1f, spawnInterval);
    }

    private void SpawnCoin()
    {
        if (currentCoins >= maxCoinsInScene || coinPrefab == null || playerTransform == null)
            return;

        Vector3 spawnPos = new Vector3(
            Random.Range(playerTransform.position.x - spawnRangeX, playerTransform.position.x + spawnRangeX),
            playerTransform.position.y,
            Random.Range(playerTransform.position.z - spawnRangeZ, playerTransform.position.z + spawnRangeZ)
        );

        GameObject coin = Instantiate(coinPrefab, spawnPos, Quaternion.identity);
        currentCoins++;

        // Atribuir o spawner à moeda
        Coin coinScript = coin.GetComponent<Coin>();
        if (coinScript != null)
        {
            coinScript.spawner = this;
        }
        else
        {
            Debug.LogWarning("A moeda instanciada não tem o script Coin!");
        }
    }

    public void OnCoinCollected()
    {
        currentCoins = Mathf.Max(0, currentCoins - 1);
    }
}
