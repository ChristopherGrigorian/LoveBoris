using System;
using UnityEngine;
using UnityEngine.UI;

public class CombatHUDManager : MonoBehaviour
{
    [Header("Containers")]
    [SerializeField] private Transform WeaponSkillsContainer;
    [SerializeField] private Transform CharacterSkillsContainer;
    [SerializeField] private Transform ItemsContainer;

    [Header("Prefabs")]
    [SerializeField] private GameObject WSPrefab;
    [SerializeField] private GameObject CSPrefab;
    [SerializeField] private GameObject IPrefab;

    [Header("Tabs")]
    [SerializeField] private Button weaponTabButton;
    [SerializeField] private Button skillTabButton;
    [SerializeField] private Button itemTabButton;

    [Header("Tab Panels")]
    [SerializeField] private GameObject weaponTabPanel;
    [SerializeField] private GameObject skillTabPanel;
    [SerializeField] private GameObject itemTabPanel;

    private CharacterData activeCharacter;
    private System.Action<SkillData> cachedSkillCallback;

    public static CombatHUDManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        weaponTabButton.onClick.AddListener(() => ShowTab("weapon"));
        skillTabButton.onClick.AddListener(() => ShowTab("skill"));
        itemTabButton.onClick.AddListener(() => ShowTab("item"));

        ShowTab("weapon");
    }

    private void ShowTab(string menu)
    {
        weaponTabPanel.SetActive(menu == "weapon");
        skillTabPanel.SetActive(menu == "skill");
        itemTabPanel.SetActive(menu == "item");
    }

    public void HideAllActionUI()
    {
        weaponTabPanel.SetActive(false);
        skillTabPanel.SetActive(false);
        itemTabPanel.SetActive(false);
    }

    public void PopulateActions(CharacterData character, Action<SkillData> onSkillChosen)
    {
        activeCharacter = character;
        cachedSkillCallback = onSkillChosen;

        ClearChildren(WeaponSkillsContainer);
        ClearChildren(CharacterSkillsContainer);
        ClearChildren(ItemsContainer);

        // These lists are up to your data model — rename to match what you actually have
        foreach (var s in character.EquippedWeapon.weaponSkills)
        {
            var btn = Instantiate(WSPrefab, WeaponSkillsContainer);
            var ui = btn.GetComponent<SkillButtonUI>();
            ui.Bind(s, cachedSkillCallback);
        }

        foreach (var s in character.Skills)
        {
            var btn = Instantiate(CSPrefab, CharacterSkillsContainer);
            var ui = btn.GetComponent<SkillButtonUI>();
            ui.Bind(s, cachedSkillCallback);
        }


        ShowTab("weapon");
    }

    private void ClearChildren(Transform t)
    {
        for (int i = t.childCount - 1; i >= 0; i--)
            Destroy(t.GetChild(i).gameObject);
    }
}
