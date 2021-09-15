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
        while (gameManager.State == GameManager.GameState.Wandering || gameManager.State == GameManager.GameState.Fishing)
        {
            foreach (InventoryList.CouCouInventory coucou in playerInventory.couCouInventory)
            {
                if (!coucou.hasCollapsed && coucou.currentHealth != coucou.maxHealth)
                {
                    coucou.currentHealth = Mathf.Min(coucou.currentHealth + (3 * coucou.maxHealth / 100), coucou.maxHealth);
                }
            }
            yield return new WaitForSeconds(20f);
        }
    }

    public void FullPartyHeal()
    {
        foreach (InventoryList.CouCouInventory coucou in playerInventory.couCouInventory)
        {
            coucou.currentHealth = coucou.maxHealth;
            if (coucou.hasCollapsed)
            {
                coucou.hasCollapsed = false;
            }
        }
    }
}
