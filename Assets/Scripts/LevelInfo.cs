using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Level/LevelInfo")]
public class LevelInfo : ScriptableObject
{
    [SerializeField] private int levelNumber = -1;
    [SerializeField] private string levelName = "";
    [SerializeField] private Sprite sigil;

    public int GetLevelNumber()
    {
        return levelNumber;
    }

    public string GetLevelName()
    {
        return levelName;
    }

    public Sprite GetSigil()
    {
        return sigil;
    }
}
