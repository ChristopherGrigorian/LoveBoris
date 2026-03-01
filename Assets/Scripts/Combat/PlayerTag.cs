using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerTag : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private TextMeshProUGUI health;
    [SerializeField] private TextMeshProUGUI mp;

    private Combatant boundEnemy;

    public void Bind(Combatant player)
    {
        
        name.text = player.data.name;
        health.text = "Health: " + player.data.maxHP.ToString();
        mp.text = "MP: " + player.data.maxMP.ToString();
    }

    public void UpdateTag(Combatant player)
    {
        health.text = "Health: " + player.EffectiveMaxHP.ToString();
        mp.text = "MP: " + player.EffectiveMaxMP.ToString();
    }
}