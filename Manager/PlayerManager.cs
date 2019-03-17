using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Database;
using UnityEngine;
using Newtonsoft.Json;

public class PlayerManager : AbstractLifeCycle {
    #region PROTECTED PROPERTIES
    #endregion
    #region PUBLIC PROPERTIES
    public static string playerLoginName;
    public PlayerGateway playerGateway;
    public delegate void PlayerEvent();
    public PlayerEntity playerEntityFireBase;

    public PlayerExperiencePoints playerExperiencePoints;
    public PlayerBonusesManager playerBonusesManager;
    //public int[] xpTable;
    public bool userIsLoaded = false;
    #endregion
    #region PROTECTED METHODS
    protected void HandleOnExperienceUpdated(string[] p_args)
    {
        playerEntityFireBase.xp = playerExperiencePoints.currentExperiencePoints;
        playerGateway.SaveEntityIntoFireBase(playerEntityFireBase);
    }
    protected void HandleOnLevelUpdated(string[] p_args)
    {
        playerEntityFireBase.level = playerExperiencePoints.currentLevel;
        playerGateway.SaveEntityIntoFireBase(playerEntityFireBase);
    }
    #region LIFECYCLE IENUMERATOR METHODS
    protected IEnumerator _Init()
    {
        E_OnStartInitializing("");
        yield return null;
        //Code Here...
        //XP Table tricks to make it ok for Jauge to update (y)
        //xpTable = new int[] { 11, 65, 150, 280, 480, 680, 0 };
        playerGateway.Init();
        playerExperiencePoints = new PlayerExperiencePoints();
        playerBonusesManager.Init();
        yield return new WaitUntil(() => {
            return playerGateway.currentLifeCycleState == LIFECYCLESTATE.INITIALIZED &&
            playerBonusesManager.currentLifeCycleState == LIFECYCLESTATE.INITIALIZED;
        });
        E_OnEndInitializing("");
    }
    protected IEnumerator _Load()
    {
        E_OnStartLoading("");
        yield return null;
        //Code Here...
        playerGateway.Load();
        while (playerGateway.currentLifeCycleState != LIFECYCLESTATE.LOADED)
        {
            yield return null;
        }
        //Experience Management
        Task<DataSnapshot> l_task = playerGateway.GET_ExperienceTableSnapshot();
        Func<bool> l_waiter = () =>
        {
            return (l_task.IsCompleted || l_task.IsCanceled || l_task.IsFaulted) ? true : false;
        };
        yield return new WaitUntil(l_waiter);
        //EXPERIENCE TABLE HAS BEEN RECEIVED
        if (l_task.IsCompleted)
        {
            Debug.Log(l_task.Result.GetRawJsonValue());
            int[] tmp = JsonConvert.DeserializeObject<int[]>(l_task.Result.GetRawJsonValue());

            //Convert Answer
            List<int> l_convertedAnswer = new List<int>();
            for(int i = 0; i < tmp.Length; i++)
            {
                l_convertedAnswer.Add(tmp[i]);
            }

            playerExperiencePoints.Assign((float)playerEntityFireBase.xp, playerEntityFireBase.level, l_convertedAnswer);
            playerExperiencePoints.OnExperienceUpdated += HandleOnExperienceUpdated;
            playerExperiencePoints.OnLevelUpdated += HandleOnLevelUpdated;
        }
        ClanManager.OnIJoinedAClan += HandleOnIJoinedAClan;
        ClanManager.OnILeavedAClan += HandleOnILeavedAClan;
        yield return null;
        E_OnEndLoading("");
    }
    protected IEnumerator _Begin()
    {
        E_OnStartBeginning("");
        yield return null;
        //Code Here...
        yield return null;
        E_OnEndBeginning("");
    }
    protected IEnumerator _Pause()
    {
        E_OnStartPausing("");
        yield return null;
        //Code Here...
        yield return null;
        E_OnEndPausing("");
    }
    protected IEnumerator _Update()
    {
        E_OnStartUpdating("");
        yield return null;
        //Code Here...
        yield return null;
        E_OnEndUpdating("");
    }
    protected IEnumerator _Resume()
    {
        E_OnStartResumeing("");
        yield return null;
        E_OnEndResumeing("");
    }
    protected IEnumerator _Stop()
    {
        E_OnStartStoping("");
        yield return null;
        //Code Here...
        yield return null;
        E_OnEndStoping("");
    }
    protected IEnumerator _Unload()
    {
        E_OnStartUnloading("");
        yield return null;
        //Code Here...
        yield return null;
        E_OnEndUnloading("");
    }
    protected IEnumerator _Destroy()
    {
        E_OnStartUnloading("");
        yield return null;
        //Code Here...
        yield return null;
        E_OnEndUnloading("");
    }
    #endregion
    #endregion
    #region PUBLIC METHODS
    #region SUBSCRIBES
    public void HandleOnIJoinedAClan(string p_clanName, string p_clanJSON)
    {
        if(!p_clanName.IsNullOrEmpty())
            SetPlayerClan(p_clanName);
    }
    public void HandleOnILeavedAClan(string p_clanLeaved, string p_clanJSON)
    {
        SetPlayerClan("");
    }
    public void SubscribeToDebugLogs()
    {
        OnStartInitializing += (string p_args) =>
        {
            Debug.Log("PLAYER MANAGER => START INITIALIZATION.");
        };
        OnEndInitializing += (string p_args) =>
        {
            Debug.Log("PLAYER MANAGER => END INITIALIZATION.");
        };
        OnStartLoading += (string p_args) =>
        {
            Debug.Log("PLAYER MANAGER => START LOADING.");
        };
        OnEndLoading += (string p_args) =>
        {
            Debug.Log("PLAYER MANAGER => END LOADING.");
        };
        OnStartBeginning += (string p_args) =>
        {
            Debug.Log("PLAYER MANAGER => START BEGINNING.");
        };
        OnEndBeginning += (string p_args) =>
        {
            Debug.Log("PLAYER MANAGER => END BEGINNING.");
        };
        OnStartUpdating += (string p_args) =>
        {
            Debug.Log("PLAYER MANAGER => START UPDATING.");
        };
        OnEndUpdating += (string p_args) =>
        {
            Debug.Log("PLAYER MANAGER => END UPDATING.");
        };
        OnStartResumeing += (string p_args) =>
        {
            Debug.Log("PLAYER MANAGER => START RESUMEING.");
        };
        OnEndResumeing += (string p_args) =>
        {
            Debug.Log("PLAYER MANAGER => END RESUMEING.");
        };
        OnStartStoping += (string p_args) =>
        {
            Debug.Log("PLAYER MANAGER => START STOPING.");
        };
        OnEndStoping += (string p_args) =>
        {
            Debug.Log("PLAYER MANAGER => END STOPING.");
        };
        OnStartUnloading += (string p_args) =>
        {
            Debug.Log("PLAYER MANAGER => START UNLOADING.");
        };
        OnEndUnloading += (string p_args) =>
        {
            Debug.Log("PLAYER MANAGER => END UNLOADING.");
        };
        OnStartDestroying += (string p_args) =>
        {
            Debug.Log("PLAYER MANAGER => START DESTROYING.");
        };
        OnEndDestroying += (string p_args) =>
        {
            Debug.Log("PLAYER MANAGER => END DESTROYING.");
        };
    }
    #endregion
    #region LIFECYCLE CALLS
    public override void Init()
    {
        FireRoutine(_Init());
    }
    public override void Load()
    {
        FireRoutine(_Load());
    }
    public override void Begin()
    {
        FireRoutine(_Begin());
    }
    public override void Pause()
    {
        FireRoutine(_Pause());
    }
    public override void Update()
    {
        FireRoutine(_Update());
    }
    public override void Resume()
    {
        FireRoutine(_Resume());
    }
    public override void Stop()
    {
        FireRoutine(_Stop());
    }
    public override void Unload()
    {
        FireRoutine(_Unload());
    }
    public override void Destroy()
    {
        FireRoutine(_Destroy());
    }
    #endregion
    public void BuildNewPlayer()
    {
        playerEntityFireBase = new PlayerEntity();
        
    }
    public void InitTemplateEntity()//For new players
    {
        this.playerEntityFireBase = playerGateway.fetchPLayerFromJson();
    }
    public float GetPlayerXp()
    {
        //return playerGateway.fetchPLayerFromJson().xp;
        return playerExperiencePoints.currentExperiencePoints;
        //return playerEntityFireBase.xp;
    }
    public int GetPlayerLevel()
    {
        //return playerGateway.fetchPLayerFromJson().level;
        return playerExperiencePoints.currentLevel;
        //return playerEntityFireBase.level;
    }
    public int GetPlayerTrophy()
    {
        //return playerGateway.fetchPLayerFromJson().trophy;
        return playerEntityFireBase.trophy;
    }
    public float GetPlayerGas()
    {
        //return playerGateway.fetchPLayerFromJson().gas;
        return playerEntityFireBase.gas;
    }
    public int GetPlayerSecurityPoint()
    {
        return playerEntityFireBase.securityPoint;
        //return playerGateway.fetchPLayerFromJson().securityPoint;
    }
    public int GetPlayerWellBeingPoint()
    {
        //return playerGateway.fetchPLayerFromJson().wellBeingPoint;
        return playerEntityFireBase.wellBeingPoint;
    }
    public float GetPlayerGreenGasPoint()
    {
        //return playerGateway.fetchPLayerFromJson().greenGasPoint;
        return playerEntityFireBase.greenGasPoint;
    }
    public int GetPlayerCustomerRelationShipPoint()
    {
        //return playerGateway.fetchPLayerFromJson().customerRelationshipPoint;
        return playerEntityFireBase.customerRelationshipPoint;
    }
    public string GetPlayerName()
    {
        //return playerGateway.fetchPLayerFromJson().name;
        return playerEntityFireBase.name;
    }
    public string GetPlayerClan()
    {
        //return playerGateway.fetchPLayerFromJson().clan;
        return (playerEntityFireBase.clan != null) ? playerEntityFireBase.clan:"";
    }
    public void SetPlayerClan(string p_value)
    {
        playerEntityFireBase.clan = p_value;
        if(p_value != "")
            playerGateway.SaveEntityIntoFireBase(this.playerEntityFireBase);
    }
    public string GetPlayerLastCoDate()
    {
        //return playerGateway.fetchPLayerFromJson().lastCoDate;
        return playerEntityFireBase.lastCoDate;
    }
    public string GetPlayerLastDecoDate()
    {
        //return playerGateway.fetchPLayerFromJson().lastDecoDate;
        return playerEntityFireBase.lastDecoDate;
    }
    public int GetPlayerCurrentLevel()
    {
        // TODO timoté : use if editor
        // return 1; // LEVEL USED TO DEBUG
        int l_output = (playerEntityFireBase != null) ?playerEntityFireBase.currentLevel : -1 ;
        return l_output;
    }
    public string GetNotificationId()
    {
        return playerEntityFireBase.notificationId;
    }
    public void SetNotificationId(string notificationId)
    {
        this.playerEntityFireBase.notificationId = notificationId;
        playerGateway.SaveEntityIntoFireBase(this.playerEntityFireBase);
    }
    public void SetPlayerCurrentLevel(int level)
    {
        this.playerEntityFireBase.currentLevel = level;
        playerGateway.SaveEntityIntoFireBase(this.playerEntityFireBase);
    }
    public void AddPlayerXp(double value)
    {
        playerExperiencePoints.AddExperiencePoints((float)value);
    }
    public void AddPlayerLevel(int value)
    {
        /*this.playerEntityFireBase.xp = 0;//TODO REPLACE THIS BY TRUE CALCUL
        this.playerEntityFireBase.level += value;
        playerGateway.SaveEntityIntoFireBase(this.playerEntityFireBase);*/
    }
    public void AddPlayerGreenGas(float p_value)
    {
        this.playerEntityFireBase.greenGasPoint += p_value;
        playerGateway.SaveEntityIntoFireBase(this.playerEntityFireBase);
    }
    public void SetLastDecoDate(string lastDeco)
    {
        this.playerEntityFireBase.lastDecoDate = lastDeco;
        
        playerGateway.SaveEntityIntoFireBase(this.playerEntityFireBase);
    }
    public void SetLastCoDate(string lastCo)
    {
        this.playerEntityFireBase.lastCoDate = lastCo;
        playerGateway.SaveEntityIntoFireBase(this.playerEntityFireBase);
    }
    public void SetPlayerName(string name)
    {
        this.playerEntityFireBase.name = name;
        playerLoginName = name;
        playerGateway.SaveEntityIntoFireBase(this.playerEntityFireBase);
    }
    public void SetPlayerGas(float gas)
    {
        this.playerEntityFireBase.gas = gas;
        playerGateway.SaveEntityIntoFireBase(this.playerEntityFireBase);
    }
    public void SetPlayerWithoutSave(string name)
    {
        playerLoginName = name;
        this.playerEntityFireBase.name = name;
    }
    public void ResetPlayerLevel()
    {
        this.playerEntityFireBase.level = 1;
        playerGateway.SaveEntityIntoFireBase(this.playerEntityFireBase);
    }
    public void ResetPlayerXp()
    {
        this.playerEntityFireBase.xp = 0;
        playerGateway.SaveEntityIntoFireBase(this.playerEntityFireBase);
    }
    public void BuildPlayerEntityFromFireBase(string json)
    {
        this.playerEntityFireBase = this.playerGateway.BuildEntityFromFireBase(json);
    }
    #endregion
    #region CONSTRUCTORS
    public PlayerManager (FireBaseClient p_firebaseClient)
    {
        //Subscribing to Primary events and Debug events
        SubscribeToDebugLogs();
        SubscribeToLifeCycle();
        //Creating Gateway
        playerGateway = new PlayerGateway(p_firebaseClient);
        playerBonusesManager = new PlayerBonusesManager(p_firebaseClient);
    }
    #endregion
}