using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Catching : MonoBehaviour
{
    private BattleManager battleManager;
    private EnemyManager enemyManager;
    private BattlingUI battlingUI;
    private BattleSystem battleSystem;
    private InventoryManager inventoryManager;

    public InventoryList enemyInventory;

    public TextMeshProUGUI dialogueText;

    public bool catchSuccessful = false;

    private void Awake()
    {
        battleSystem = gameObject.GetComponent<BattleSystem>();
        battleManager = gameObject.GetComponent<BattleManager>();
        enemyManager = gameObject.GetComponent<EnemyManager>();
        inventoryManager = gameObject.GetComponent<InventoryManager>();
        battlingUI = GameObject.FindGameObjectWithTag("BattlingUI").GetComponent<BattlingUI>();
    }

    public IEnumerator CatchCouCou()
    {
        if (enemyManager.wild)
        {
            float rnd = Random.Range(1, 101 - battleSystem.enemy.currentMindset);

            Debug.Log("1: " + rnd + " mindset: " + (101 - (battleSystem.enemy.currentMindset / 2)));

            // Do animation (throw)
            yield return new WaitForSeconds(1.5f);

            rnd -= battleSystem.enemy.maxHealth / battleSystem.enemy.currentMindset / 3;

            Debug.Log("2: " + rnd);

            if (rnd < 40)
            {
                // Do animation (Glow)
                yield return new WaitForSeconds(1.5f);
            }
            else
            {
                catchSuccessful = false;
                dialogueText.text = battleSystem.enemy.coucouName + " escaped the capsule";
                yield return new WaitForSeconds(2f);
                battleSystem.EnemyTurn();
                yield break;
            }

            if (rnd < 30)
            {
                // Do animation (Glow)
                yield return new WaitForSeconds(1.5f);
            }
            else
            {
                catchSuccessful = false;
                dialogueText.text = battleSystem.enemy.coucouName + " escaped the capsule";
                yield return new WaitForSeconds(2f);
                battleSystem.EnemyTurn();
                yield break;
            }

            // Do final animation (Glow)
            catchSuccessful = true;

            dialogueText.text = "You caught " + battleSystem.enemy.coucouName + "!";

            inventoryManager.AddCouCou(battleSystem.enemy.coucouName, battleSystem.enemy.coucouLevel);
            enemyInventory.couCouInventory.Remove(battleSystem.enemy);

        }
    }
}
