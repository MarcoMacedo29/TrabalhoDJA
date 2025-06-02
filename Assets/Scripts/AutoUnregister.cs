using UnityEngine;

public class AutoUnregister : MonoBehaviour
{
    public CoinSpawner spawner;

    private void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.OnCoinCollected();
        }
    }
}
