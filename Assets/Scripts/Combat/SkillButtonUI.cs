using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SkillButtonUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI label;

    private SkillData boundSkill;
    private Action<SkillData> onChosen;

    public void Bind(SkillData skill, Action<SkillData> onChosenCallback)
    {
        boundSkill = skill;
        onChosen = onChosenCallback;

        if (label) label.text = skill.skillName;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onChosen?.Invoke(boundSkill));
    }
}