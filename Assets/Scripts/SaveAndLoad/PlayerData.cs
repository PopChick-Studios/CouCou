using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int questProgress;
    public int amountOfCapsules;
    public float[] position;
    
    public PlayerData (Player player)
    {
        questProgress = player.questProgress;
        amountOfCapsules = player.amountOfCapsules;

        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;
    }
}
