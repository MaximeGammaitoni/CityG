using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldDateEntity {

    public string id;
    public string currentDateTime;
    public string utcOffset;
    public bool isDayLightSavingsTime;
    public string dayOfTheWeek;
    public string timeZoneName;
    public long currentFileTime;
    public string ordinalDate;
    public object serviceResponse;

    public static WorldDateEntity CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<WorldDateEntity>(jsonString);
    }
}
