using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Combat/Encounter")]
public class EncounterData : ScriptableObject
{
    public string encounterID;
    public List<CharacterData> enemies;
}
