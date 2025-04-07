using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    private Camera maincamera;
    private new Rigidbody rigidbody;

    private Vector3 velocity;

    public float moveSpeed = 8f;

    private void Awake()
    {
        maincamera = Camera.main;
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Movement();
    }

    private void Movement()
    {
        float inputAxisX = Input.GetAxisRaw("Horizontal");
        float inputAxisZ = Input.GetAxisRaw("Vertical");

        if (inputAxisX != 0 || inputAxisZ != 0)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, inputAxisX * moveSpeed, moveSpeed * Time.deltaTime);
            velocity.z = Mathf.MoveTowards(velocity.z, inputAxisZ * moveSpeed, moveSpeed * Time.deltaTime);
        }
        else
        {
            velocity = Vector3.zero; 
        }
        

    }

    private void FixedUpdate()
    {
        rigidbody.linearVelocity = new Vector3(velocity.x, rigidbody.linearVelocity.y, velocity.z);

        Vector3 position = rigidbody.position;
       
        rigidbody.MovePosition(position);
    }
}
