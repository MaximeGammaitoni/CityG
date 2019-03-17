using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

//W.I.P /!\
public class BonusEntity
{
    #region PROTECTED PROPERTIES
    #endregion
    #region PUBLIC PROPERTIES
    public int amountUsage;
    public string description;
    public string name;
    [JsonIgnore]
    public int id;
    #endregion
    #region PROTECTED METHODS
    #endregion
    #region PUBLIC METHODS
    public static BonusEntity CreateFromJSON(string p_json)
    {
        return JsonConvert.DeserializeObject<BonusEntity>(p_json);
    }
    public static string ToJSON(BonusEntity p_bonusEntity)
    {
        return JsonConvert.SerializeObject(p_bonusEntity);
    }
    #endregion
    #region CONSTRUCTORS
    public BonusEntity()
    {

    }
    public BonusEntity(int p_id, int p_amountUsage, string p_description, string p_name)
    {
        amountUsage = p_amountUsage;
        description = p_description;
        name = p_name;
        id = p_id;
    }
    #endregion
}
//Used by PlayerBonusesGateway to convert JSON to waiting List
public class WaitingBonusEntity
{
    [JsonIgnore]
    public string keyID;

    public int bonusID;
    public DateTime receivedTime;
    public DateTime startedUnlockDate;
    public TimeSpan timeToUnlock;

    public static WaitingBonusEntity JSONToEntity (string p_JSON)
    {
        WaitingBonusEntity l_output = JsonConvert.DeserializeObject<WaitingBonusEntity>(p_JSON);
        return l_output;
    }
    public static string EntityToJSON (WaitingBonusEntity p_entity)
    {
        string l_output = JsonConvert.SerializeObject(p_entity);
        return l_output;
    }
}
//Used by PlayerBonusesGateway to convert JSON to unlocked List
public class UnlockedBonusEntity
{
    [JsonIgnore]
    public string keyID;

    public int bonusID;
    public string name;
    public string description;
    public int usageAmount;

    public static UnlockedBonusEntity JSONToEntity(string p_JSON)
    {
        UnlockedBonusEntity l_output = JsonConvert.DeserializeObject<UnlockedBonusEntity>(p_JSON);
        return l_output;
    }
    public static string EntityToJSON(UnlockedBonusEntity p_entity)
    {
        string l_output = JsonConvert.SerializeObject(p_entity);
        return l_output;
    }
}
/*
public class CityGazBonusEntity
{
    public TimeSpan timeToUnlockDuration;
    public DateTime receivedDate;
    public DateTime startedUnlockedDate;
    public DateTime unlockedDate;
    public int bonusID;
    public int usageRemaining;
}*/