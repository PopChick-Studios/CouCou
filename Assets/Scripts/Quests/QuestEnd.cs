using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestEnd : MonoBehaviour
{
    private GameManager gameManager;
    private Player player;

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    public void EndQuest()
    {
        //StartCoroutine(gameManager.CompleteQuest(player.questProgress));
    }
}
