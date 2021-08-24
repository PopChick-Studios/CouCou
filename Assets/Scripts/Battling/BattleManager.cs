using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    private AbilityDisplay abilityDisplay;
    private AbilityFinder abilityFinder;
    private EnemyManager enemyManager;
    private BattleSystem battleSystem;
    private BattlingUI battlingUI;
    private InventoryManager inventoryManager;
    private Catching catching;

    public InventoryList inventory;
    public CouCouDatabase coucouDatabase;
    public AbilitiesDatabase abilitiesDatabase;
    public ItemsDatabase itemsDatabase;

    [Space]
    public InventoryList.CouCouInventory activeCouCou;
    public List<InventoryList.CouCouInventory> coucouParty;

    private AbilitiesDatabase.AttackAbilityData attackAbility;
    private AbilitiesDatabase.UtilityAbilityData utilityAbility;

    public List<CouCouDatabase.CouCouData> coucouDataList;
    public List<CouCouDatabase.CouCouVariant> coucouVariantList;

    public List<ItemsDatabase> ItemsDatabaseList;

    [Header("Dialogue")]
    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueText;

    [Header("Health Bars")]
    public GameObject allyHealthBar;
    public TextMeshProUGUI allyNameText;
    public TextMeshProUGUI allyLevelText;
    public TextMeshProUGUI allyHealthText;
    public Image allyElementSprite;

    public bool finishedIncrement;
    public bool incorrectItemUse = false;

    public float attackDiminishingReturns = 1;
    public float resistanceDiminishingReturns = 1;

    private void Awake()
    {
        catching = GetComponent<Catching>();
        inventoryManager = GetComponent<InventoryManager>();
        battleSystem = GetComponent<BattleSystem>();
        enemyManager = GetComponent<EnemyManager>();
        abilityFinder = GetComponent<AbilityFinder>();
        battlingUI = GameObject.FindGameObjectWithTag("BattlingUI").GetComponent<BattlingUI>();
        abilityDisplay = GameObject.FindGameObjectWithTag("BattlingUI").GetComponent<AbilityDisplay>();

        activeCouCou = new InventoryList.CouCouInventory();
        coucouParty = new List<InventoryList.CouCouInventory>();

        coucouDataList = new List<CouCouDatabase.CouCouData>();
        coucouVariantList = new List<CouCouDatabase.CouCouVariant>();

        ItemsDatabaseList = new List<ItemsDatabase>();
    }

    private void Start()
    {

        foreach (InventoryList.CouCouInventory c in inventory.couCouInventory)
        {
            if (c.onParty)
            {
                coucouParty.Insert(Mathf.Min(coucouParty.Count, c.lineupOrder), c);
            }
            if (c.isCurrentlyActive)
            {
                activeCouCou = c;
                if (c.onParty)
                {
                    coucouParty.Remove(c);
                }
                coucouParty.Insert(0, c);
            }
        }

        // If they haven't set up a party, default to the list given.
        for (int i = 0; coucouParty.Count < Mathf.Min(5, inventory.couCouInventory.Count); i++)
        {
            inventory.couCouInventory[coucouParty.Count].onParty = true;
            inventory.couCouInventory[coucouParty.Count].lineupOrder = coucouParty.Count;
            coucouParty.Add(inventory.couCouInventory[coucouParty.Count]);
        }

        // If they haven't set up an active CouCou, default to the first on the list
        if (activeCouCou.coucouLevel == 0)
        {
            coucouParty[0].isCurrentlyActive = true;
            inventory.couCouInventory[0].isCurrentlyActive = true;
            activeCouCou = coucouParty[0];
        }

        if (coucouParty.Count > 5)
        {
            coucouParty.RemoveRange(5, coucouParty.Count - 5);
        }

        foreach (CouCouDatabase.CouCouData c in coucouDatabase.coucouData)
        {
            coucouDataList.Add(c);
        }
        foreach (CouCouDatabase.CouCouVariant c in coucouDatabase.coucouVariant)
        {
            coucouVariantList.Add(c);
        }

        // Give the CouCou in party stats
        int level;
        int bonusStatsPer5;
        int bonusStatsPer1;

        for (int i = 0; i < coucouParty.Count; i++)
        {
            for (int a = 1; a < coucouVariantList.Count; a++)
            {

                // Be sure too add variant to the inventory when catching CouCou

                if (coucouParty[i].coucouVariant == coucouVariantList[a].variant)
                {
                    level = coucouParty[i].coucouLevel;
                    bonusStatsPer5 = Mathf.FloorToInt(level / 5);
                    bonusStatsPer1 = level - bonusStatsPer5 - 1;

                    coucouParty[i].maxHealth = coucouVariantList[a].hp + (coucouVariantList[a].bonusHP * bonusStatsPer1) + (coucouVariantList[a].bonusHPPer5 * bonusStatsPer5);
                    coucouParty[i].currentAttack = coucouVariantList[a].attack + (coucouVariantList[a].bonusAttack * bonusStatsPer1) + (coucouVariantList[a].bonusAttackPer5 * bonusStatsPer5);
                    coucouParty[i].currentResistance = coucouVariantList[a].resistance + (coucouVariantList[a].bonusResistance * bonusStatsPer1) + (coucouVariantList[a].bonusResistancePer5 * bonusStatsPer5);
                    coucouParty[i].currentMindset = coucouVariantList[a].mindset;
                    coucouParty[i].currentDetermination = coucouVariantList[a].determination;

                    // *********** REMOVE THIS ONCE YOU MADE THE CURRENT HEALTH SCRIPT *********** //
                    coucouParty[i].currentHealth = coucouParty[i].maxHealth;
                }
            }
        }
        battleSystem.player = activeCouCou;
        InitializeHealthBarAlly();
        InitializeAbilities();
    }

    public void InitializeHealthBarAlly()
    {
        allyNameText.text = activeCouCou.coucouName;
        allyHealthBar.GetComponent<Image>().fillAmount = activeCouCou.currentHealth / activeCouCou.maxHealth;
        allyHealthText.text = activeCouCou.currentHealth + "/" + activeCouCou.maxHealth;
        allyLevelText.text = activeCouCou.coucouLevel.ToString();
        allyElementSprite = null; // Fix this when sprites are made
    }

    public void InitializeAbilities()
    {
        StartCoroutine(abilityDisplay.DisplayAbilities(activeCouCou.ability1, activeCouCou.ability2, activeCouCou.ability3, activeCouCou.ability4));
    }

    public IEnumerator UseItem(string name)
    {
        bool usedCapsule = false;

        foreach (InventoryList.ItemInventory i in inventory.itemInventory)
        {
            if (name == i.itemName)
            {
                yield return new WaitForSeconds(1f);

                switch (i.itemAttribute)
                {
                    case ItemsDatabase.ItemAttribute.Capsule:
                        if (enemyManager.wild)
                        {
                            dialogueText.text = inventory.inventoryOwner + " throws out a " + i.itemName;
                            usedCapsule = true;
                        }
                        else
                        {
                            dialogueText.text = "This item can't be used right now";
                            incorrectItemUse = true;
                        }                        
                        break;

                    case ItemsDatabase.ItemAttribute.Health:
                        StartCoroutine(IncrementallyIncreaseHP((int)(activeCouCou.currentHealth + activeCouCou.maxHealth * 0.3f)));
                        yield return new WaitUntil(() => finishedIncrement);
                        finishedIncrement = false;
                        dialogueText.text = activeCouCou.coucouName + " gained " + (int)Mathf.Min(activeCouCou.maxHealth - activeCouCou.currentHealth, activeCouCou.currentHealth + activeCouCou.maxHealth * 0.3f) + " health";
                        break;

                    case ItemsDatabase.ItemAttribute.Resistance:
                        float resistance = activeCouCou.currentResistance * 1.15f * resistanceDiminishingReturns;
                        dialogueText.text = activeCouCou.coucouName + " gained " + (int)resistance + " resistance";
                        activeCouCou.currentResistance = (int)resistance;
                        resistanceDiminishingReturns *= 0.87f;
                        break;

                    case ItemsDatabase.ItemAttribute.Attack:
                        float attack = activeCouCou.currentAttack * 1.15f * attackDiminishingReturns;
                        dialogueText.text = activeCouCou.coucouName + " gained " + (int)attack + " attack";
                        activeCouCou.currentAttack = (int)attack;
                        attackDiminishingReturns *= 0.87f;
                        break;

                    case ItemsDatabase.ItemAttribute.ElementalMindset:
                        if (activeCouCou.element != i.element)
                        {
                            dialogueBox.SetActive(true);
                            dialogueText.text = "This berry can't be used with " + activeCouCou.coucouName;
                            yield return new WaitForSeconds(2f);
                            dialogueBox.SetActive(false);
                            incorrectItemUse = true;
                           break;
                        }
                        dialogueText.text = "The berry gave " + activeCouCou.coucouName + " " + Mathf.Min(30 - activeCouCou.currentMindset, activeCouCou.currentMindset + 5) +  " more Mindset";
                        activeCouCou.currentMindset += 5;

                        break;

                    default:
                        dialogueBox.SetActive(true);
                        dialogueText.text = "This item can't be used right now";
                        yield return new WaitForSeconds(2f);
                        dialogueBox.SetActive(false);
                        incorrectItemUse = true;
                        break;
                }
            }
        }

        if (incorrectItemUse)
        {
            battlingUI.OnNewRound();
            yield break;
        }

        inventoryManager.UsedItem(name);

        if (!usedCapsule)
        {
            yield return new WaitForSeconds(2f);
            battleSystem.EnemyTurn();
        }
        else
        {
            StartCoroutine(catching.CatchCouCou());
        }
    }

    public IEnumerator ChangeCouCou(string name)
    {
        dialogueText.text = "You recall " + activeCouCou.coucouName + " for " + name;

        // Recall Animation

        yield return new WaitForSeconds(2f);

        foreach (InventoryList.CouCouInventory c in coucouParty)
        {
            if (name == c.coucouName)
            {
                activeCouCou.lineupOrder = c.lineupOrder;
                activeCouCou = c;
                activeCouCou.lineupOrder = 0;
                battleSystem.player = activeCouCou;
                InitializeHealthBarAlly();
                InitializeAbilities();
            }
        }
        enemyManager.InitializeElementalAdvantage();
        yield return new WaitForSeconds(2f);
        bool surpirseSuccess = enemyManager.SurpriseAttack();
        if (!surpirseSuccess)
        {
            StartCoroutine(battleSystem.PlayerTurn());
        }
    }

    public IEnumerator IncrementallyIncreaseHP(int desiredHP)
    {
        float increase = activeCouCou.currentHealth;

        while (increase != desiredHP && activeCouCou.currentHealth < activeCouCou.maxHealth)
        {
            increase = Mathf.MoveTowards(increase, desiredHP, Time.deltaTime * 500f);
            allyHealthBar.GetComponent<Image>().fillAmount = increase / activeCouCou.maxHealth;
            allyHealthText.text = (int)increase + "/" + activeCouCou.maxHealth;
            activeCouCou.currentHealth = (int)increase;
            yield return new WaitForSeconds(0.02f);
        }
        finishedIncrement = true;
        if (activeCouCou.currentHealth >= activeCouCou.maxHealth)
        {
            activeCouCou.currentHealth = activeCouCou.maxHealth;
        }
        yield break;
    }

    public bool SurpriseAttack()
    {
        float chance = 3f * activeCouCou.currentDetermination / 5f;
        float rnd = Random.Range(1, 101);

        if (chance < 0)
        {
            chance = 0;
        }
        else if (chance > 60)
        {
            chance = 60;
        }

        if (rnd <= chance)
        {
            StartCoroutine(battleSystem.PlayerTurn());
            battleSystem.state = BattleState.PLAYERTURN;
            return true;
        }
        else
        {
            return false;
        }
    }
}
