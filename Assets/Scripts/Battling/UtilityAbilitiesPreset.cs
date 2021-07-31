using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Utility Ability Preset", menuName = "Scriptables/Utility Abilities Preset", order = 2)]
public class UtilityAbilitiesPreset : ScriptableObject
{
    public string abilityName;
    public string description;

    public float resistanceMultiplier = 1;
    public float selfMindset;
    public bool enemyMindset;
    public float selfDetermination;
    public float enemyDetermination;

    public Color abilityColor;
}
