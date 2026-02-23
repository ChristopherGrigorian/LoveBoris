using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelNumber;
    [SerializeField] private TextMeshProUGUI levelName;
    [SerializeField] private Image sigil;
    
    public void SetUp(LevelInfo level)
    {
        levelNumber.text = "Level " + level.GetLevelNumber().ToString();
        levelName.text = level.GetLevelName();
        sigil.sprite = level.GetSigil();
    }
}
