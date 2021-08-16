using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Item Database", menuName = "Databases/Item Database", order = 4)]
public class ItemsDatabase : ScriptableObject
{
    public enum ItemType
    {
        Basic,
        Berries
    }

    public enum ItemAttribute
    {
        Health,
        Attack,
        Resistance,
        ElementalMindset,
        Experience,
        Capsule,
        Fishing
    }

    [System.Serializable]
    public class ItemData
    {
        public string itemName;
        public string itemDescription;
        public GameObject itemModel;

        [Header("Unique Attributes")]
        public ItemType itemType;
        public CouCouDatabase.Element element;
        public ItemAttribute itemAttribute;
    }

    public List<ItemData> itemData = new List<ItemData>();
}
