using UnityEngine;
using UnityEngine.UI;

public class PlayerStatus : MonoBehaviour
{
    public Slider slider;
    public int MaxHealth = 100;
    public int currentHealth;

    public Gradient gradient;
    public Image fill;

    // Para animação suave da barra
    private float displayedHealth;
    public float smoothSpeed = 5f;

    private bool isDead = false;

    private void Start()
    {
        currentHealth = MaxHealth;
        displayedHealth = MaxHealth;
        SetMaxHealth(MaxHealth);
    }

    private void Update()
    {
        // Interpola o valor mostrado para o valor atual, suavizando a barra
        displayedHealth = Mathf.Lerp(displayedHealth, currentHealth, Time.deltaTime * smoothSpeed);
        slider.value = displayedHealth;

        if (fill != null)
            fill.color = gradient.Evaluate(slider.normalizedValue);

        // Verifica se o player morreu
        if (!isDead && currentHealth <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(currentHealth - damage, 0);
        // Não chama SetHealth direto, a barra vai animar no Update
    }

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        displayedHealth = health;

        if (fill != null)
            fill.color = gradient.Evaluate(1f);
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("Player morreu!");

        // Tenta avisar o objeto pai (Player) para morrer
        Player player = GetComponentInParent<Player>();
        if (player != null)
        {
            player.Die();  // Chama método Die do Player
        }
        else
        {
            // Caso não tenha Player pai, só desativa a barra mesmo
            gameObject.SetActive(false);
        }
    }

}