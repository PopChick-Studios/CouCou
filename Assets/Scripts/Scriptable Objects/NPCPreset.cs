using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NPC Database", menuName = "Databases/NPC Database", order = 4)]
public class NPCPreset : ScriptableObject
{
    [System.Serializable]
    public class NPCDatabase
    {
        public string nameOfCharacter;

        public string coucou1;
        public string coucou2;
        public string coucou3;
        public string coucou4;
        public string coucou5;
    }

    public List<NPCDatabase> npcDatabase = new List<NPCDatabase>();
}
