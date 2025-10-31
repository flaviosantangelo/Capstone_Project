using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public string currentSceneName;
    public float[] playerPosition;
    public int currentHealth; 
    public HashSet<string> collectedItemIDs; 

    public GameData(int startingHealth)
    {
        currentSceneName = "Level1"; 
        playerPosition = new float[3] { 0, 0, 0 };
        currentHealth = startingHealth;
        collectedItemIDs = new HashSet<string>();
    }
}