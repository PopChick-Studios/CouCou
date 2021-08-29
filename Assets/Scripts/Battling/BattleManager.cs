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
    private CouCouFinder coucouFinder;
    private SatchelManager satchelManager;

    public InventoryList inventory;
    public CouCouDatabase coucouDatabase;
    public AbilitiesDatabase abilitiesDatabase;
    public ItemsDatabase itemsDatabase;

    [Space]
    public InventoryList.CouCouInventory activeCouCou;
    public List<InventoryList.CouCouInventory> coucouParty;

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
        coucouFinder = GetComponent<CouCouFinder>();
        catching = GetComponent<Catching>();
        inventoryManager = GetComponent<InventoryManager>();
        battleSystem = GetComponent<BattleSystem>();
        enemyManager = GetComponent<EnemyManager>();
        abilityFinder = GetComponent<AbilityFinder>();
        battlingUI = GameObject.FindGameObjectWithTag("BattlingUI").GetComponent<BattlingUI>();
        abilityDisplay = GameObject.FindGameObjectWithTag("BattlingUI").GetComponent<AbilityDisplay>();
        satchelManager = GameObject.FindGameObjectWithTag("BattlingUI").GetComponent<SatchelManager>();

        activeCouCou = new InventoryList.CouCouInventory();
        coucouParty = new List<InventoryList.CouCouInventory>();

        ItemsDatabaseList = new List<ItemsDatabase>();
    }

    private void Start()
    {

        foreach (InventoryList.CouCouInventory c in inventory.couCouInventory)
        {
            if (c.lineupOrder < 6)
            {
                coucouParty.Add(c);
            }
        }

        activeCouCou = coucouParty[0];

        battleSystem.player = activeCouCou;
        InitializeHealthBarAlly();
        InitializeAbilities();
    }

    public void InitializeHealthBarAlly()
    {
        allyNameText.text = activeCouCou.coucouName;
        allyHealthBar.GetComponent<Image>().fillAmount = activeCouCou.currentHealth / (float)activeCouCou.maxHealth;
        allyHealthText.text = activeCouCou.currentHealth + "/" + activeCouCou.maxHealth;
        allyLevelText.text = activeCouCou.coucouLevel.ToString();
        allyElementSprite.sprite = coucouFinder.GetElementSprite(activeCouCou.element); 
    }

    public void InitializeAbilities()
    {
        CouCouDatabase.CouCouData coucou = coucouFinder.FindCouCou(activeCouCou.coucouName);
        StartCoroutine(abilityDisplay.DisplayAbilities(coucou.ability1, coucou.ability2, coucou.ability3, coucou.ability4));
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
                        StartCoroutine(battleSystem.IncrementallyIncreaseHP((int)(activeCouCou.currentHealth + activeCouCou.maxHealth * 0.3f), activeCouCou));
                        yield return new WaitUntil(() => finishedIncrement);
                        finishedIncrement = false;
                        dialogueText.text = activeCouCou.coucouName + " gained " + (int)Mathf.Min(activeCouCou.maxHealth - activeCouCou.currentHealth, activeCouCou.currentHealth + activeCouCou.maxHealth * 0.3f) + " health";
                        break;

                    case ItemsDatabase.ItemAttribute.Resistance:
                        float resistance = activeCouCou.currentResistance * 1.15f * resistanceDiminishingReturns;
                        dialogueText.text = activeCouCou.coucouName + " gained " + (Mathf.Round(resistance - activeCouCou.currentResistance * 100) / 100) + " resistance";
                        activeCouCou.currentResistance = (int)resistance;
                        resistanceDiminishingReturns *= 0.87f;
                        break;

                    case ItemsDatabase.ItemAttribute.Attack:
                        float attack = activeCouCou.currentAttack * 1.15f * attackDiminishingReturns;
                        dialogueText.text = activeCouCou.coucouName + " gained " + (Mathf.Round(attack - activeCouCou.currentAttack * 100) / 100) + " attack";
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
                        int mindset = 10;
                        dialogueText.text = "The berry gave " + activeCouCou.coucouName + " " + Mathf.Min(100 - activeCouCou.currentMindset, mindset) + " more Mindset";
                        activeCouCou.currentMindset += mindset;

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
        satchelManager.isStuck = false;

        dialogueText.text = "You recall " + activeCouCou.coucouName + " for " + name;

        // Recall Animation

        yield return new WaitForSeconds(2f);

        foreach (InventoryList.CouCouInventory c in coucouParty)
        {
            if (name == c.coucouName)
            {
                inventoryManager.MoveCouCou(c, 0);
                inventoryManager.SortCouCouInventory();
                activeCouCou = c;
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
