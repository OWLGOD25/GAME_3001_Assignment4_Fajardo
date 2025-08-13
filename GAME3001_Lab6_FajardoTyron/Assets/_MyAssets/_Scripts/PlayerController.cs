using TMPro;
using UnityEngine;
using static ProjectileAbility;
using static EMPAbility;
using static ShieldAbility;

public class PlayerController : MonoBehaviour
{
    [Header("ShieldAbility")]
    [SerializeField] public GameObject shieldPrefab;
    public float shieldDuration = 3f;
    public float shieldCooldown = 5f;
    public bool shieldActive = false;
    public bool shieldOnCooldown = false;


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

    private ShieldAbility sa;
    private MovementAbility ma;
    private DetectAbility da;
    private ProjectileAbility pa;
    private EMPAbility ea;

    private void Start()
    {

        // Initialize the abilities
        sa = gameObject.AddComponent<ShieldAbility>();
        sa.shieldPrefab = shieldPrefab;
        sa.shieldDuration = shieldDuration;
        sa.shieldCooldown = shieldCooldown;
     
      

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
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !shieldOnCooldown)
        {
            StartCoroutine(sa.ActivateShield());
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(ea.TriggerEMP());
        }

    }
}
