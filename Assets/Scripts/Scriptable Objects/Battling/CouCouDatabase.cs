using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "CouCou Database", menuName = "Databases/CouCou Database", order = 1)]
public class CouCouDatabase : ScriptableObject
{
    public enum Element
    {
        Normal,
        Flame,
        Nature,
        Aqua,
        Lux,
        Umbral,
        Psychic
    }

    [System.Serializable]
    public class CouCouData
    {
        public string coucouName;
        public string coucouDescription;
        public bool isStarter;
        public GameObject coucouModel;
        public GameObject coucouBattleModel;
        public Element coucouElement;

        public int coucouVariant;

        [Header("Abilities")]
        public int ability1;
        public int ability2;
        public int ability3;
        public int ability4;
    }

    [System.Serializable]
    public class CouCouVariant
    {
        public int variant;

        [Header("Attack")]
        public int attack;
        public int bonusAttack;
        public int bonusAttackPer5;

        [Header("Resistance")]
        public float resistance;
        public float bonusResistance;
        public float bonusResistancePer5;

        [Header("Health Points")]
        public int hp;
        public int bonusHP;
        public int bonusHPPer5;

        [Header("Mindset and Determination")]
        public int determination;
        public int mindset;
    }

    public List<CouCouData> coucouData = new List<CouCouData>();
    public List<CouCouVariant> coucouVariant = new List<CouCouVariant>();
}
