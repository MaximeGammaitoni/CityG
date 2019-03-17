using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class GazTipsEntity {
    #region PROTECTED PROPERTIES
    #endregion
    #region PUBLIC PROPERTIES
    public string rawTextToDisplay;
    #endregion
    #region PROTECTED METHODS
    #endregion
    #region PUBLIC METHODS
    #endregion
    #region CONSTRUCTORS
    public GazTipsEntity()
    {

    }
    #endregion
    #region STATIC METHODS
    public static GazTipsEntity CreateFromJSON(string p_json)
    {
        GazTipsEntity l_gazTips = new GazTipsEntity();
        Debug.LogWarning("EEUUUH"); 
        KeyValuePair<string,string> l_entity = JsonConvert.DeserializeObject<KeyValuePair<string,string>>(p_json);
        l_gazTips.rawTextToDisplay = l_entity.Value;
        return l_gazTips;
    }
    public static string ToJSON(GazTipsEntity p_gazTips)
    {
        return JsonConvert.SerializeObject(p_gazTips);
    }
    #endregion
}
