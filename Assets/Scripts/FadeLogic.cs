using System.Collections;
using System.Collections.Generic;
using Ink.Parsed;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeLogic : MonoBehaviour
{
    public static FadeLogic Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(gameObject);
            return;
        }
    }
    

    public IEnumerator FadeAndLoad(List<Canvas> canvasList, float fadeDuration, float endAlpha)
    {
        foreach (Canvas canvas in canvasList)
        {
            StartCoroutine(FadeCanvasGroup(canvas, fadeDuration, endAlpha));
        }

        yield return new WaitForSeconds(fadeDuration);

        // Load the next scene after elements have faded.
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    public IEnumerator Fade(List<Canvas> canvasList, float fadeDuration, float endAlpha)
    {
        foreach (Canvas canvas in canvasList)
        {
            StartCoroutine(FadeCanvasGroup(canvas, fadeDuration, endAlpha));
        }

        yield return new WaitForSeconds(fadeDuration);
    }

    public IEnumerator FadeCanvasGroup(Canvas canvas, float fadeDuration, float endAlpha)
    {
        float elapsed = 0f;
        CanvasGroup cgroup = canvas.GetComponent<CanvasGroup>();

        float startAlpha = cgroup.alpha;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            cgroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            yield return null;
        }

        cgroup.alpha = endAlpha;
    }
}
