using UnityEngine;

public class Bill : MonoBehaviour
{
    private Quaternion initialRotation;

    private void Start()
    {
        initialRotation = transform.rotation; 
    }

    private void LateUpdate()
    {
        transform.rotation = initialRotation; 
    }
}
