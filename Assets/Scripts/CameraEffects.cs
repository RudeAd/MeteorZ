using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraEffects : MonoBehaviour
{
    [Header("Screenshake Settings")]
    public float shakeDuration = 0.2f;  // Hur länge skakningen varar
    public float shakeMagnitude = 0.3f; // Hur kraftig skakningen är

    [Header("Damage Overlay")]
    public Image damageOverlay;
    public float overlayDuration = 0.2f; // Hur länge overlay visas
    private Color overlayColor;

    [Header("Fade Settings")]
    public CanvasGroup fadeCanvas;
    public float fadeDuration = 1f; // Hur snabbt fade sker

    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.position;

        // Initiera overlay
        if (damageOverlay != null)
        {
            overlayColor = damageOverlay.color;
            overlayColor.a = 0;
            damageOverlay.color = overlayColor;
        }

        // Starta med en fade in
        if (fadeCanvas != null)
        {
            fadeCanvas.alpha = 1;
            StartCoroutine(FadeIn());
        }
    }

    public void TriggerScreenShake()
    {
        StartCoroutine(ScreenShake());
    }

    public void ShowDamageOverlay()
    {
        StartCoroutine(DamageFlash());
    }

    public void FadeOutAndLoadScene(string sceneName)
    {
        StartCoroutine(FadeOut(sceneName));
    }

    private IEnumerator ScreenShake()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.position = originalPosition + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;
    }

    private IEnumerator DamageFlash()
    {
        if (damageOverlay != null)
        {
            overlayColor.a = 1;
            damageOverlay.color = overlayColor;

            yield return new WaitForSeconds(overlayDuration);

            overlayColor.a = 0;
            damageOverlay.color = overlayColor;
        }
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            fadeCanvas.alpha = Mathf.Lerp(1, 0, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        fadeCanvas.alpha = 0;
    }

    private IEnumerator FadeOut(string sceneName)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            fadeCanvas.alpha = Mathf.Lerp(0, 1, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        fadeCanvas.alpha = 1;

        // Byt scen här istället för att anropa FadeOutAndLoadScene igen
        SceneManager.LoadScene(sceneName);
    }

}
