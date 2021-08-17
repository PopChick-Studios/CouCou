using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerPlatform;
    public Transform enemyPlatform;

    private BattleManager battleManager;
    private EnemyManager enemyManager;
    private CouCouFinder coucouFinder;

    public BattleState state;

    private void Awake()
    {
        battleManager = gameObject.GetComponent<BattleManager>();
        enemyManager = gameObject.GetComponent<EnemyManager>();

        playerPrefab = coucouFinder.FindCouCou(battleManager.activeCouCou.coucouName).coucouModel;
        enemyPrefab = coucouFinder.FindCouCou(enemyManager.enemyActiveCouCou.coucouName).coucouModel;
    }

    void Start()
    {
        state = BattleState.START;
        SetupBattle();
    }

    public void SetupBattle()
    {
        GameObject playerGO = Instantiate(playerPrefab, playerPlatform);
        GameObject enemyGO = Instantiate(enemyPrefab, enemyPlatform);
    }
}
