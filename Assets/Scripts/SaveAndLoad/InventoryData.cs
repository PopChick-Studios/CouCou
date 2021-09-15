using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryData
{
    public string playerName;
    public InventoryList.CouCouInventory starterCouCou = new InventoryList.CouCouInventory();
    public List<InventoryList.CouCouInventory> coucouInventory = new List<InventoryList.CouCouInventory>();
    public List<InventoryList.ItemInventory> itemInventory = new List<InventoryList.ItemInventory>();

    public InventoryData (InventoryList inventory)
    {
        playerName = inventory.preGameDialogue.name;
        starterCouCou = inventory.starterCouCou;
        foreach (InventoryList.CouCouInventory c in inventory.couCouInventory)
        {
            coucouInventory.Add(c);
        }
        foreach (InventoryList.ItemInventory i in inventory.itemInventory)
        {
            itemInventory.Add(i);
        }
    }
}
