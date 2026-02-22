using System.Collections;
using TMPro;
using UnityEngine;

public class TextDots : MonoBehaviour
{
    private TextMeshProUGUI tmp;
    private string textString;

    private Coroutine textDotCycle;

    private float waitTime = 1f;

    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        textString = tmp.text;
        if (textDotCycle == null)
        {
            textDotCycle = StartCoroutine(TextDotCycle());
        }
    }

    IEnumerator TextDotCycle()
    {
        while (true)
        {
            tmp.text = textString;
            yield return new WaitForSeconds(waitTime);

            tmp.text += ".";
            yield return new WaitForSeconds(waitTime);

            tmp.text += ".";
            yield return new WaitForSeconds(waitTime);
        }
    } 
}
