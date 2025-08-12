using UnityEngine;

public class EnemyBaseScript : MonoBehaviour
{
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
}
