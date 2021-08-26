using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindWildCouCou : MonoBehaviour
{
    private GameManager gameManager;

    public CouCouDatabase coucouDatabase;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    private void Start()
    {
        
    }

    public void WildCouCouFound()
    {
        gameManager.SetState(GameManager.GameState.Battling);
    }
}
