using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed = 2f;
    public float stopDistance = 1.5f;
    public float attackRange = 1.2f;
    public float attackCooldown = 2f;

    private float lastAttackTime;
    private Transform target;

    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            target = player.transform;
    }

    private void Update()
    {
        if (target == null) return;

        Vector3 direction = target.position - transform.position;
        direction.y = 0f;
        float distance = direction.magnitude;

        if (distance > stopDistance)
        {
            // Move
            Vector3 moveDir = direction.normalized;
            transform.position += moveDir * speed * Time.deltaTime;

         
        }
        else if (distance <= attackRange && Time.time > lastAttackTime + attackCooldown)
        {
            // Atacar
            Debug.Log("Enemy atacou o player!");
            lastAttackTime = Time.time;
        }
    }
}