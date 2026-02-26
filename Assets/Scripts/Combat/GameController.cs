using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GameController : MonoBehaviour
{
    [SerializeField] private List<EncounterData> allEncounters;
    private List<CharacterData> playerCharacters;
    private List<CharacterData> enemyCharacters;

    [Header("HUDS")]
    [SerializeField] private GameObject combatHUD;
    [SerializeField] private GameObject dialogueHUD;

    [Header("CombatantInformation")]
    [SerializeField] private TextMeshProUGUI CombatantName;
    [SerializeField] private TextMeshProUGUI CombatantHealth;
    [SerializeField] private TextMeshProUGUI CombatantMP;

    private Dictionary<string, EncounterData> encounterMap;
    private List<Combatant> turnOrder = new();
    private int currentTurnIndex = 0;
    private bool combatActive = false;

    public static GameController Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        encounterMap = new Dictionary<string, EncounterData>();
        foreach (var encounter in allEncounters)
        {
            encounterMap[encounter.encounterID] = encounter;
        }
    }

    public void StartCombat(string encounterID, string continueKnot)
    {
        if (!encounterMap.ContainsKey(encounterID))
        {
            Debug.LogError($"No encountere found with ID: {encounterID}");
        }

        combatHUD.SetActive(true);
        dialogueHUD.SetActive(false);

        combatActive = true;
        var encounter = encounterMap[encounterID];
        enemyCharacters = encounter.enemies;

        PrepareCombatants();

        currentTurnIndex = 0;

        StartCoroutine(CombatLoop());
    }

    private void PrepareCombatants()
    {
        turnOrder.Clear();

        var equippedPlayers = playerCharacters
            .Where(p => p.currentlyEquipped)
            .Take(3)
            .ToList();

        if (equippedPlayers.Count == 0)
        {
            Debug.LogWarning("No equipped player characters selected. Aborting combat start.");
            combatActive = false;
            return;
        }

        foreach (var p in equippedPlayers)
        {
            turnOrder.Add(new Combatant(p, true));
        }

        foreach (var e in enemyCharacters)
        {
            turnOrder.Add(new Combatant(e, false));
        }

        turnOrder.Sort((a, b) => b.data.speed.CompareTo(a.data.speed));
    }

    private IEnumerator CombatLoop()
    {
        yield return null;

        while (combatActive)
        {
            Combatant current = turnOrder[currentTurnIndex];

            if (current.currentHP > 0)
            {
                CombatantName.text = "Current Combatant: " + current.Name;
                CombatantHealth.text = "Combatant HP: " + current.currentHP.ToString();
                CombatantMP.text = "Combatant MP: " + current.currentMP.ToString();

                yield return current.isPlayerControlled
                    ? StartCoroutine(PlayerTurn(current))
                    : StartCoroutine(EnemyTurn(current));
            }

            currentTurnIndex = (currentTurnIndex + 1) % turnOrder.Count;

            // Add a check for victory
        }
    }

    private IEnumerator PlayerTurn(Combatant player)
    {
        yield return null;
    }

    private IEnumerator EnemyTurn(Combatant enemy)
    {
        yield return null;
    }
}
