using UnityEngine;

public class EnemyDamagePlayer : MonoBehaviour
{
    public int damageAmount = 10;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damageAmount);
                Debug.Log($"Inimigo causou {damageAmount} de dano ao player.");
            }
        }
    }

}