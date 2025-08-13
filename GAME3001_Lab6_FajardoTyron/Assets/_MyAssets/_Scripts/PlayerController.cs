using TMPro;
using UnityEngine;
using static ProjectileAbility;

public class PlayerController : MonoBehaviour
{
    [Header("ShieldAbility")]
    [SerializeField] private GameObject shieldPrefab; 
    public float shieldDuration = 3f;
    public float shieldCooldown = 5f;
    private bool shieldActive = false;
    private bool shieldOnCooldown = false;


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

    [Header("EMP Fields")]
    [SerializeField] private GameObject empEffectPrefab;
    public float empRadius = 5f;
    public float empCooldown = 10f;
    

    private MovementAbility ma;
    private DetectAbility da;
    private ProjectileAbility pa;
    private EMPAbility ea;

    private void Start()    
    {
        ea = gameObject.AddComponent<EMPAbility>();
        ea.empRadius = empRadius;
        ea.empCooldown = empCooldown;
        ea.empEffectPrefab = empEffectPrefab;
        ea.DisableElectronics = () => { ea.TriggerEMP(); };

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
