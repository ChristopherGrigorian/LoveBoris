using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum StatType { Strength, Perception, Evasiveness, Spirit, Speed, MaxHP, MaxMP }

[System.Serializable]
public class ActiveStatus
{
    public StatusEffectData data;
    public int remainingTurns;
    public int stacks;
    public bool skipFirstTick;

    public ActiveStatus(StatusEffectData d, int duration, int stacks = 1)
    {
        data = d;
        remainingTurns = duration;
        this.stacks = stacks;
        skipFirstTick = false;
    }
}


public class Combatant
{
    public CharacterData data;
    public int currentHP;
    public int currentMP = 1;
    public bool isPlayerControlled;

    public WeaponData equippedWeapon;
    public List<SkillData> unlockedSkills = new();

    public List<ActiveStatus> statuses = new();

    private PlayerTag playerTag;

    public Combatant(CharacterData data, bool isPlayer)
    {
        this.data = data;
        this.currentHP = data.maxHP;
        this.isPlayerControlled = isPlayer;

        equippedWeapon = data.EquippedWeapon;
        unlockedSkills.AddRange(data.Skills);
    }

    public List<SkillData> GetAvailableSkills()
    {
        var skills = new List<SkillData>();
        if (equippedWeapon != null)
        {
            skills.AddRange(equippedWeapon.weaponSkills);
        }
        skills.AddRange(unlockedSkills);
        return skills;
    }

    public string Name => data.characterName;
    public bool IsAlive => currentHP > 0;

    public bool HasStatus(string statusId) =>
        statuses.Any(s => s.data.statusId == statusId && s.remainingTurns > 0);

    public void ApplyStatus(StatusEffectData effect, int durationOverride = -1, int stacksToAdd = 1)
    {
        if (effect == null) return;
        int dur = durationOverride > 0 ? durationOverride : effect.baseDurationTurns;

        var existing = statuses.FirstOrDefault(s => s.data.statusId == effect.statusId);
        if (existing != null)
        {
            if (existing.data.stackable)
            {
                existing.stacks = Mathf.Min(existing.stacks + stacksToAdd, existing.data.maxStacks);
            }
            else
            {
                existing.stacks = 1;
            }
            existing.remainingTurns = Mathf.Max(existing.remainingTurns, dur); // refresh duration
        }
        else
        {
            statuses.Add(new ActiveStatus(effect, dur, effect.stackable ? stacksToAdd : 1));
        }
    }

    public void AssignPlayerTag(PlayerTag tag) 
    {
        playerTag = tag;
    }

    // ---------- EFFECTIVE STATS (base + modifiers from active statuses) ----------
    private int GetStat(StatType type, int baseValue)
    {
        int delta = 0;
        foreach (var s in statuses)
        {
            if (s.remainingTurns <= 0) continue;
            foreach (var mod in s.data.statModifiers)
            {
                if (mod.stat == type) delta += mod.effectiveFlatDelta * Mathf.Max(1, s.stacks);
            }
        }
        return Mathf.Max(0, baseValue + delta);
    }

    public int EffectivePerception => GetStat(StatType.Perception, data.perception);
    public int EffectiveEvasiveness => GetStat(StatType.Evasiveness, data.evasiveness);
    public int EffectiveSpirit => GetStat(StatType.Spirit, data.spirit);
    public int EffectiveSpeed => GetStat(StatType.Speed, data.speed);
    public int EffectiveStrength => GetStat(StatType.Strength, data.strength);
    public int EffectiveMaxHP => GetStat(StatType.MaxHP, data.maxHP);
    public int EffectiveMaxMP => GetStat(StatType.MaxMP, data.maxMP);

}
