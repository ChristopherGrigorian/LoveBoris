using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{

    private List<Character> characters;

    public static CharacterManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        characters = Resources.LoadAll<Character>("Characters").ToList();
    }

    public List<Character> grabCharacters()
    {
        return characters;
    }
}