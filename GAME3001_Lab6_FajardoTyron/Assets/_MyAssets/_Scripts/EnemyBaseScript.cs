using UnityEngine;
using System.Collections;
using static InterfaceScript; // Assuming this is where IDamageable is defined

public class EnemyBaseScript : MonoBehaviour
{
    
    public int hitPoints = 3; // Change for each enemy type
    public GameObject explosionPrefab;
    public AudioClip explosionSound;
    public float empDisableDuration = 3f; // How long the enemy is frozen
    private bool isDisabled = false;


    [SerializeField] private GameObject missilePrefab;

    private Transform spawnPoint;

    void Start()
    {
        spawnPoint = transform.GetChild(1);
        // SpawnMissile();
    }

    void Update()
    {
        if (isDisabled)
            SpawnMissile(); // Optional: spawn missile if disabled, or handle differently

        return;
        // Enemy behavior logic here, e.g., movement, attacking, etc.
    }

    public void SpawnMissile()
    {
        GameObject.Instantiate(missilePrefab, spawnPoint.position, spawnPoint.rotation);
    }
    public void TakeDamage(int damage)
    {
        IDamageable damageable = GetComponent<IDamageable>();
        hitPoints -= damage; 
        Debug.Log($"Enemy hit points: {hitPoints}");
        if (hitPoints <= 0)
        {

            Destroy(gameObject);
        }
    }
    public void ApplyEMP(float duration)
    {
        if (!isDisabled)
            StartCoroutine(EMPDisableCoroutine(duration));
    }

    private IEnumerator EMPDisableCoroutine(float duration)
    {
        isDisabled = true;

        // Optional: visual/audio effect for EMP hit
        yield return new WaitForSeconds(duration);

        isDisabled = false;
    }
    private void OnCollision2D(Collider2D other)
    {
        TakeDamage(1); // Assuming 1 damage per hit, adjust as needed
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(1);
        }
    }
}
