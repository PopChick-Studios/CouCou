using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseCouCouHealth : MonoBehaviour
{
    private GameManager gameManager;

    public InventoryList playerInventory;

    private void Awake()
    {
        gameManager = gameObject.GetComponent<GameManager>();
        StartCoroutine(IncreaseHealth());
    }

    public IEnumerator IncreaseHealth()
    {
        while (gameManager.State == GameManager.GameState.Wandering)
        {
            foreach (InventoryList.CouCouInventory coucou in playerInventory.couCouInventory)
            {
                coucou.currentHealth = Mathf.Min(coucou.currentHealth + (3 * coucou.maxHealth / 100), coucou.maxHealth);
            }

            yield return new WaitForSeconds(10f);
        }
    }
}
