using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuOverseer : MonoBehaviour
{
    [SerializeField] private List<Canvas> canvasList;

    private void Start()
    {
        StartCoroutine(FadeLogic.Instance.Fade(canvasList, 6f, 1f));
    }
}
