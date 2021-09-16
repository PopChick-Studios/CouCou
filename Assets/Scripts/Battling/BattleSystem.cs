using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum BattleState { NOTBATTLING, START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public GameObject menu;
    public GameObject playerInScene;
    public GameObject enemyInScene;

    public QuestScriptable questScriptable;
    public InventoryList.CouCouInventory player;
    public InventoryList.CouCouInventory enemy;
    public bool takeDamageFinished = false;

    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI sentenceText;
    public Image allyHealthBar;
    public Image experienceBar;
    public TextMeshProUGUI allyHealthText;
    public Image enemyHealthBar;
    public TextMeshProUGUI enemyHealthText;
    public Transform playerSpawner;
    public Transform enemySpawner;

    public int enemyPsychicAbilitiesUsed = 0;
    public int playerPsychicAbilitiesUsed = 0;
    public bool finishedHealthIncrement;
    public bool dialogueFinished;
    public bool grantingExperience = false;

    private BattleManager battleManager;
    private EnemyManager enemyManager;
    private BattlingUI battlingUI;
    private Catching catching;
    private CouCouFinder coucouFinder;
    private AbilityFinder abilityFinder;
    private GameManager gameManager;
    private InventoryManager inventoryManager;
    private SatchelManager satchelManager;

    public BattleState state;

    private void Awake()
    {
        inventoryManager = GetComponent<InventoryManager>();
        gameManager = GetComponent<GameManager>();
        abilityFinder = GetComponent<AbilityFinder>();
        coucouFinder = GetComponent<CouCouFinder>();
        catching = GetComponent<Catching>();
        battleManager = GetComponent<BattleManager>();
        enemyManager = GetComponent<EnemyManager>();
        battlingUI = GameObject.FindGameObjectWithTag("BattlingUI").GetComponent<BattlingUI>();
        satchelManager = GameObject.FindGameObjectWithTag("BattlingUI").GetComponent<SatchelManager>();
    }

    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    public void InstantiateModels(bool isPlayer, string coucouName)
    {
        if (isPlayer)
        {
            playerInScene = Instantiate(coucouFinder.FindCouCou(coucouName).coucouBattleModel, playerSpawner);
        }
        else
        {
            enemyInScene = Instantiate(coucouFinder.FindCouCou(coucouName).coucouBattleModel, enemySpawner);
        }
    }

    public IEnumerator SetupBattle()
    {
        gameManager.SetState(GameManager.GameState.Battling);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        InstantiateModels(true, player.coucouName);
        InstantiateModels(false, enemy.coucouName);

        if (!enemyManager.wild)
        {
            dialogueFinished = false;
            StartCoroutine(StartingDialogue());
            yield return new WaitUntil(() => dialogueFinished);
        }
        speakerText.gameObject.SetActive(false);
        sentenceText.gameObject.SetActive(false);
        StartCoroutine(PlayerTurn());
    }

    public IEnumerator StartingDialogue()
    {
        menu.SetActive(false);
        dialogueText.gameObject.SetActive(false);
        speakerText.gameObject.SetActive(true);
        sentenceText.gameObject.SetActive(true);
        speakerText.text = enemyManager.enemyInventory.preGameDialogue.name;
        sentenceText.text = "";
        yield return new WaitForSeconds(0.65f);
        foreach (string sentence in enemyManager.enemyInventory.preGameDialogue.sentences)
        {
            Debug.Log(sentence);
            sentenceText.text = "";
            foreach (char letter in sentence.ToCharArray())
            {
                sentenceText.text += letter;
                yield return new WaitForSeconds(0.05f);
            }
            yield return new WaitForSeconds(2f);
        }

        dialogueText.gameObject.SetActive(true);
        speakerText.gameObject.SetActive(false);
        sentenceText.gameObject.SetActive(false);
        menu.SetActive(true);
        dialogueFinished = true;
    }

    public IEnumerator PlayerTurn()
    {
        state = BattleState.PLAYERTURN;
        battlingUI.OnNewRound();
        dialogueText.text = "Choose an option...";
        yield return new WaitForSeconds(2f);
    }

    public void EnemyTurn()
    {
        state = BattleState.ENEMYTURN;
        
        if (enemyManager.wild)
        {
            enemyManager.WildCouCouAttack();
        }
        else
        {
            enemyManager.CouCorpAI();
        }
    }

    public void OnButtonCall(AbilityUID button)
    {
        dialogueText.text = player.coucouName + " used " + button.GetComponentInChildren<TextMeshProUGUI>().text;
        battlingUI.OnFinishTurn();
        StartCoroutine(UseAbility(button));
    }

    public IEnumerator EnemyAbility(float damage, int abilityUID, bool surprise)
    {
        string ability = "";
        AbilitiesDatabase.AttackAbilityData attackAbility = null;
        AbilitiesDatabase.UtilityAbilityData utilityAbility = null;
        bool isUtility;
        float waitAfterAbility = 2.5f;

        Debug.Log("Ability UID: " + abilityUID);
        Debug.Log("Damage: " + damage);

        if (abilityUID > 99)
        {
            utilityAbility = abilityFinder.FindUtilityAbility(abilityUID);
            ability = utilityAbility.abilityName;
            isUtility = true;
        }
        else
        {
            attackAbility = abilityFinder.FindAttackAbility(abilityUID);
            ability = attackAbility.abilityName;
            isUtility = false;
        }

        if (!surprise)
        {
            dialogueText.text = enemy.coucouName + " used " + ability;
        }

        bool isDead = false;
        bool crit = false;
        bool abilityDisadvantage = false;
        bool abilityAdvantage = false;

        if (isUtility)
        {
            yield return new WaitForSeconds(waitAfterAbility);
            if (utilityAbility.enemyMindset)
            {
                enemyPsychicAbilitiesUsed++;
                ReduceMindset(enemyPsychicAbilitiesUsed);
                yield return new WaitForSeconds(waitAfterAbility);
            }
            if (utilityAbility.canStun)
            {
                dialogueText.text = enemy.coucouName + " has stunned " + player.coucouName;
                player.isStunned = true;
                yield return new WaitForSeconds(waitAfterAbility);
            }
            if (utilityAbility.resistanceMultiplier != 1)
            {
                float resistance = Mathf.Max(enemy.currentResistance, enemy.currentResistance * utilityAbility.resistanceMultiplier * enemyManager.resistanceDiminishingReturns);
                dialogueText.text = enemy.coucouName + " gained " + ((Mathf.Round(resistance * 100f) / 100f) - enemy.currentResistance) + " resistance";
                enemy.currentResistance = resistance;
                enemyManager.resistanceDiminishingReturns *= 0.9f;
                yield return new WaitForSeconds(waitAfterAbility);
            }
            if (utilityAbility.selfMindset > 0)
            {
                dialogueText.text = enemy.coucouName + " gained " + Mathf.Min(100 - enemy.currentMindset, utilityAbility.selfMindset).ToString() + " mindset";
                enemy.currentMindset += (int)utilityAbility.selfMindset;
                if (enemy.currentMindset > 100)
                {
                    enemy.currentMindset = 100;
                }
                yield return new WaitForSeconds(waitAfterAbility);
            }
            if (utilityAbility.selfDetermination > 0)
            {
                dialogueText.text = enemy.coucouName + " gained " + (int)Mathf.Min(100 - enemy.currentDetermination, utilityAbility.selfDetermination) + " determination";
                enemy.currentDetermination += (int)utilityAbility.selfDetermination;
                if (enemy.currentDetermination > 100)
                {
                    enemy.currentDetermination = 100;
                }
                yield return new WaitForSeconds(waitAfterAbility);
            }
            if (utilityAbility.enemyDetermination > 0)
            {
                int determinationAbsolute = (int)Mathf.Abs(utilityAbility.enemyDetermination);
                dialogueText.text = player.coucouName + " lost " + Mathf.Min(Mathf.Max(100 - enemy.currentDetermination, 0), determinationAbsolute) + " determination";
                enemy.currentDetermination -= (int)utilityAbility.enemyDetermination;
                if (enemy.currentDetermination < 0)
                {
                    enemy.currentDetermination = 0;
                }
                yield return new WaitForSeconds(waitAfterAbility);
            }
            if (utilityAbility.enemyDetermination < 0)
            {
                int determinationAbsolute = (int)Mathf.Abs(utilityAbility.enemyDetermination);
                dialogueText.text = player.coucouName + " gained " + Mathf.Min(100 - enemy.currentDetermination, determinationAbsolute) + " determination";
                enemy.currentDetermination += determinationAbsolute;
                if (enemy.currentDetermination > 100)
                {
                    enemy.currentDetermination = 100;
                }
                yield return new WaitForSeconds(waitAfterAbility);
            }
        }
        else
        {
            abilityDisadvantage = HasDisadvantage(attackAbility.coucouElement, player.element);
            abilityAdvantage = HasAdvantage(attackAbility.coucouElement, player.element);
            crit = false;
            float damageModifier = 1;
            int rnd = Random.Range(1, 101);
            Debug.Log(rnd + " + " + enemy.currentMindset);
            if (abilityDisadvantage)
            {
                damageModifier = 0.9f;
            }
            else if (abilityAdvantage)
            {
                damageModifier = 1.12f;
            }
            if (rnd <= enemy.currentMindset)
            {
                damageModifier *= 1 + (enemy.currentDetermination / 100f);
                crit = true;
            }

            float damageAfterResistance = damage / player.currentResistance;
            isDead = TakeDamage(damageAfterResistance * damageModifier);
            yield return new WaitUntil(() => takeDamageFinished);
            takeDamageFinished = false;

            if (crit)
            {
                dialogueText.text = enemy.coucouName + " critically hit!";
            }
            else if (abilityDisadvantage)
            {
                dialogueText.text = "The attack wasn't very outstanding...";
            }
            else if (abilityAdvantage)
            {
                dialogueText.text = "The attack was glorious!";
            }
            yield return new WaitForSeconds(2f);
        }

        if (isDead)
        {
            dialogueText.text = player.coucouName + " has collapsed";
            player.hasCollapsed = true;
            Destroy(playerInScene);
            inventoryManager.SortCouCouInventory();

            if (inventoryManager.HasPlayableCouCou())
            {
                battlingUI.OnSatchel();
                satchelManager.OnCouCouSection();
            }
            else
            {
                state = BattleState.LOST;
                StartCoroutine(EndBattle());
            }
        }
        else if (player.isStunned)
        {
            dialogueText.text = player.coucouName + " is stunned";
            player.isStunned = false;
            yield return new WaitForSeconds(2f);
            EnemyTurn();
        }
        else
        {
            battlingUI.OnNewRound();
            StartCoroutine(PlayerTurn());
        }
        yield break;
    }

    public IEnumerator UseAbility(AbilityUID button)
    {
        yield return new WaitForSeconds(2f);

        float waitAfterAbility = 2.5f;

        int abilityUID = button.abilityUID;
        bool isUtility = button.isUtility;
        bool isDead = false;

        if (isUtility)
        {
            if (button.utilityAbility.enemyMindset)
            {
                playerPsychicAbilitiesUsed++;
                ReduceMindset(playerPsychicAbilitiesUsed);
                yield return new WaitForSeconds(waitAfterAbility);
            }
            if (button.utilityAbility.canStun)
            {
                int rnd = Random.Range(0, 101 - enemy.currentMindset);
                if (rnd < 30)
                {
                    dialogueText.text = player.coucouName + " has stunned " + enemy.coucouName;
                    enemy.isStunned = true;
                }
                else
                {
                    dialogueText.text = enemy.coucouName + "'s mental is too strong to be stunned";
                }
                yield return new WaitForSeconds(waitAfterAbility);
            }
            if (button.utilityAbility.resistanceMultiplier != 1)
            {
                float resistance = Mathf.Max(player.currentResistance, player.currentResistance * button.utilityAbility.resistanceMultiplier * battleManager.resistanceDiminishingReturns);
                dialogueText.text = player.coucouName + " gained " + (Mathf.Round((resistance - player.currentResistance) * 100f) / 100f) + " resistance";
                player.currentResistance = resistance;
                battleManager.resistanceDiminishingReturns *= 0.9f;
                yield return new WaitForSeconds(waitAfterAbility);
            }
            if (button.utilityAbility.selfMindset > 0)
            {
                dialogueText.text = player.coucouName + " gained " + (int)Mathf.Min(100 - player.currentMindset, button.utilityAbility.selfMindset) + " mindset";
                player.currentMindset += (int)button.utilityAbility.selfMindset;
                if (player.currentMindset > 100)
                {
                    player.currentMindset = 100;
                }
                yield return new WaitForSeconds(waitAfterAbility);
            }
            if (button.utilityAbility.selfDetermination > 0)
            {
                dialogueText.text = player.coucouName + " gained " + (int)Mathf.Min(100 - player.currentDetermination, button.utilityAbility.selfDetermination) + " determination";
                player.currentDetermination += (int)button.utilityAbility.selfDetermination;
                if (player.currentDetermination > 100)
                {
                    player.currentDetermination = 100;
                }
                yield return new WaitForSeconds(waitAfterAbility);
            }
            if (button.utilityAbility.enemyDetermination < 0)
            {
                int determinationAbsolute = (int)Mathf.Abs(button.utilityAbility.enemyDetermination);
                dialogueText.text = enemy.coucouName + " lost " + Mathf.Min(Mathf.Max(100 - enemy.currentDetermination, 0), determinationAbsolute) + " determination";
                enemy.currentDetermination -= (int)button.utilityAbility.enemyDetermination;
                if (enemy.currentDetermination < 0)
                {
                    enemy.currentDetermination = 0;
                }
                yield return new WaitForSeconds(waitAfterAbility);
            }
            if (button.utilityAbility.enemyDetermination > 0)
            {
                int determinationAbsolute = (int)Mathf.Abs(button.utilityAbility.enemyDetermination);
                dialogueText.text = enemy.coucouName + " gained " + Mathf.Min(100 - enemy.currentDetermination, determinationAbsolute) + " determination";
                enemy.currentDetermination += determinationAbsolute;
                if (enemy.currentDetermination > 100)
                {
                    enemy.currentDetermination = 100;
                }
                yield return new WaitForSeconds(waitAfterAbility);
            }
        }
        else
        {
            bool abilityDisadvantage = HasDisadvantage(button.attackAbility.coucouElement, enemy.element);
            bool abilityAdvantage = HasAdvantage(button.attackAbility.coucouElement, enemy.element);
            bool crit = false;
            float damageModifier = 1;
            int rnd = Random.Range(1, 101);
            Debug.Log(rnd + " + " + player.currentMindset);
            if (abilityDisadvantage)
            {
                damageModifier = 0.9f;
            }
            else if (abilityAdvantage)
            {
                damageModifier = 1.12f;
            }
            if (rnd <= player.currentMindset)
            {
                damageModifier *= 1 + (player.currentDetermination / 100f);
                crit = true;
            }
            isDead = TakeDamage(player.currentAttack * button.attackAbility.damageMultiplier * damageModifier / enemy.currentResistance);
            yield return new WaitUntil(() => takeDamageFinished);
            takeDamageFinished = false;

            if (crit)
            {
                dialogueText.text = player.coucouName + " critically hit!";
            }
            else if (abilityDisadvantage)
            {
                dialogueText.text = "The attack wasn't very outstanding...";
            }
            else if (abilityAdvantage)
            {
                dialogueText.text = "The attack was glorious!";
            }
            else
            {
                dialogueText.text = "The attack is successful";
            }
            
            yield return new WaitForSeconds(waitAfterAbility);
        }

        if (isDead)
        {
            dialogueText.text = enemy.coucouName + " has collapsed";
            enemy.hasCollapsed = true;
            Destroy(enemyInScene);
            StartCoroutine(GrantExperience(false));
            yield return new WaitWhile(() => grantingExperience);
            StartCoroutine(enemyManager.NextCouCou());
        }
        else if (enemy.isStunned)
        {
            dialogueText.text = enemy.coucouName + " is stunned";
            enemy.isStunned = false;
            yield return new WaitForSeconds(2f);
            StartCoroutine(PlayerTurn());
        }
        else
        {
            EnemyTurn();
        }
        yield break;
    }

    public bool TakeDamage(float damage)
    {
        Debug.Log(damage);

        if (damage == 0)
        {
            takeDamageFinished = true;
            return false;
        }

        if (state == BattleState.PLAYERTURN)
        {
            int afterDamageHealth = enemy.currentHealth - Mathf.RoundToInt(damage);
            StartCoroutine(IncrementallyRemoveHP(afterDamageHealth));
            if (afterDamageHealth <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (state == BattleState.ENEMYTURN)
        {
            StartCoroutine(IncrementallyRemoveHP(player.currentHealth - Mathf.RoundToInt(damage)));
            int afterDamageHealth = player.currentHealth - Mathf.RoundToInt(damage);
            if (afterDamageHealth <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            Debug.Log("How did you get here?");
            takeDamageFinished = true;
            return false;
        }
    }

    public IEnumerator IncrementallyRemoveHP(int desiredHP)
    {
        if (state == BattleState.PLAYERTURN)
        {
            float increase = enemy.currentHealth;

            while (increase != desiredHP && enemy.currentHealth > 0)
            {
                increase = Mathf.MoveTowards(increase, desiredHP, Time.deltaTime * 250f);
                enemyHealthBar.fillAmount = increase / enemy.maxHealth;
                enemyHealthText.text = (int)Mathf.Max(increase, 0) + "/" + enemy.maxHealth;
                enemy.currentHealth = (int)Mathf.Max(increase, 0);
                yield return new WaitForSeconds(0.04f);
            }
            takeDamageFinished = true;
            if (enemy.currentHealth <= 0)
            {
                enemy.currentHealth = 0;
            }
            yield break;
        }
        else if (state == BattleState.ENEMYTURN)
        {
            float increase = player.currentHealth;

            while (increase != desiredHP && player.currentHealth > 0)
            {
                increase = Mathf.MoveTowards(increase, desiredHP, Time.deltaTime * 250f);
                allyHealthBar.fillAmount = increase / player.maxHealth;
                allyHealthText.text = (int)Mathf.Max(increase, 0) + "/" + player.maxHealth;
                player.currentHealth = (int)Mathf.Max(increase, 0);
                yield return new WaitForSeconds(0.04f);
            }
            takeDamageFinished = true;
            if (player.currentHealth <= 0)
            {
                player.currentHealth = 0;
            }
            yield break;
        }
    }

    public bool HasDisadvantage(CouCouDatabase.Element attackingElement, CouCouDatabase.Element defendingElement)
    {
        switch (attackingElement)
        {
            case CouCouDatabase.Element.Flame:
                if (defendingElement == CouCouDatabase.Element.Aqua || defendingElement == CouCouDatabase.Element.Lux)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case CouCouDatabase.Element.Aqua:
                if (defendingElement == CouCouDatabase.Element.Nature || defendingElement == CouCouDatabase.Element.Lux)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case CouCouDatabase.Element.Nature:
                if (defendingElement == CouCouDatabase.Element.Flame || defendingElement == CouCouDatabase.Element.Umbral)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case CouCouDatabase.Element.Umbral:
                if (defendingElement == CouCouDatabase.Element.Flame || defendingElement == CouCouDatabase.Element.Aqua)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case CouCouDatabase.Element.Lux:
                if (defendingElement == CouCouDatabase.Element.Umbral || defendingElement == CouCouDatabase.Element.Nature)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            default:
                return false;
        }
    }

    public bool HasAdvantage(CouCouDatabase.Element attackingElement, CouCouDatabase.Element defendingElement)
    {
        switch (attackingElement)
        {
            case CouCouDatabase.Element.Flame:
                if (defendingElement == CouCouDatabase.Element.Aqua || defendingElement == CouCouDatabase.Element.Lux)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            case CouCouDatabase.Element.Aqua:
                if (defendingElement == CouCouDatabase.Element.Nature || defendingElement == CouCouDatabase.Element.Lux)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            case CouCouDatabase.Element.Nature:
                if (defendingElement == CouCouDatabase.Element.Flame || defendingElement == CouCouDatabase.Element.Umbral)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            case CouCouDatabase.Element.Umbral:
                if (defendingElement == CouCouDatabase.Element.Flame || defendingElement == CouCouDatabase.Element.Aqua)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            case CouCouDatabase.Element.Lux:
                if (defendingElement == CouCouDatabase.Element.Umbral || defendingElement == CouCouDatabase.Element.Nature)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            default:
                return false;
        }
    }

    public void ReduceMindset(int amountUsed)
    {
        float amountToBeReduced = Mathf.Pow(amountUsed, 2) - Mathf.Pow(amountUsed - 1, 2);
        if (state == BattleState.PLAYERTURN)
        {
            enemy.currentMindset -= (int)amountToBeReduced;
            dialogueText.text = enemy.coucouName + " had its Mindset reduced by " + (int)amountToBeReduced;
        }
        else if (state == BattleState.ENEMYTURN)
        {
            player.currentMindset -= (int)amountToBeReduced;
            dialogueText.text = player.coucouName + " had its Mindset reduced by " + (int)amountToBeReduced;
        }
    }

    public IEnumerator IncrementallyIncreaseHP(int desiredHP, InventoryList.CouCouInventory coucou)
    {
        float increase = coucou.currentHealth;

        while (increase != desiredHP && coucou.currentHealth < coucou.maxHealth)
        {
            increase = Mathf.MoveTowards(increase, desiredHP, Time.deltaTime * 250f);
            allyHealthBar.GetComponent<Image>().fillAmount = increase / coucou.maxHealth;
            allyHealthText.text = (int)increase + "/" + coucou.maxHealth;
            coucou.currentHealth = (int)increase;
            yield return new WaitForSeconds(0.04f);
        }
        finishedHealthIncrement = true;
        if (coucou.currentHealth >= coucou.maxHealth)
        {
            coucou.currentHealth = coucou.maxHealth;
        }
        yield break;
    }

    public IEnumerator GrantExperience(bool caught)
    {
        yield return new WaitForSeconds(2f);
        grantingExperience = true;
        inventoryManager.AddExperience(player, player.coucouLevel, enemy.coucouLevel, caught);

        yield return new WaitUntil(() => inventoryManager.experienceIncrement);

        dialogueText.text = player.coucouName + " gained " + inventoryManager.experienceGained + " experience";

        yield return new WaitForSeconds(2f);

        for (int i = 0; i < inventoryManager.partyLevelUps.Count; i++)
        {
            dialogueText.text = inventoryManager.partyLevelUps[i] + " leveled up!";
            // level up sound
            yield return new WaitForSeconds(2f);
        }
        grantingExperience = false;
    }

    public IEnumerator EndBattle()
    {
        if (state == BattleState.WON)
        {
            yield return new WaitForSeconds(2f);

            if (!enemyManager.wild)
            {
                dialogueText.text = "You defeated " + enemyManager.enemyInventory.preGameDialogue.name;
                yield return new WaitForSeconds(2f);
            }

            gameManager.SetState(GameManager.GameState.Wandering);
        }
        else if (state == BattleState.LOST)
        {
            yield return new WaitForSeconds(2f);

            if (!enemyManager.wild)
            {
                dialogueText.text = "You lost to " + enemyManager.enemyInventory.preGameDialogue.name;
                yield return new WaitForSeconds(2f);
                StartCoroutine(gameManager.BadEnding());
            }
            else
            {
                gameManager.SetState(GameManager.GameState.Wandering);
            }
        }
    }
}
