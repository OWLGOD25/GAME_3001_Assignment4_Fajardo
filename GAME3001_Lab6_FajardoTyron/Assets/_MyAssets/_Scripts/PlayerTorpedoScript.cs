using UnityEngine;

public class PlayerTorpedoScript : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float lifeTime;

    private Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // In case you ever need to zero out the velocities:
        rb.angularVelocity = 0f;
        rb.linearVelocity = Vector3.zero;

        // Apply new velocity based on projectile's forward vector specified below.
        rb.linearVelocity = transform.right * moveSpeed * Time.deltaTime;

        Destroy(gameObject, lifeTime);
    }


    
}
