using System.Collections.Generic;
using System.Linq;
using Ink.Parsed;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButtonInit : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform container;
    private List<LevelInfo> levelInfo = new();

    private void Awake()
    {
        levelInfo = Resources.LoadAll<LevelInfo>("Levels").ToList();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AddLevelButtons();
    }

    void AddLevelButtons()
    {
        foreach (var level in levelInfo)
        {
            var btn = Instantiate(buttonPrefab, container);
            var levelButtonUI = btn.GetComponent<LevelButtonUI>();
            levelButtonUI.SetUp(level);

            btn.GetComponent<Button>().onClick.AddListener(() => LoadLevel(level.GetLevelNumber()));
        }
    }

    private void LoadLevel(int levelNumber)
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + levelNumber);
    }
}
