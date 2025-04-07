using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    private Transform player;
    public float cameraz = -7f;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    private void LateUpdate()
    {
        
        Vector3 cameraPosition = transform.position;
        cameraPosition.x = player.position.x;
        cameraPosition.z = player.position.z + cameraz;
        cameraPosition.y = Mathf.Max(cameraPosition.y, 26f);
        transform.position = cameraPosition;
    }
}
