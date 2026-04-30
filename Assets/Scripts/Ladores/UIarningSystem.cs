using UnityEngine;
using System.Collections;

public class UIWarningSystem : MonoBehaviour
{
    public static UIWarningSystem Instance;

    [Header("Warning Settings")]
    public GameObject warningPanel;
    public float warningDisplayTime = 2f;
    public float flashSpeed = 0.1f;

    private CanvasGroup canvasGroup;

    void Awake()
    {
        // Singleton pattern for easy access
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (warningPanel != null)
        {
            canvasGroup = warningPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = warningPanel.AddComponent<CanvasGroup>();
            }
            warningPanel.SetActive(false);
        }
    }

    public void ShowMeteorWarning()
    {
        if (warningPanel != null)
        {
            StopAllCoroutines();
            StartCoroutine(DisplayWarningWithFlash());
        }
    }

    IEnumerator DisplayWarningWithFlash()
    {
        warningPanel.SetActive(true);

        // Flash the warning several times
        float startTime = Time.time;

        while (Time.time - startTime < warningDisplayTime)
        {
            // Calculate flash: on for 0.2s, off for 0.1s
            float flashTimer = Mathf.PingPong(Time.time, flashSpeed * 2);
            bool isVisible = flashTimer < flashSpeed;

            if (canvasGroup != null)
            {
                canvasGroup.alpha = isVisible ? 1f : 0.3f;
            }
            else
            {
                warningPanel.SetActive(isVisible);
            }

            yield return null;
        }

        warningPanel.SetActive(false);

        // Reset alpha if using canvas group
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }
    }
}