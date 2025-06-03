using UnityEngine;

public class Coin : MonoBehaviour
{
    public CoinSpawner spawner;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CoinManager.Instance.AddCoins(2);

            if (spawner != null)
            {
                spawner.OnCoinCollected();
            }
            else
            {
                Debug.LogWarning("Coin não tem spawner atribuído!");
            }

            Destroy(gameObject);
        }
    }
}
