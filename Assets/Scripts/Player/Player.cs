using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private PlayerStatus playerStatus;

    private void Start()
    {
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
