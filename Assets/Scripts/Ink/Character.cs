using UnityEngine;


[CreateAssetMenu(menuName = "Toolbox/Character")]
public class Character : ScriptableObject
{
    [SerializeField] public string characterName;
    [SerializeField] public AudioClip voiceBank;
}
