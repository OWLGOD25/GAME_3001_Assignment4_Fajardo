using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FogScript : MonoBehaviour
{
    [Header("Fog Sprite Reference")]
    [SerializeField] private SpriteRenderer fogSprite;

    [Header("Defogging Fields")]
    [Tooltip("Total duration in seconds for a defogging.")]
    [SerializeField] private float fadeDuration;

    [Tooltip("Alpha when fade operation completes.")]
    [SerializeField][Range(0f, 1f)]private float minAlpha;

    [Tooltip("A reference to our Coroutine.")]
    Coroutine fadeRoutine;

    void Awake()
    {
        if (fogSprite == null)
        {
            fogSprite = GetComponentInChildren<SpriteRenderer>();
            fogSprite.color = new Color(0.75f, Random.Range(0.45f, 0.65f), 1f);
        }
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            BeginFade();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StopFade();
        }
    }

    private void BeginFade()
    {
        StopFade(); // Stops a duplicate fade from running.
        fadeRoutine = StartCoroutine(FadeOut());
    }

    private void StopFade()
    {
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
            fadeRoutine = null;
        }
    }

    IEnumerator FadeOut()
    {
        Debug.Log("Starting Coroutine...");
        float currentAlpha = fogSprite.color.a;
        float fadeAmount = Mathf.Abs(currentAlpha - minAlpha) / fadeDuration; 

        if (fadeAmount <= 0f) // Wouldn't need this if we turn off trigger.
        {
            fadeRoutine = null;
            yield break;
        }

        while (currentAlpha > minAlpha)
        {
            currentAlpha = Mathf.MoveTowards(currentAlpha, minAlpha, fadeAmount * Time.deltaTime);

            var col = fogSprite.color;
            col.a = currentAlpha;
            fogSprite.color = col;

            fadeDuration -= Time.deltaTime;
            if (fadeDuration < 0f) fadeDuration = 0f;

            yield return null;
        }

        fadeRoutine = null;
        // Optionally we can now turn off trigger if you don't want patch to return.
    }
}
