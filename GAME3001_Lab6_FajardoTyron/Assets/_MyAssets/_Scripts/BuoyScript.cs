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
        dist = Vector3.Distance(target.transform.position, transform.position);
        if (!isDetecting && dist < detectionRange)
        { // Starts the Warning sequence.
            isDetecting = true;
            PlayWarning();
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
}
