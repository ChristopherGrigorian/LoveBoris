using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenOverseer : MonoBehaviour
{
    [SerializeField] private List<Canvas> canvasList;

    private float fadeDuration = 4f;
    private bool isFading = false;
    public void OnButtonPress()
    {
        if (isFading) return; // Spam prevention
        StartCoroutine(FadeAndLoad());
    }

    private IEnumerator FadeAndLoad()
    {
        isFading = true;

        foreach (Canvas canvas in canvasList)
        {
            StartCoroutine(FadeCanvasGroup(canvas));
        }

        yield return new WaitForSeconds(fadeDuration);

        // Load the next scene after elements have faded.
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);

    }
    private IEnumerator FadeCanvasGroup(Canvas canvas)
    {
        float elapsed = 0f;
        CanvasGroup cgroup = canvas.GetComponent<CanvasGroup>();

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            float startAlpha = cgroup.alpha;

            cgroup.alpha = Mathf.Lerp(startAlpha, 0f, t);
            yield return null;
        }

        cgroup.alpha = 0f;
    }
}
