using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public InventoryList.CouCouInventory player;
    public InventoryList.CouCouInventory enemy;
    public bool takeDamageFinished = false;

    public TextMeshProUGUI dialogueText;
    public Image allyHealthBar;
    public TextMeshProUGUI allyHealthText;
    public Image enemyHealthBar;
    public TextMeshProUGUI enemyHealthText;
    public Transform playerPlatform;
    public Transform enemyPlatform;

    public int psychicAbilitiesUsed;

    private BattleManager battleManager;
    private EnemyManager enemyManager;
    private CouCouFinder coucouFinder;
    private BattlingUI battlingUI;

    public BattleState state;

    private void Awake()
    {
        battleManager = gameObject.GetComponent<BattleManager>();
        enemyManager = gameObject.GetComponent<EnemyManager>();
        battlingUI = GameObject.FindGameObjectWithTag("BattlingUI").GetComponent<BattlingUI>();

        //playerPrefab = coucouFinder.FindCouCou(battleManager.activeCouCou.coucouName).coucouModel;
        //enemyPrefab = coucouFinder.FindCouCou(enemyManager.enemyActiveCouCou.coucouName).coucouModel;
    }

    void Start()
    {
        state = BattleState.START;
        SetupBattle();
    }

    public void SetupBattle()
    {
        //GameObject playerGO = Instantiate(playerPrefab, playerPlatform);
        //GameObject enemyGO = Instantiate(enemyPrefab, enemyPlatform);

        state = BattleState.PLAYERTURN;
        StartCoroutine(PlayerTurn());
    }

    IEnumerator PlayerTurn()
    {
        dialogueText.text = "Choose an option...";
        yield return new WaitForSeconds(2f);
        battlingUI.OnNewRound();
        yield break;
    }

    public IEnumerator EnemyTurn(float damage, string ability)
    {
        dialogueText.text = enemy.coucouName + " used " + ability;

        bool isDead = TakeDamage(damage);
        Debug.Log("takeDamageFinished = " + takeDamageFinished);
        yield return new WaitUntil(() => takeDamageFinished);
        Debug.Log("takeDamageFinished=true");
        takeDamageFinished = false;

        if (isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            state = BattleState.PLAYERTURN;
            StartCoroutine(PlayerTurn());
        }
        yield break;
    }

    public void OnButtonCall(AbilityUID button)
    {
        dialogueText.text = player.coucouName + " used " + button.GetComponentInChildren<TextMeshProUGUI>().text;
        battlingUI.OnFinishTurn();
        StartCoroutine(UseAbility(button));
    }

    public IEnumerator UseAbility(AbilityUID button)
    {
        yield return new WaitForSeconds(1f);

        int abilityUID = button.abilityUID;
        bool isUtility = button.isUtility;

        if (isUtility)
        {
            if (button.utilityAbility.enemyMindset)
            {
                psychicAbilitiesUsed++;
                // Decrease enemy mindset
            }
            if (button.utilityAbility.canStun)
            {
                // set is stunned to true
            }
        }
        else
        {
            bool isDead = TakeDamage(player.currentAttack * button.attackAbility.damageMultiplier);
            Debug.Log("takeDamageFinished = " + takeDamageFinished);
            yield return new WaitUntil(() => takeDamageFinished);
            Debug.Log("takeDamageFinished=true");
            takeDamageFinished = false;

            dialogueText.text = "The attack is successful";

            yield return new WaitForSeconds(1f);

            if (isDead)
            {
                state = BattleState.WON;
                EndBattle();
            }
            else
            {
                state = BattleState.ENEMYTURN;
                enemyManager.WildCouCouAttack();
            }
        }
        yield break;
    }

    public void EndBattle()
    {
        Debug.Log("Battle Ended");
        if (state == BattleState.WON)
        {
            dialogueText.text = "You Won!!";
        }
        else if (state == BattleState.LOST)
        {
            dialogueText.text = player.coucouName + " has fainted.";
        }
    }

    public bool TakeDamage(float damage)
    {
        Debug.Log(damage);

        if (state == BattleState.PLAYERTURN)
        {
            StartCoroutine(IncrementallyRemoveHP(enemy.currentHealth - Mathf.RoundToInt(damage)));
            int afterDamageHealth = enemy.currentHealth - Mathf.RoundToInt(damage);
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
            return false;
        }
    }

    public IEnumerator IncrementallyRemoveHP(int desiredHP)
    {
        if (state == BattleState.PLAYERTURN)
        {
            float increase = enemy.currentHealth;

            while (increase != desiredHP && player.currentHealth > 0)
            {
                increase = Mathf.MoveTowards(increase, desiredHP, Time.deltaTime * 500f);
                enemyHealthBar.fillAmount = increase / enemy.maxHealth;
                enemyHealthText.text = (int)increase + "/" + enemy.maxHealth;
                enemy.currentHealth = (int)increase;
                yield return new WaitForSeconds(0.02f);
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
                increase = Mathf.MoveTowards(increase, desiredHP, Time.deltaTime * 500f);
                allyHealthBar.fillAmount = increase / player.maxHealth;
                allyHealthText.text = (int)increase + "/" + player.maxHealth;
                player.currentHealth = (int)increase;
                yield return new WaitForSeconds(0.02f);
            }
            takeDamageFinished = true;
            if (player.currentHealth <= 0)
            {
                player.currentHealth = 0;
            }
            yield break;
        }
    }
}
