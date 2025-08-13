using TMPro;
using UnityEngine;
using System.Collections;
public abstract class AbilityBehaviour : MonoBehaviour
{

}

public class ShieldAbility : AbilityBehaviour
{
    [Header("ShieldAbility")]
    [SerializeField] public GameObject shieldPrefab;
    public float shieldDuration = 3f;
    public float shieldCooldown = 5f;
    public bool shieldActive = false;
    public bool shieldOnCooldown = false;

    public IEnumerator ActivateShield()
    {
        shieldOnCooldown = true;
        shieldActive = true;

        // Enable the shield visual
        if (shieldPrefab != null)
            shieldPrefab.SetActive(true);

        // Wait for shield duration
        yield return new WaitForSeconds(shieldDuration);

        shieldActive = false;

        // Disable the shield visual
        if (shieldPrefab != null)
            shieldPrefab.SetActive(false);

        // Wait for cooldown before allowing reuse
        yield return new WaitForSeconds(shieldCooldown);
        shieldOnCooldown = false;
    }

}

public class EMPAbility : AbilityBehaviour
{
    public float empRadius = 5f;
    public float empCooldown = 10f;
    public float empDuration = 3f; // Duration for which electronics are disabled
    public GameObject empEffectPrefab;

    private bool onCooldown = false;

    public System.Action DisableElectronics;

    public string TriggerEMP()
    {
        if (onCooldown)
            StartCoroutine(DoEMP());

        return "EMP Triggered";
    }

    private IEnumerator DoEMP()
    {
        // Show EMP effect
        if (empEffectPrefab != null)
            Instantiate(empEffectPrefab, transform.position, Quaternion.identity);

        // Disable detectors within radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, empRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("EnemyDetector"))
            {
                var detector = hit.GetComponent<EnemyDetector>();
                if (detector != null)
                    detector.DisableForSeconds(3f); // Jam time, can be exposed
            }

        }

        DisableElectronics?.Invoke();

        onCooldown = true;
        yield return new WaitForSeconds(empCooldown);
        onCooldown = false;

    }
}

public class MovementAbility : AbilityBehaviour
{
    [Header("Movement Fields")]
    public float moveSpeed;
    public float rotationSpeed;

    private void Update()
    {
        float moveInput = Input.GetAxis("Vertical"); // W/S input.
        float rotInput = Input.GetAxis("Horizontal"); // A/D input.

        transform.Translate(Vector3.right * moveInput * moveSpeed * Time.deltaTime, Space.Self);
        transform.Rotate(Vector3.forward, -rotInput * rotationSpeed * Time.deltaTime);
    }
}

public class DetectAbility : AbilityBehaviour
{
    [Header("Detect Fields")]
    public float detectReset;

    private TMP_Text detectLabel;
    private bool isDetecting;
    private bool canDetect;

    private void Start()
    {
        detectLabel = GameObject.Find("Detect Label").GetComponent<TMP_Text>();
        detectLabel.text = "Closest Buoy: Unknown";
        canDetect = true;
        isDetecting = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canDetect && !isDetecting)
        {
            canDetect = false;
            isDetecting = true;
            Invoke("DetectCooldown", detectReset);
        }

        if (isDetecting)
        {
            detectLabel.text = "Closest Buoy: " + Game.Instance.GetClosestBuoyRange(transform.position);
        }
    }

    private void DetectCooldown()
    {
        canDetect = true;
        isDetecting = false;
        detectLabel.text = "Closest Buoy: Unknown";
    }
}

public class ProjectileAbility : AbilityBehaviour
{
    [Header("Projectile Fields")]
    public GameObject torpedoPrefab;
    public int maxAmmo;
    public float fireDelay;
    public float reloadTime;

    private TMP_Text ammoLabel;
    private int ammo;
    private bool canFire;




    private void Start()
    {
        ammoLabel = GameObject.Find("Ammo Label").GetComponent<TMP_Text>();
        ammoLabel.text = "Ammo: " + ammo;
        ammo = maxAmmo;
        canFire = true;

        InvokeRepeating("Reload", 0, reloadTime);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canFire && ammo > 0) // Projectile component.
        {
            GameObject torpInst = Instantiate(torpedoPrefab, transform.position, transform.rotation);
            Physics2D.IgnoreCollision(torpInst.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true);
            Game.Instance.SOMA.PlaySound("Torpedo");
            canFire = false;
            ammoLabel.text = "Ammo: " + --ammo;
            Invoke("FireCooldown", fireDelay);
        }
    }
    private void Reload()
    {
        if (ammo < maxAmmo)
        {
            ammo++;
            ammoLabel.text = "Ammo: " + ammo;
        }
    }

    private void FireCooldown()
    {
        canFire = true;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<SmartMissile>()?.TakeDamage(1);
            collision.GetComponent<BuoyScript>()?.TakeDamage(1);
            collision.GetComponent<EnemyBaseScript>()?.TakeDamage(1);
            Destroy(gameObject);
        }
    }
}
