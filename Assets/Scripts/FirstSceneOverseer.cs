using System.Collections.Generic;
using UnityEngine;

public class FirstSceneOverseer : MonoBehaviour
{
    [SerializeField] private List<Canvas> canvasList;

    private void Start()
    {
        //StartCoroutine(FadeLogic.Instance.Fade(canvasList, 6f, 1f));
    }
}
