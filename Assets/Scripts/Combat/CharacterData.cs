using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Combat/Character")]
public class CharacterData : ScriptableObject
{
    [Header("Identification")]
    public string id;
    public string characterName;
    public bool currentlyEquipped = false;

    [Header("Statline")]
    public int level = 1;
    public int experience;
    public int maxHP;
    public int maxMP;
    public int strength;
    public int perception;
    public int evasiveness;
    public int spirit;
    public int speed;

    [Header("Equipment")]
    public WeaponData EquippedWeapon;
    public List<WeaponType> equipableWeaponTypes;
    public List<SkillData> Skills;

    public void GainExperience(int amount)
    {
        experience += amount;
        while (experience >= GetRequiredXPForLevel(level + 1))
        {
            experience -= GetRequiredXPForLevel(level + 1);
            LevelUp();
        }
    }

    private void LevelUp()
    {
        level++;
        maxHP += Mathf.RoundToInt(10 + level * 2f);
        strength += Mathf.RoundToInt(1 + level * 0.5f);
        perception += Mathf.RoundToInt(1 + level * 0.3f);
        evasiveness += Mathf.RoundToInt(1 + level * 0.3f);
        spirit += Mathf.RoundToInt(1 + level * 0.5f);
        speed += Mathf.RoundToInt(1 + level * 0.4f);
        Debug.Log($"{characterName} leveled up to {level}!");
    }

    public int GetRequiredXPForLevel(int targetLevel)
    {
        return Mathf.FloorToInt(100 * Mathf.Pow(targetLevel, 1.5f));
    }

    public int XPNeededToNextLevel()
    {
        int nextLevelCost = GetRequiredXPForLevel(level + 1);
        return Mathf.Max(0, nextLevelCost - experience);
    }
}
