using UnityEngine;

public class EnemyBaseScript : MonoBehaviour
{
    public int hitPoints = 3; // Change for each enemy type
    public GameObject explosionPrefab;
    public AudioClip explosionSound;

    [SerializeField] private GameObject missilePrefab;

    private Transform spawnPoint;

    void Start()
    {
        spawnPoint = transform.GetChild(1);
        // SpawnMissile();
    }

    public void SpawnMissile()
    {
        GameObject.Instantiate(missilePrefab, spawnPoint.position, spawnPoint.rotation);
    }
    public void TakeDamage(int damage)
    {
        hitPoints -= damage;
        if (hitPoints <= 0)
        {
            if (explosionPrefab) Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            if (explosionSound) AudioSource.PlayClipAtPoint(explosionSound, transform.position);
            Destroy(gameObject);
        }
    }

}
