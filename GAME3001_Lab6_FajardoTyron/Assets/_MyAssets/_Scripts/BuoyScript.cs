using System.Collections;
using UnityEngine;

public class BuoyScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public GameObject enemyBase;
    [SerializeField] public GameObject target;

    [Header("Detection Fields")]
    [SerializeField] private float detectionRange; // Outer range. Max
    [SerializeField] private float launchRange; // Inner range. Min.
    [SerializeField] private float minRepeat;
    [SerializeField] private float maxRepeat;

    [Header("Misc. Fields")]
    [SerializeField] private float missileCooldown = 3f;

    public int hitPoints = 3; // Change for each enemy type
    public GameObject explosionPrefab;
    public AudioClip explosionSound;
    public float empDisableDuration = 3f; // How long the enemy is frozen
    private bool isDisabled = true;
    private bool playWarning = false; // Used to control the warning sound.

    private bool isDetecting = false;
    private bool canLaunch = true;
    private float dist; // Holds result of distance check.

    private void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        enemyBase = GameObject.Find("EnemyBaseParent");
    }

    void Update()
    {
        if (isDisabled)
        {
            if (!playWarning)
            {
                PlayWarning();
                playWarning = true;
            }
            return;
        }
        if (playWarning)
        {
            PlayWarning();
            playWarning = false;
        }


        
   


        dist = Vector3.Distance(target.transform.position, transform.position);
        if (!isDetecting && dist < detectionRange)
        { // Starts the Warning sequence.
            isDetecting = true;
            
        }
        if (isDetecting && dist > detectionRange)
        {
            isDetecting = false;
        }
        if (isDetecting && canLaunch && dist <= launchRange)
        { // Do launch sequence.
            canLaunch = false;
            enemyBase.GetComponent<EnemyBaseScript>().SpawnMissile();
            Invoke("MissileCooldown", missileCooldown);
        }
    }

    private void PlayWarning() // Sonar ping.
    {
        Game.Instance.SOMA.PlaySound("Pang");
        if (dist < detectionRange) // Player is still within detection range.
        {
            float t = Mathf.Clamp01((dist - launchRange) / (detectionRange - launchRange));
            // Debug.Log($"Dist: {dist} t: {t}");
            Invoke("PlayWarning", Mathf.Lerp(minRepeat, maxRepeat, t));
        }
    }

    private void MissileCooldown()
    {
        canLaunch = true;
    }
    public void TakeDamage(int damage)
    {
        hitPoints -= damage;
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
}
  
