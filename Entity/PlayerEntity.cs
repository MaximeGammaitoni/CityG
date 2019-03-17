using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerEntity 
{
    public int trophy;
    public string clan;
    public string name;
    public float gas;
    public int level;
    public float xp;
    public int currentLevel;
    
    public int securityPoint;
    public int wellBeingPoint;
    public float greenGasPoint;
    public int customerRelationshipPoint;

    public string lastCoDate;
    public string lastDecoDate;
    public string notificationId;

    public static PlayerEntity CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<PlayerEntity>(jsonString);
    }

    public PlayerEntity()
    {
        trophy = 0;
        clan = string.Empty;
        name = string.Empty;
        gas = 0F;
        level = 1;
        xp = 0;
        currentLevel = 1;
        securityPoint = 0;
        wellBeingPoint = 0;
        greenGasPoint = 0F;
        customerRelationshipPoint = 0;
        lastCoDate = System.DateTime.Now.ToString();
        lastDecoDate = System.DateTime.Now.ToString();
        notificationId = string.Empty;
    }
}
