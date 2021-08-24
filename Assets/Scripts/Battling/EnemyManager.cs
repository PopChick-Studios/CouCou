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

    private void Awake()
    {
        // CHANGE THIS WHEN YOU CREATE THE INTO BATTLE SCRIPT
        wild = true;

        attackAbilities = new List<AbilitiesDatabase.AttackAbilityData>();
        utilityAbilities = new List<AbilitiesDatabase.UtilityAbilityData>();
        cumulativeProbability = new float[4];
        abilityProbability = new float[4];
        abilityAdvantage = new bool[4];
        coucouVariantList = new List<CouCouDatabase.CouCouVariant>();

        battleSystem = gameObject.GetComponent<BattleSystem>();
        abilityFinder = gameObject.GetComponent<AbilityFinder>();
        battleManager = gameObject.GetComponent<BattleManager>();
    }

    void Start()
    {
        foreach (CouCouDatabase.CouCouVariant c in coucouDatabase.coucouVariant)
        {
            coucouVariantList.Add(c);
        }
        foreach (InventoryList.CouCouInventory c in enemyInventory.couCouInventory)
        {
            if (c.onParty)
            {
                enemyCouCouParty.Insert(Mathf.Min(enemyCouCouParty.Count, c.lineupOrder), c);
            }
            if (c.isCurrentlyActive)
            {
                enemyActiveCouCou = c;
                if (c.onParty)
                {
                    enemyCouCouParty.Remove(c);
                }
                enemyCouCouParty.Insert(0, c);
            }
        }

        for (int i = 0; enemyCouCouParty.Count < Mathf.Min(5, enemyInventory.couCouInventory.Count); i++)
        {
            enemyInventory.couCouInventory[enemyCouCouParty.Count].onParty = true;
            enemyInventory.couCouInventory[enemyCouCouParty.Count].lineupOrder = enemyCouCouParty.Count;
            enemyCouCouParty.Add(enemyInventory.couCouInventory[enemyCouCouParty.Count]);
        }

        // If they haven't set up an active CouCou, default to the first on the list
        if (enemyActiveCouCou.maxHealth == 0)
        {
            enemyCouCouParty[0].isCurrentlyActive = true;
            enemyInventory.couCouInventory[0].isCurrentlyActive = true;
            enemyActiveCouCou = enemyCouCouParty[0];
        }
        InitializeEnemyCouCou();
    }

    public void InitializeHealthBarEnemy()
    {
        enemyNameText.text = enemyActiveCouCou.coucouName;
        enemyHealthBar.GetComponent<Image>().fillAmount = enemyActiveCouCou.currentHealth / enemyActiveCouCou.maxHealth;
        enemyHealthText.text = enemyActiveCouCou.currentHealth + "/" + enemyActiveCouCou.maxHealth;
        enemyLevelText.text = enemyActiveCouCou.coucouLevel.ToString();
        enemyElementSprite = null; // Fix this when sprites are made
    }

    public void InitializeEnemyCouCou()
    {
        int level;
        int bonusStatsPer5;
        int bonusStatsPer1;

        for (int i = 0; i < enemyCouCouParty.Count; i++)
        {
            for (int a = 1; a < coucouVariantList.Count; a++)
            {

                // Be sure too add variant to the inventory when catching CouCou

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

                    // *********** REMOVE THIS ONCE YOU MADE THE CURRENT HEALTH SCRIPT *********** //
                    enemyCouCouParty[i].currentHealth = enemyCouCouParty[i].maxHealth;
                }
            }
        }

        InitializeHealthBarEnemy();

        battleSystem.enemy = enemyActiveCouCou;

        int abilityUID = -1;
        for (int i = 0; i < 4; i++)
        {
            switch (i)
            {
                case 0:
                    abilityUID = enemyActiveCouCou.ability1;
                    break;

                case 1:
                    abilityUID = enemyActiveCouCou.ability2;
                    break;

                case 2:
                    abilityUID = enemyActiveCouCou.ability3;
                    break;

                case 3:
                    abilityUID = enemyActiveCouCou.ability4;
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

    public void InitializeElementalAdvantage()
    {
        for (int i = 0; i < 4; i++)
        {
            if (attackAbilities[i] != null)
            {
                abilityAdvantage[i] = battleSystem.HasDisadvantage(attackAbilities[i].coucouElement, battleSystem.player.element);
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

        previousAbilityIndex = PerformEnemyAbility(abilityProbability);

        if (attackAbilities[previousAbilityIndex] != null)
        {
            // Attack display
            dialogueText.text = enemyActiveCouCou.coucouName + " used " + attackAbilities[previousAbilityIndex].abilityName;
            StartCoroutine(battleSystem.EnemyAbility(enemyActiveCouCou.currentAttack * attackAbilities[previousAbilityIndex].damageMultiplier, attackAbilities[previousAbilityIndex].abilityName, false));
        }
        else
        {
            enemyPsychicAbilitiesUsed++;
            StartCoroutine(battleSystem.EnemyAbility(0, utilityAbilities[previousAbilityIndex].abilityName, false));
            dialogueText.text = enemyActiveCouCou.coucouName + " used " + utilityAbilities[previousAbilityIndex].abilityName;
        }
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
            StartCoroutine(battleSystem.EnemyAbility(enemyActiveCouCou.currentAttack * attackAbilities[previousAbilityIndex].damageMultiplier, attackAbilities[previousAbilityIndex].abilityName, false));
        }
        else
        {
            enemyPsychicAbilitiesUsed++;
            StartCoroutine(battleSystem.EnemyAbility(0, utilityAbilities[previousAbilityIndex].abilityName, false));
            dialogueText.text = enemyActiveCouCou.coucouName + " used " + utilityAbilities[previousAbilityIndex].abilityName;
        }
    }

    public IEnumerator GiveBerry(CouCouDatabase.Element element)
    {
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
        yield return new WaitForSeconds(2f);
    }

    public int PerformEnemyAbility(float[] probability)
    {
        float rnd = Random.Range(1, 101); // Get a random number between 0 and 100

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
            StartCoroutine(battleSystem.EnemyAbility(enemyActiveCouCou.currentAttack * attackAbilities[0].damageMultiplier, attackAbilities[0].abilityName, true));
            return true;
        }
        else
        {
            return false;
        }
    }

    public IEnumerator NextCouCou()
    {
        foreach (InventoryList.CouCouInventory i in enemyCouCouParty)
        {
            if (!i.hasCollapsed)
            {
                dialogueText.text = enemyInventory.inventoryOwner + " swaps " + enemyActiveCouCou.coucouName + " with " + i.coucouName;
                
                yield return new WaitForSeconds(2f);

                // Swapping animation

                enemyActiveCouCou = i;
                battleSystem.enemy = i;

                InitializeEnemyCouCou();

                yield return new WaitForSeconds(2f);

                break;
            }
        }
        if (enemyActiveCouCou.hasCollapsed)
        {
            StartCoroutine(battleSystem.EndBattle());
        }
        else
        {
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
