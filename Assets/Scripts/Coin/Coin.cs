using UnityEngine;

public class Coin : MonoBehaviour
{
    public CoinSpawner spawner;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CoinManager.Instance.AddCoins(10);

            if (spawner != null)
            {
                spawner.OnCoinCollected();
            }
            else
            {
                Debug.LogWarning("Coin n�o tem spawner atribu�do!");
            }

            Destroy(gameObject);
        }
    }
}
