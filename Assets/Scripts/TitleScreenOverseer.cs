using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenOverseer : MonoBehaviour
{
    [SerializeField] private List<Canvas> canvasList;

    private bool isFading = false;
    public void OnButtonPress()
    {
        if (isFading) return; // Spam prevention
        isFading = true;
        StartCoroutine(FadeLogic.Instance.FadeAndLoad(canvasList, 2f, 0f));
    }
}
