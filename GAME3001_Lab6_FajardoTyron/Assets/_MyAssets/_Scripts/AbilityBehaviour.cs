using TMPro;
using UnityEngine;

public abstract class AbilityBehaviour : MonoBehaviour
{
    
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
}
