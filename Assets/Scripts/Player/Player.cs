using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public InventoryList playerInventory;
    public QuestScriptable questScriptable;

    public int questProgress;
    public int subquestProgress;
    public int amountOfCapsules;

    private void Awake()
    {
        questProgress = questScriptable.questProgress;
        subquestProgress = questScriptable.subquestProgress;
        foreach (InventoryList.ItemInventory item in playerInventory.itemInventory)
        {
            if (item.itemName == "CouCou Capsule")
            {
                amountOfCapsules = item.itemAmount;
            }
        }
    }

    public int MaxCapsules()
    {
        switch (questProgress)
        {
            case 1:
                return 10;

            case 2:
                goto case 1;

            case 3:
                return 15;

            default:
                return 26;
        }
    }

    public bool HasMaxCapsules()
    {
        if (amountOfCapsules == MaxCapsules())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
