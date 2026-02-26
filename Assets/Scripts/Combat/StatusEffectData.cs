using UnityEngine;

[System.Serializable]
public struct StatModifier
{
    public StatType stat;
    public int flatDelta; // e.g., -2 Perception
    public int effectiveFlatDelta;
}

[CreateAssetMenu(menuName = "Combat/Status Effect")]
public class StatusEffectData : ScriptableObject
{
    public string statusId;        // "POISON"
    public string displayName;     // "Poisoned"

    public int baseDurationTurns = 3;
    public bool stackable = false;
    public int maxStacks = 1;

    public int damagePerTurn = 0;  // DoT applied at end of afflicted unit’s turn
    public StatModifier[] statModifiers;
}
