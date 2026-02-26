using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class EnemyTargetButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI label;

    private Combatant boundEnemy;
    private Action<Combatant> onClicked;

    public void Bind(Combatant enemy, Action<Combatant> onClickedCallback)
    {
        boundEnemy = enemy;
        onClicked = onClickedCallback;

        if (label) label.text = enemy.Name;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClicked?.Invoke(boundEnemy));
    }

    public void SetInteractable(bool value)
    {
        button.interactable = value;
        gameObject.SetActive(true); // or keep always visible; your call
    }
}