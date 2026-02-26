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

    [Header("Enemy Target Buttons")]
    [SerializeField] private Transform enemyTargetContainer; // parent that holds enemy buttons
    [SerializeField] private EnemyTargetButton enemyTargetButtonPrefab;

    private Dictionary<string, EncounterData> encounterMap;
    private List<Combatant> turnOrder = new();
    private int currentTurnIndex = 0;
    private bool combatActive = false;

    public Combatant currentCombatant;
    public static GameController Instance;

    private SkillData selectedSkill;
    private Combatant selectedTarget;


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
        BuildEnemyTargetButtons();

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

    private void BuildEnemyTargetButtons()
    {
        // clear old
        for (int i = enemyTargetContainer.childCount - 1; i >= 0; i--)
            Destroy(enemyTargetContainer.GetChild(i).gameObject);

        // create one button per enemy combatant in current turnOrder
        var enemies = turnOrder.Where(c => !c.isPlayerControlled).ToList();
        foreach (var enemy in enemies)
        {
            var btn = Instantiate(enemyTargetButtonPrefab, enemyTargetContainer);
            btn.Bind(enemy, OnEnemyTargetClicked);
            btn.SetInteractable(false); // default off, only on during targeting
        }
    }

    private IEnumerator CombatLoop()
    {
        yield return null;

        while (combatActive)
        {
            currentCombatant = turnOrder[currentTurnIndex];

            if (currentCombatant.currentHP > 0)
            {
                CombatantName.text = "Current Combatant: " + currentCombatant.Name;
                CombatantHealth.text = "Combatant HP: " + currentCombatant.currentHP.ToString();
                CombatantMP.text = "Combatant MP: " + currentCombatant.currentMP.ToString();

                yield return currentCombatant.isPlayerControlled
                    ? StartCoroutine(PlayerTurn(currentCombatant))
                    : StartCoroutine(EnemyTurn(currentCombatant));
            }

            currentTurnIndex = (currentTurnIndex + 1) % turnOrder.Count;

            // Add a check for victory
        }
    }

    private IEnumerator PlayerTurn(Combatant player)
    {
        // reset selections
        selectedSkill = null;
        selectedTarget = null;

        // show player action HUD + populate buttons for THIS character
        CombatHUDManager.Instance.PopulateActions(player.data, OnSkillChosen);

        // WAIT for skill selection
        yield return new WaitUntil(() => selectedSkill != null);

        // If the skill needs a target, enable enemy buttons and wait
        if (selectedSkill.targetsEnemies)
        {
            SetEnemyTargetButtonsEnabled(true);
            yield return new WaitUntil(() => selectedTarget != null);
            SetEnemyTargetButtonsEnabled(false);
        }

        CombatHUDManager.Instance.HideAllActionUI();

        // Resolve action (you fill this in)
        Debug.Log($"Player used {selectedSkill.skillName} on {(selectedTarget != null ? selectedTarget.Name : "no target")}");
        yield return null;
    }

    private IEnumerator EnemyTurn(Combatant enemy)
    {
        yield return null;
    }

    private void OnSkillChosen(SkillData skill)
    {
        selectedSkill = skill;

        // if it does NOT require a target, you're done immediately
        if (!skill.targetsEnemies)
            selectedTarget = null;
    }

    private void OnEnemyTargetClicked(Combatant enemy)
    {
        // Only accept target clicks if we’re actually targeting
        if (selectedSkill == null || !selectedSkill.targetsEnemies) return;
        if (enemy.currentHP <= 0) return;

        selectedTarget = enemy;
    }

    private void SetEnemyTargetButtonsEnabled(bool enabled)
    {
        for (int i = 0; i < enemyTargetContainer.childCount; i++)
        {
            var btn = enemyTargetContainer.GetChild(i).GetComponent<EnemyTargetButton>();
            btn.SetInteractable(enabled);
        }
    }
}
