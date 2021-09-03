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
    private GameManager gameManager;

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

    public bool incorrectItemUse = false;

    public float attackDiminishingReturns = 1;
    public float resistanceDiminishingReturns = 1;

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
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

        for (int a = 0; a < coucouParty.Count; a++)
        {
            int level;
            int bonusStatsPer5;
            int bonusStatsPer1;

            for (int i = 0; i < coucouDatabase.coucouVariant.Count; i++)
            {
                level = coucouParty[a].coucouLevel;
                bonusStatsPer5 = Mathf.FloorToInt(level / 5);
                bonusStatsPer1 = level - bonusStatsPer5 - 1;

                if (coucouParty[a].coucouVariant == coucouDatabase.coucouVariant[i].variant)
                {
                    coucouParty[a].maxHealth = coucouDatabase.coucouVariant[i].hp + (coucouDatabase.coucouVariant[i].bonusHP * bonusStatsPer1) + (coucouDatabase.coucouVariant[i].bonusHPPer5 * bonusStatsPer5);
                    coucouParty[a].currentAttack = coucouDatabase.coucouVariant[i].attack + (coucouDatabase.coucouVariant[i].bonusAttack * bonusStatsPer1) + (coucouDatabase.coucouVariant[i].bonusAttackPer5 * bonusStatsPer5);
                    coucouParty[a].currentResistance = coucouDatabase.coucouVariant[i].resistance + (coucouDatabase.coucouVariant[i].bonusResistance * bonusStatsPer1) + (coucouDatabase.coucouVariant[i].bonusResistancePer5 * bonusStatsPer5);
                    coucouParty[a].currentMindset = coucouDatabase.coucouVariant[i].mindset;
                    coucouParty[a].currentDetermination = coucouDatabase.coucouVariant[i].determination;
                }
            }
        }

        for (int a = 0; a < coucouParty.Count; a++)
        {
            if (!coucouParty[a].hasCollapsed)
            {
                activeCouCou = coucouParty[a];
                break;
            }
        }

        battleSystem.player = activeCouCou;
        InitializeHealthBarAlly();
        InitializeAbilities();
    }

    public void InitializeHealthBarAlly()
    {
        float maxEXP = Mathf.Pow(4 * activeCouCou.coucouLevel, 2) / 5;

        allyNameText.text = activeCouCou.coucouName;
        allyHealthBar.GetComponent<Image>().fillAmount = activeCouCou.currentHealth / (float)activeCouCou.maxHealth;
        battleSystem.experienceBar.fillAmount = activeCouCou.coucouEXP / maxEXP;
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
        InventoryList.ItemInventory itemBeingUsed = new InventoryList.ItemInventory();

        foreach (InventoryList.ItemInventory i in inventory.itemInventory)
        {
            if (name == i.itemName)
            {
                itemBeingUsed = i;
                break;
            }
        }

        yield return new WaitForSeconds(1f);

        switch (itemBeingUsed.itemAttribute)
        {
            case ItemsDatabase.ItemAttribute.Capsule:
                if (enemyManager.wild)
                {
                    bool ownsWildCouCou = false;
                    for (int i = 0; i < inventory.couCouInventory.Count; i++)
                    {
                        if (battleSystem.enemy.coucouName == inventory.couCouInventory[i].coucouName)
                        {
                            ownsWildCouCou = true;
                            break;
                        }
                    }
                    if (ownsWildCouCou)
                    {
                        dialogueText.text = "You already own this CouCou";
                        incorrectItemUse = true;
                    }
                    else
                    {
                        dialogueText.text = inventory.preGameDialogue.name + " throws out a " + itemBeingUsed.itemName;
                        usedCapsule = true;
                    }
                }
                else
                {
                    dialogueText.text = "This item can't be used right now";
                    incorrectItemUse = true;
                }
                break;

            case ItemsDatabase.ItemAttribute.Health:
                if (activeCouCou.currentHealth == activeCouCou.maxHealth)
                {
                    dialogueText.text = "This item can't be used right now";
                    incorrectItemUse = true;
                    break;
                }
                float healthToIncrease = activeCouCou.currentHealth + activeCouCou.maxHealth * 0.3f;
                int previousCurrentHealth = activeCouCou.currentHealth;
                StartCoroutine(battleSystem.IncrementallyIncreaseHP((int)healthToIncrease, activeCouCou));
                yield return new WaitUntil(() => battleSystem.finishedHealthIncrement);
                battleSystem.finishedHealthIncrement = false;
                dialogueText.text = activeCouCou.coucouName + " gained " + (int)Mathf.Min(healthToIncrease - previousCurrentHealth, activeCouCou.maxHealth - previousCurrentHealth) + " health";
                break;

            case ItemsDatabase.ItemAttribute.Resistance:
                float resistance = Mathf.Max(activeCouCou.currentResistance, activeCouCou.currentResistance * 1.15f * resistanceDiminishingReturns);
                dialogueText.text = activeCouCou.coucouName + " gained " + (Mathf.Round(resistance - activeCouCou.currentResistance * 100) / 100) + " resistance";
                activeCouCou.currentResistance = resistance;
                resistanceDiminishingReturns *= 0.9f;
                break;

            case ItemsDatabase.ItemAttribute.Attack:
                int attack = Mathf.Max(activeCouCou.currentAttack, Mathf.RoundToInt(activeCouCou.currentAttack * 1.15f * attackDiminishingReturns));
                dialogueText.text = activeCouCou.coucouName + " gained " + (attack - activeCouCou.currentAttack) + " attack";
                activeCouCou.currentAttack = attack;
                attackDiminishingReturns *= 0.9f;
                break;

            case ItemsDatabase.ItemAttribute.ElementalMindset:
                if (activeCouCou.element != itemBeingUsed.element)
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
        battleSystem.state = BattleState.ENEMYTURN;

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

    public IEnumerator OnRun()
    {
        int rnd = Random.Range(0, battleSystem.enemy.currentDetermination);
        if (rnd > 30 && enemyManager.wild)
        {
            dialogueText.text = "You successfully retreat";
            yield return new WaitForSeconds(1f);
            gameManager.SetState(GameManager.GameState.Wandering);
        }
        else if (enemyManager.wild)
        {
            dialogueText.text = "The " + battleSystem.enemy.coucouName + " won't let you escape so easily";
            yield return new WaitForSeconds(2f);
            battleSystem.EnemyTurn();
        }
        else
        {
            dialogueText.text = "You cannot run away against " + enemyManager.enemyInventory.preGameDialogue.name;
            yield return new WaitForSeconds(2f);
            StartCoroutine(battleSystem.PlayerTurn());
        }
    }
}
