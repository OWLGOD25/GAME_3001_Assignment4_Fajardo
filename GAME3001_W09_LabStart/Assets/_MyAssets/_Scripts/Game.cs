using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public static Game Instance { get; private set; } // Static object of the class.
    public SoundManager SOMA;
    [Header("Object References")]
    [SerializeField] private GameObject buoyPrefab;
    [SerializeField] int numberOfBuoys;

    private List<GameObject> buoyList;

    // TODO: Add a list of buoys for detection mechanic.

    private void Awake() // Ensure there is only one instance.
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Will persist between scenes.
            Initialize();
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances.
        }
    }

    private void Initialize()
    {
        SOMA = new SoundManager();
        SOMA.Initialize(gameObject);
        SOMA.AddSound("Pang", Resources.Load<AudioClip>("Pang"), SoundManager.SoundType.SOUND_SFX);
        SOMA.AddSound("Torpedo", Resources.Load<AudioClip>("Torpedo"), SoundManager.SoundType.SOUND_SFX);
        SOMA.AddSound("Jet2Holiday", Resources.Load<AudioClip>("Jet2Holiday"), SoundManager.SoundType.SOUND_MUSIC);
        // Spawn the enemy buoys.
        buoyList = new List<GameObject>();
        for (int i = 0; i < numberOfBuoys; i++)
        {
            GameObject buoyInst = GameObject.Instantiate(buoyPrefab, 
                new Vector3(Random.Range(-2f, 4f), Random.Range(-5f, 5f), 0f), Quaternion.identity);
            buoyList.Add(buoyInst);
        }
        SOMA.PlayMusic("Jet2Holiday");
    }

    public float GetClosestBuoyRange(Vector3 shipLoc)
    {
        if (buoyList.Count == 0) return -1f; // Temporary error value returned.
        float minDist = float.MaxValue;
        for (int i = 0; i < buoyList.Count; i++) // This can also be a foreach.
        {
            float newDist = Vector3.Distance(shipLoc, buoyList[i].transform.position);
            if (newDist < minDist) minDist = newDist;
        }
        return minDist;
    }
}
