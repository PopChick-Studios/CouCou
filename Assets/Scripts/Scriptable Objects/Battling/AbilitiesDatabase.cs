using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Abilities Database", menuName = "Databases/Abilities Database", order = 2)]
public class AbilitiesDatabase : ScriptableObject
{
    public enum AbilityType
    {
        Attack,
        Utility
    }

    [System.Serializable]
    public class AttackAbilityData
    {
        public string abilityName;
        public int uniqueIdentifier;
        public string description;
        public CouCouDatabase.Element coucouElement;

        [Header("Damage")]
        public float damageMultiplier;

        [Header("Colour")]
        public Color abilityColor;
    }

    [System.Serializable]
    public class UtilityAbilityData
    {
        public string abilityName;
        public int uniqueIdentifier;
        public string description;
        public CouCouDatabase.Element coucouElement;

        public float resistanceMultiplier = 1;

        [Header("Mindset")]
        public float selfMindset;
        public bool enemyMindset;

        [Header("Determination")]
        public float selfDetermination;
        public float enemyDetermination;

        [Header("Colour")]
        public Color abilityColor;

        [Header("Special Abilities")]
        public bool canStun;
    }

    public List<AttackAbilityData> attackAbilities = new List<AttackAbilityData>();
    public List<UtilityAbilityData> utilityAbilities = new List<UtilityAbilityData>();
}
