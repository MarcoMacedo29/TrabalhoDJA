using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Configurações de Vida")]
    [Tooltip("Quantidade máxima de corações (por ex. 10)")]
    public int maxHealth = 10;

    [Header("Referências aos SpriteRenderers (Draw Mode = Tiled)")]
    [Tooltip("SpriteRenderer que mostra os corações cinza (vazios)")]
    public SpriteRenderer emptyHeartsRenderer;
    [Tooltip("SpriteRenderer que mostra os corações vermelhos (cheios)")]
    public SpriteRenderer fullHeartsRenderer;

    private int currentHealth;

    private float tileWidth;  // largura de um coração em unidades de mundo
    private float tileHeight; // altura de um coração em unidades de mundo

    void Start()
    {
        // 1) Inicializa vida cheia
        currentHealth = maxHealth;

        // 2) Validações rápidas
        if (emptyHeartsRenderer == null || fullHeartsRenderer == null)
        {
            Debug.LogError("EnemyHealth: arraste os SpriteRenderers (empty + full) no Inspector!");
            enabled = false;
            return;
        }

        // 3) Garanta que DrawMode esteja configurado para Tiled
        emptyHeartsRenderer.drawMode = SpriteDrawMode.Tiled;
        fullHeartsRenderer.drawMode = SpriteDrawMode.Tiled;

        // 4) Calcule tileWidth/tileHeight a partir do sprite:
        //    (assume-se que o import PPU = 32, então cada coração é 1×1 unidade)
        tileWidth = emptyHeartsRenderer.sprite.bounds.size.x;
        tileHeight = emptyHeartsRenderer.sprite.bounds.size.y;

        // 5) Configure ambos os renderers para mostrar exatemente `maxHealth` tiles:
        Vector2 totalSize = new Vector2(maxHealth * tileWidth, tileHeight);
        emptyHeartsRenderer.size = totalSize;
        fullHeartsRenderer.size = totalSize;
    }

    /// <summary>
    /// Chame isto quando você quer dar dano ao inimigo.
    /// </summary>
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHeartsVisual();

        Debug.Log($"Inimigo {gameObject.name} recebeu {damageAmount} de dano. Vida atual: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Ajusta fullHeartsRenderer.size.x para mostrar apenas `currentHealth` corações vermelhos.
    /// </summary>
    private void UpdateHeartsVisual()
    {
        // Se um coração = tileWidth de largura, queremos que o total width seja:
        float newWidth = currentHealth * tileWidth;

        // E a altura continua sendo um coração (tileHeight):
        fullHeartsRenderer.size = new Vector2(newWidth, tileHeight);
    }

    private void Die()
    {
        Debug.Log($"Inimigo {gameObject.name} morreu.");
        Destroy(gameObject);
    }

    /// <summary>
    /// Retorna 0..1 representando a porcentagem de vida atual.
    /// </summary>
    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }
}
