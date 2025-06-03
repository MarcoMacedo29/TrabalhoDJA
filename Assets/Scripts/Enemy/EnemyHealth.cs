using UnityEngine;
using System.Collections.Generic;

public class EnemyHealth : MonoBehaviour
{
    [Header("Configurações de Vida")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Objetos Visuais das Vidas")]
    public List<GameObject> lifeVisuals = new List<GameObject>();

    private void Start()
    {
        currentHealth = maxHealth;

        // Esconde vidas extras se o maxHealth for menor que a lista
        for (int i = 0; i < lifeVisuals.Count; i++)
        {
            lifeVisuals[i].SetActive(i < currentHealth);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(currentHealth, 0);

        UpdateLifeVisuals();

        Debug.Log($"Inimigo {gameObject.name} recebeu {damageAmount} de dano. Vida restante: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateLifeVisuals()
    {
        for (int i = 0; i < lifeVisuals.Count; i++)
        {
            lifeVisuals[i].SetActive(i < currentHealth);
        }
    }

    private void Die()
    {
        Debug.Log($"Inimigo {gameObject.name} morreu.");
        Destroy(gameObject);
    }

    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }
}