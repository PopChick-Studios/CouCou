using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "Databases/Inventory", order = 5)]
public class InventoryList : ScriptableObject
{
    [System.Serializable]
    public class CouCouInventory
    {
        public string coucouName;
        public int coucouLevel;
        public float coucouEXP;
        public int coucouVariant;
        public CouCouDatabase.Element element;

        public bool isCurrentlyActive = false;
        public bool onParty = false;
        public int lineupOrder = -1;

        public int maxHealth;

        [Header("Current Stats")]
        public int currentAttack;
        public float currentResistance;
        public int currentHealth;
        public int currentMindset;
        public int currentDetermination;
        public bool isStunned = false;
        public bool hasCollapsed = false;

        [Header("Abilities")]
        public int ability1;
        public int ability2;
        public int ability3;
        public int ability4;

        // will be getting other relevant information from applying the name into a for loop to find it's matching scriptable
    }

    [System.Serializable]
    public class ItemInventory
    {
        public string itemName;
        public int itemAmount;

        public ItemsDatabase.ItemAttribute itemAttribute;
        public CouCouDatabase.Element element;
        // will be getting other relevant information from applying the name into a for loop to find it's matching scriptable
    }

    // Save these lists for save function
    public string inventoryOwner;
    public List<CouCouInventory> couCouInventory = new List<CouCouInventory>();
    public List<ItemInventory> itemInventory = new List<ItemInventory>();
}
