using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed = 2f;
    private Transform target;

    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
    }
    private void Update()
    {
        if (target == null) return;

        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0;

        transform.position += direction * speed * Time.deltaTime;
    }


}