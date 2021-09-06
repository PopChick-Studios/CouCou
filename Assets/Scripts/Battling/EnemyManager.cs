using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class EnemyManager : MonoBehaviour
{
    private AbilityFinder abilityFinder;
    private BattleManager battleManager;
    private BattleSystem battleSystem;
    private CouCouFinder coucouFinder;
    private InventoryManager inventoryManager;
    private BattlingUI battlingUI;

    public TextMeshProUGUI dialogueText;

    public CouCouDatabase coucouDatabase;
    public InventoryList enemyInventory;
    public InventoryList.CouCouInventory enemyActiveCouCou = null;
    public List<InventoryList.CouCouInventory> enemyCouCouParty;

    public List<CouCouDatabase.CouCouVariant> coucouVariantList;
    public List<AbilitiesDatabase.AttackAbilityData> attackAbilities;
    public List<AbilitiesDatabase.UtilityAbilityData> utilityAbilities;

    public GameObject enemyHealthBar;
    public TextMeshProUGUI enemyNameText;
    public TextMeshProUGUI enemyLevelText;
    public TextMeshProUGUI enemyHealthText;
    public Image enemyElementSprite;

    public float[] cumulativeProbability;
    public float[] abilityProbability;
    public bool[] abilityAdvantage;
    public int amountOfAdvantage = 0;

    public bool initializeFinished;
    public bool wild;
    public int enemyPsychicAbilitiesUsed;

    public int turn;
    public int previousAbilityIndex;

    public float resistanceDiminishingReturns = 1;
    public float attackDiminishingReturns = 1;

    private void Awake()
    {
        if (string.IsNullOrEmpty(enemyInventory.preGameDialogue.name))
        {
            wild = true;
        }
        else
        {
            wild = false;
        }

        attackAbilities = new List<AbilitiesDatabase.AttackAbilityData>();
        utilityAbilities = new List<AbilitiesDatabase.UtilityAbilityData>();
        cumulativeProbability = new float[4];
        abilityProbability = new float[4];
        abilityAdvantage = new bool[4];
        coucouVariantList = new List<CouCouDatabase.CouCouVariant>();

        inventoryManager = GetComponent<InventoryManager>();
        coucouFinder = gameObject.GetComponent<CouCouFinder>();
        battleSystem = gameObject.GetComponent<BattleSystem>();
        abilityFinder = gameObject.GetComponent<AbilityFinder>();
        battleManager = gameObject.GetComponent<BattleManager>();
        battlingUI = GameObject.FindGameObjectWithTag("BattlingUI").GetComponent<BattlingUI>();
    }

    void Start()
    {
        foreach (CouCouDatabase.CouCouVariant c in coucouDatabase.coucouVariant)
        {
            coucouVariantList.Add(c);
        }
        foreach (InventoryList.CouCouInventory c in enemyInventory.couCouInventory)
        {
            Debug.Log("Adding " + c.coucouName + " to enemy party");
            if (c.lineupOrder < 6)
            {
                enemyCouCouParty.Add(c);
            }
        }

        InitializeEnemyCouCou();
    }

    public void InitializeEnemyCouCou()
    {
        int level;
        int bonusStatsPer5;
        int bonusStatsPer1;

        for (int i = 0; i < enemyCouCouParty.Count; i++)
        {
            for (int a = 0; a < coucouVariantList.Count; a++)
            {
                if (enemyCouCouParty[i].coucouVariant == coucouVariantList[a].variant)
                {
                    level = enemyCouCouParty[i].coucouLevel;
                    bonusStatsPer5 = Mathf.FloorToInt(level / 5);
                    bonusStatsPer1 = level - bonusStatsPer5 - 1;

                    enemyCouCouParty[i].maxHealth = coucouVariantList[a].hp + (coucouVariantList[a].bonusHP * bonusStatsPer1) + (coucouVariantList[a].bonusHPPer5 * bonusStatsPer5);
                    enemyCouCouParty[i].currentAttack = coucouVariantList[a].attack + (coucouVariantList[a].bonusAttack * bonusStatsPer1) + (coucouVariantList[a].bonusAttackPer5 * bonusStatsPer5);
                    enemyCouCouParty[i].currentResistance = coucouVariantList[a].resistance + (coucouVariantList[a].bonusResistance * bonusStatsPer1) + (coucouVariantList[a].bonusResistancePer5 * bonusStatsPer5);
                    enemyCouCouParty[i].currentMindset = coucouVariantList[a].mindset;
                    enemyCouCouParty[i].currentDetermination = coucouVariantList[a].determination;

                    enemyCouCouParty[i].currentHealth = enemyCouCouParty[i].maxHealth;
                }
            }
        }

        enemyActiveCouCou = enemyCouCouParty[0];
        battleSystem.enemy = enemyActiveCouCou;
        InitializeHealthBarEnemy();

        int abilityUID = -1;
        for (int i = 0; i < 4; i++)
        {
            switch (i)
            {
                case 0:
                    abilityUID = coucouFinder.FindCouCou(enemyActiveCouCou.coucouName).ability1;
                    break;

                case 1:
                    abilityUID = coucouFinder.FindCouCou(enemyActiveCouCou.coucouName).ability2;
                    break;

                case 2:
                    abilityUID = coucouFinder.FindCouCou(enemyActiveCouCou.coucouName).ability3;
                    break;

                case 3:
                    abilityUID = coucouFinder.FindCouCou(enemyActiveCouCou.coucouName).ability4;
                    break;
            }
            if (abilityUID > 99)
            {
                utilityAbilities.Add(abilityFinder.FindUtilityAbility(abilityUID));
                attackAbilities.Add(null);
            }
            else
            {
                attackAbilities.Add(abilityFinder.FindAttackAbility(abilityUID));
                utilityAbilities.Add(null);
            }
        }

        InitializeElementalAdvantage();
    }

    public void InitializeHealthBarEnemy()
    {
        enemyNameText.text = enemyActiveCouCou.coucouName;
        enemyHealthBar.GetComponent<Image>().fillAmount = enemyActiveCouCou.currentHealth / (float)enemyActiveCouCou.maxHealth;
        enemyHealthText.text = enemyActiveCouCou.currentHealth + "/" + enemyActiveCouCou.maxHealth;
        enemyLevelText.text = enemyActiveCouCou.coucouLevel.ToString();
        enemyElementSprite.sprite = coucouFinder.GetElementSprite(enemyActiveCouCou.element);
    }

    public void InitializeElementalAdvantage()
    {
        for (int i = 0; i < 4; i++)
        {
            if (attackAbilities[i] != null)
            {
                abilityAdvantage[i] = battleSystem.HasAdvantage(attackAbilities[i].coucouElement, battleSystem.player.element);
                if (abilityAdvantage[i])
                {
                    amountOfAdvantage++;
                }
            }
            else
            {
                abilityAdvantage[i] = false;
            }
        }

        initializeFinished = true;
    }

    public void CouCorpAI()
    {
        turn++;
        if (turn == 1)
        {
            for (int i = 0; i < 4; i++)
            {
                switch (amountOfAdvantage)
                {
                    case 0:
                        abilityProbability.SetValue(25, i);
                        break;

                    case 1:
                        if (abilityAdvantage[i])
                        {
                            abilityProbability.SetValue(40, i);
                        }
                        else
                        {
                            abilityProbability.SetValue(20, i);
                        }
                        break;

                    case 2:
                        if (abilityAdvantage[i])
                        {
                            abilityProbability.SetValue(40, i);
                        }
                        else
                        {
                            abilityProbability.SetValue(10, i);
                        }
                        break;

                    case 3:
                        if (abilityAdvantage[i])
                        {
                            abilityProbability.SetValue(30, i);
                        }
                        else
                        {
                            abilityProbability.SetValue(10, i);
                        }
                        break;

                    case 4:
                        abilityProbability.SetValue(25, i);
                        break;
                }                
            }
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                if (i == previousAbilityIndex)
                {
                    abilityProbability.SetValue(abilityProbability[i] - 18, i);
                }
                else
                {
                    abilityProbability.SetValue(abilityProbability[i] + 6, i);
                }

            }
        }

        bool hasRipe = false;
        bool hasElemental = false;

        foreach (InventoryList.ItemInventory item in enemyInventory.itemInventory)
        {
            if (item.itemName == "Ripe Berry")
            {
                hasRipe = true;
            }
            else if (item.element == enemyActiveCouCou.element)
            {
                hasElemental = true;
            }
        }

        int rnd = Random.Range(0, enemyActiveCouCou.currentHealth);

        if (rnd < 50 && enemyActiveCouCou.currentHealth < 200 && hasRipe)
        {
            StartCoroutine(UseItem("Ripe Berry"));
            return;
        }
        else if (rnd < 5 * enemyActiveCouCou.currentHealth / 6 && hasElemental)
        {
            switch (enemyActiveCouCou.element)
            {
                case CouCouDatabase.Element.Flame:
                    StartCoroutine(UseItem("Burnt Berry"));
                    break;

                case CouCouDatabase.Element.Aqua:
                    StartCoroutine(UseItem("Frozen Berry"));
                    break;

                case CouCouDatabase.Element.Nature:
                    StartCoroutine(UseItem("Fruitful Berry"));
                    break;

                case CouCouDatabase.Element.Umbral:
                    StartCoroutine(UseItem("Dark Berry"));
                    break;

                case CouCouDatabase.Element.Lux:
                    StartCoroutine(UseItem("Bright Berry"));
                    break;
            }
            return;
        }

        previousAbilityIndex = PerformEnemyAbility(abilityProbability);

        if (attackAbilities[previousAbilityIndex] != null)
        {
            // Attack display
            dialogueText.text = enemyActiveCouCou.coucouName + " used " + attackAbilities[previousAbilityIndex].abilityName;
            StartCoroutine(battleSystem.EnemyAbility(enemyActiveCouCou.currentAttack * attackAbilities[previousAbilityIndex].damageMultiplier, attackAbilities[previousAbilityIndex].uniqueIdentifier, false));
        }
        else
        {
            enemyPsychicAbilitiesUsed++;
            StartCoroutine(battleSystem.EnemyAbility(0, utilityAbilities[previousAbilityIndex].uniqueIdentifier, false));
            dialogueText.text = enemyActiveCouCou.coucouName + " used " + utilityAbilities[previousAbilityIndex].abilityName;
        }
    }

    public IEnumerator UseItem(string name)
    {
        Debug.Log("Enemy used " + name);

        foreach (InventoryList.ItemInventory i in enemyInventory.itemInventory)
        {
            if (name == i.itemName)
            {
                yield return new WaitForSeconds(1f);

                switch (i.itemAttribute)
                {
                    case ItemsDatabase.ItemAttribute.Health:
                        float healthToIncrease = enemyActiveCouCou.currentHealth + enemyActiveCouCou.maxHealth * 0.3f;
                        int previousCurrentHealth = enemyActiveCouCou.currentHealth;
                        StartCoroutine(battleSystem.IncrementallyIncreaseHP((int)healthToIncrease, enemyActiveCouCou));
                        yield return new WaitUntil(() => battleSystem.finishedHealthIncrement);
                        battleSystem.finishedHealthIncrement = false;
                        dialogueText.text = enemyActiveCouCou.coucouName + " gained " + (int)Mathf.Min(healthToIncrease - previousCurrentHealth, enemyActiveCouCou.maxHealth - previousCurrentHealth) + " health";
                        break;

                    case ItemsDatabase.ItemAttribute.ElementalMindset:

                        dialogueText.text = "The berry gave " + enemyActiveCouCou.coucouName + " " + Mathf.Min(100 - enemyActiveCouCou.currentMindset, 10) + " more Mindset";
                        enemyActiveCouCou.currentMindset += 10;
                        break;
                }
            }
        }
        battleSystem.enemy = enemyActiveCouCou;
        inventoryManager.EnemyUsedItem(name);

        yield return new WaitForSeconds(2f);
        StartCoroutine(battleSystem.PlayerTurn());
    }

    public void WildCouCouAttack()
    {
        turn++;
        if (turn == 1)
        {
            for(int i = 0; i < 4; i++)
            {
                abilityProbability.SetValue(25, i);
            }
        }
        else
        {
            for(int i = 0; i < 4; i++)
            {
                if (i == previousAbilityIndex)
                {
                    abilityProbability.SetValue(abilityProbability[i] - 18, i);
                }
                else
                {
                    abilityProbability.SetValue(abilityProbability[i] + 6, i);
                }

            }
        }
       
        previousAbilityIndex = PerformEnemyAbility(abilityProbability);

        if (attackAbilities[previousAbilityIndex] != null)
        {
            // Attack display
            dialogueText.text = enemyActiveCouCou.coucouName + " used " + attackAbilities[previousAbilityIndex].abilityName;
            StartCoroutine(battleSystem.EnemyAbility(enemyActiveCouCou.currentAttack * attackAbilities[previousAbilityIndex].damageMultiplier, attackAbilities[previousAbilityIndex].uniqueIdentifier, false));
        }
        else
        {
            enemyPsychicAbilitiesUsed++;
            StartCoroutine(battleSystem.EnemyAbility(0, utilityAbilities[previousAbilityIndex].uniqueIdentifier, false));
            dialogueText.text = enemyActiveCouCou.coucouName + " used " + utilityAbilities[previousAbilityIndex].abilityName;
        }
    }

    public IEnumerator GiveBerry(CouCouDatabase.Element element, string berryName)
    {
        Debug.Log("Enemy " + enemyActiveCouCou.element + " vs Berry " + element);

        if (enemyActiveCouCou.element == element)
        {
            dialogueText.text = "The wild " + enemyActiveCouCou.coucouName + " gained " + Mathf.Min(100 - enemyActiveCouCou.currentMindset, 20) + " mindset";
            enemyActiveCouCou.currentDetermination += 20;
            if (enemyActiveCouCou.currentMindset > 100)
            {
                enemyActiveCouCou.currentMindset = 100;
            }
        }
        else
        {
            dialogueText.text = "The wild " + enemyActiveCouCou.coucouName + " didn't like that berry";
        }

        inventoryManager.UsedItem(berryName);

        yield return new WaitForSeconds(2f);
        battleSystem.EnemyTurn();
        battlingUI.OnFinishTurn();
    }

    public int PerformEnemyAbility(float[] probability)
    {
        float rnd = Random.Range(0, 101); // Get a random number between 0 and 100

        if (!MakeCumulativeProbability(probability))
            return -1;

        for (int i = 0; i < probability.Length; i++)
        {
            if (rnd <= cumulativeProbability[i]) // If the probility reach the correct sum
            {
                return i;
            }
        }
        return -1; // return -1 if some error happens
    }

    bool MakeCumulativeProbability(float[] probability)
    {
        float probabilitiesSum = 0;

        cumulativeProbability = new float[4]; // reset the Array

        for (int i = 0; i < probability.Length; i++)
        {
            probabilitiesSum += probability[i]; // add the probability to the sum
            cumulativeProbability.SetValue(probabilitiesSum, i); // add the new sum to the list

            // All Probabilities need to be under 100% or it'll throw an exception
            if (probabilitiesSum > 100f)
            {
                Debug.LogError("Probabilities exceed 100%");
                return false;
            }
        }
        return true;
    }

    public bool SurpriseAttack()
    {
        float chance = 3f * enemyActiveCouCou.currentDetermination / 5f;
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
            dialogueText.text = enemyActiveCouCou.coucouName + " surprised " + battleSystem.player.coucouName + " with " + attackAbilities[previousAbilityIndex].abilityName;
            battleSystem.state = BattleState.ENEMYTURN;

            for (int i = 0; i < attackAbilities.Count; i++)
            {
                if (attackAbilities[i] != null)
                {
                    StartCoroutine(battleSystem.EnemyAbility(enemyActiveCouCou.currentAttack * attackAbilities[previousAbilityIndex].damageMultiplier, attackAbilities[previousAbilityIndex].uniqueIdentifier, false));
                    break;
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public IEnumerator NextCouCou()
    {
        foreach (InventoryList.CouCouInventory coucou in enemyCouCouParty)
        {
            if (!coucou.hasCollapsed)
            {
                dialogueText.text = enemyInventory.preGameDialogue.name + " swaps " + enemyActiveCouCou.coucouName + " with " + coucou.coucouName;
                enemyCouCouParty.Remove(coucou);
                yield return new WaitForSeconds(2f);

                // Swapping animation

                enemyActiveCouCou = coucou;
                enemyCouCouParty.Insert(0, enemyActiveCouCou);

                battleSystem.enemy = coucou;

                InitializeEnemyCouCou();

                yield return new WaitForSeconds(2f);

                break;
            }
        }
        if (enemyActiveCouCou.hasCollapsed)
        {
            Debug.Log("end battle");
            battleSystem.state = BattleState.WON;
            StartCoroutine(battleSystem.EndBattle());
        }
        else
        {
            battleSystem.InstantiateModels(false, enemyActiveCouCou.coucouName);
            yield return new WaitForSeconds(1f);
            bool surpriseSuccess = battleManager.SurpriseAttack();
            if (surpriseSuccess)
            {
                StartCoroutine(battleSystem.PlayerTurn());
            }
            else
            {
                battleSystem.EnemyTurn();
            }
        }
    }
}
