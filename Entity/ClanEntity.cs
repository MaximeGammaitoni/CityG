using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

[System.Serializable]
public class ClanEntity {
    #region PROTECTED PROPERTIES
    #endregion
    #region PUBLIC PROPERTIES
    public int level;
    public string displayName;
    public string description;
    public string regionName;
    public Dictionary<string,string> usersList;
    public Dictionary<string, string> messagesList;
    public string clanID;
    #endregion
    #region PROTECTED METHODS
    #endregion
    #region PUBLIC METHODS
    public static ClanEntity CreateFromJSON(string p_json)
    {
        ClanEntity l_output;
        if (p_json == "")
        {
            l_output = new ClanEntity();
        }
        else
        {
            try
            {
                l_output = JsonConvert.DeserializeObject<ClanEntity>(p_json);
            }
            catch(Exception e)
            {
                return new ClanEntity();
            }
            
        }
        return l_output;
    }
    public string GetJSON ()
    {
        string l_output = JsonConvert.SerializeObject(this);
        return l_output;
    }
    public bool isValid()
    {
        bool l_output = true;
        if (displayName.IsNullOrEmpty())
            l_output = false;
        if (level < 0)
            l_output = false;
        if (clanID.IsNullOrEmpty())
            l_output = false;
        return l_output;
    }
    #endregion
    #region CONSTRUCTORS
    public ClanEntity ()
    {
        level = 1;
        displayName = "";
        description = "";
        regionName = "";
        usersList = new Dictionary<string, string>();
        messagesList = new Dictionary<string, string>();
    }
    #endregion
}