using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private PlayerStatus playerStatus;

    private void Start()
    {
        transform.position = new Vector3(38f, 1f, -25.7f); // Y = 1 para estar acima do chão
        playerStatus = GetComponentInChildren<PlayerStatus>();
    }

    public void TakeDamage(int damage)
    {
        if (playerStatus != null)
        {
            playerStatus.TakeDamage(damage);
        }
    }

    public void Die()
    {
        Debug.Log("Player foi destru�do!");
        // Aqui voc� pode desativar, destruir o gameobject, ou o que quiser na morte
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
