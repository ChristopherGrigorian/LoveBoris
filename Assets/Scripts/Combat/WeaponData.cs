using UnityEngine;
using System.Collections.Generic;

public enum WeaponType { Gun, Sword, Tome }

[CreateAssetMenu(menuName = "Combat/Weapon")]

public class WeaponData : ScriptableObject
{
    public string id;
    public string weaponName;

    [TextArea(3, 10)]
    public string weaponDescription;

    public WeaponType weaponType;
    public List<SkillData> weaponSkills;

    [Header("Power")]
    public int basePower = 10;
    public int powerPerLevel = 5;
    public int maxLevel = 10;

    /*
    public int GrabWeaponPower()
    {
        return InventoryManager.Instance.GetWeaponPower(this);
    }
    */
}

public struct WeaponProgress
{
    public int level;

    public WeaponProgress(int lvl)
    {
        level = lvl;
    }
}
