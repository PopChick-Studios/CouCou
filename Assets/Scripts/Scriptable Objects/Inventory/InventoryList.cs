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
        public float currentEXP;
        public int coucouVariant;
        public CouCouDatabase.Element element;

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
    }

    [System.Serializable]
    public class ItemInventory
    {
        public string itemName;
        public int itemAmount;

        public ItemsDatabase.ItemAttribute itemAttribute;
        public CouCouDatabase.Element element;
    }

    // Save these lists for save function
    public Dialogue preGameDialogue;
    public CouCouInventory starterCouCou;
    public List<CouCouInventory> couCouInventory = new List<CouCouInventory>();
    public List<ItemInventory> itemInventory = new List<ItemInventory>();
}
