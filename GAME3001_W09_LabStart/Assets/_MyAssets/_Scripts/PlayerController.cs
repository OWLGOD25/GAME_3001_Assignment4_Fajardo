using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Fields")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed;

    [Header("Detect Fields")]
    [SerializeField] private float detectReset;

    [Header("Projectile Fields")]
    public GameObject torpedoPrefab;
    public int maxAmmo;
    public float fireDelay;
    public float reloadTime;

    private MovementAbility ma;
    private DetectAbility da;
    private ProjectileAbility pa;

    private void Start()
    {
        ma = gameObject.AddComponent<MovementAbility>();
        ma.moveSpeed = moveSpeed;
        ma.rotationSpeed = rotationSpeed;

        da = gameObject.AddComponent<DetectAbility>();
        da.detectReset = detectReset;

        pa = gameObject.AddComponent<ProjectileAbility>();
        pa.torpedoPrefab = torpedoPrefab;
        pa.maxAmmo = maxAmmo;
        pa.fireDelay = fireDelay;
        pa.reloadTime = reloadTime;
    }
}
