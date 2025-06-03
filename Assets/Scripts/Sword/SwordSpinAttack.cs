using UnityEngine;

public class SwordSpinAttack : MonoBehaviour
{
    [Header("Referências")]
    public Transform playerTransform;

    [Header("Configurações de Velocidade")]
    public float normalSpeed = 180f;
    public float boostedSpeed = 720f;

    [Header("Tecla de Boost")]
    public KeyCode spinKey = KeyCode.Space;

    private float currentSpinSpeed;

    void Start()
    {
        if (playerTransform == null)
        {
            Debug.LogError("SwordSpinAttack: você precisa arrastar o Transform do jogador!");
            enabled = false;
            return;
        }

        currentSpinSpeed = normalSpeed;
    }

    void Update()
    {
        currentSpinSpeed = Input.GetKey(spinKey) ? boostedSpeed : normalSpeed;

        transform.RotateAround(
            playerTransform.position,
            Vector3.up,
            currentSpinSpeed * Time.deltaTime
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(1);
            Debug.Log("Acertou um inimigo!");
        }
    }
}