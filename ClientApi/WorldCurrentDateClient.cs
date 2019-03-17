using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class WorldCurrentDateClient : ClientAbstract {

    public double differenceBeetweenNowAndLastDeco;
    public delegate void WorlCurrentDateClientEvent();
    public static event WorlCurrentDateClientEvent OnDecoDateSetting;
    public float timerForAddWithLastCo = 0;
    public WorldCurrentDateClient() 
    {
        SetUrl("https://us-central1-city-gaz.cloudfunctions.net/helloWorld", "GET");
        GameManager.ApplicationOnPause += SendRequestForSetLastDecoDate;
        GameManager.ApplicationOnFocus += SendRequestForSetLastCoDate;
        GameManager.ApplicationOnFocus += SubstractDateWithLastDeco;
        GameManager.GameUpdate += UpdateTimer;
    }

    protected override WWW GetRequest()
    {
        return new WWW(url);      
    }

    public void SendRequestForSetLastCoDate()
    {
        WWW req = GetRequest();
        GameManager.singleton.StartCouroutineInGameManager(OnRequestForSaveLastCoDate(req), "OnRequestForSaveLastCoDate");
    }

    void UpdateTimer()
    {
        if (!Application.isFocused) return;
        this.timerForAddWithLastCo += Time.deltaTime;
    }

    public void SendRequestForSetLastDecoDate()
    {
        DateTime playerLastCo = DateTime.Parse(GameManager.singleton.playerManager.GetPlayerLastCoDate());
        DateTime decoDate = playerLastCo + TimeSpan.FromSeconds(timerForAddWithLastCo);
        GameManager.singleton.playerManager.SetLastDecoDate(decoDate.ToString());
        timerForAddWithLastCo = 0;
        Debug.Log("LAST DECO DATE" + decoDate);
    }

    public WorldDateEntity BuildEntityFromJson(string requestText)
    {
        string json = DeleteInvalidCharsIntoJson(requestText);
        return JsonUtility.FromJson<WorldDateEntity>(json);
    }

    public void SubstractDateWithLastDeco()
    {
        WWW req = GetRequest();
        GameManager.singleton.StartCouroutineInGameManager(OnSubstractDateWithLastDeco(req), "OnSubstractDateWithLastDeco");
    }

    public double GetDifferenceNowLastDecoAndReset()
    {
        double difference = this.differenceBeetweenNowAndLastDeco;
        return difference;
    }

    public void OnDecoSetting()
    {
       if(OnDecoDateSetting != null) OnDecoDateSetting();
    }

    IEnumerator OnSubstractDateWithLastDeco(WWW request)
    {
        yield return request;

        WorldDateEntity nowEntity = BuildEntityFromJson(request.text);
        DateTime nowDate = DateTime.Parse(nowEntity.currentDateTime) + TimeSpan.FromSeconds(System.DateTime.Now.Second); ;
        DateTime lastDecoDate = DateTime.Parse(GameManager.singleton.playerManager.GetPlayerLastDecoDate());
        TimeSpan difference = nowDate - lastDecoDate;
        Debug.Log("difference: " + difference.TotalSeconds );
        this.differenceBeetweenNowAndLastDeco = difference.TotalSeconds;
        OnDecoSetting();
        GameManager.singleton.StopCouroutineInGameManager("OnSubstractDateWithLastDeco");

    }

    IEnumerator OnRequestForSaveLastCoDate(WWW request)
    {
        yield return request;
        WorldDateEntity wd = BuildEntityFromJson(request.text);
        DateTime loadedDate = DateTime.Parse(wd.currentDateTime);
        loadedDate += TimeSpan.FromSeconds(System.DateTime.Now.Second);
        GameManager.singleton.playerManager.SetLastCoDate(loadedDate.ToString());
        Debug.Log("LAST CO DATE" + loadedDate);
        GameManager.singleton.StopCouroutineInGameManager("OnRequestForSaveLastCoDate");
    }
}
