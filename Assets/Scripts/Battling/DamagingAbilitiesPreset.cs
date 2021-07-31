using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Damage Ability Preset", menuName = "Scriptables/Damaging Abilities Preset", order = 1)]
public class DamagingAbilitiesPreset : ScriptableObject
{
    public string abilityName;
    public string description;

    public float damageMultiplier;

    public Color abilityColor;
}
