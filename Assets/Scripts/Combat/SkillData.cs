using System.Collections.Generic;
using UnityEngine;

public enum SkillType { Attack, Heal, Special, Buff, Debuff }

[System.Serializable]
public struct StatusToApply
{
    public StatusEffectData status;
    [Range(0f, 1f)] public float chance;
    public int durationOverride; // <=0 uses default
    public int stacks;           // default 1
}

[CreateAssetMenu(menuName = "Combat/Skill")]
public class SkillData : ScriptableObject
{
    public string skillName;

    [TextArea(3, 10)]
    public string skillDescription;

    public SkillType type;
    public int power;
    public int potency;
    public int MPCost;

    [Header("Targeting")]
    public bool targetsEnemies;
    public bool isAOE = false;

    [Header("Optional: statuses this skill applies")]
    public List<StatusToApply> statusesToApply;

    [Header("Audio")]
    public AudioClip castSFX;
    [Range(0f, 1f)] public float sfxVolume = 1f;
}
