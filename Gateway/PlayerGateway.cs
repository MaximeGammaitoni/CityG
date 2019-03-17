using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Database;
using UnityEngine;
using UnityEngine.UI;


public class PlayerGateway : AbstractLifeCycle {
    #region PROTECTED PROPERTIES
    protected string dataFileName = "PlayerJson.json";
    protected FireBaseClient fireBaseClient;
    #endregion
    #region PUBLIC PROPERTIES
    #endregion
    #region PROTECTED METHODS
    #region LIFECYCLE IENUMERATOR METHODS
    protected IEnumerator _Init()
    {
        E_OnStartInitializing("");
        yield return null;
        //Code Here...
        if (fireBaseClient == null)
            fireBaseClient = GameManager.singleton.fireBaseClient;
        yield return null;
        E_OnEndInitializing("");
    }
    protected IEnumerator _Load()
    {
        E_OnStartLoading("");
        yield return null;
        //Code Here...
        //ASK FOR PLAYER SNAPSHOT
        System.Threading.Tasks.Task<DataSnapshot> l_task = GET_PlayerSnapshot();
        //WAIT FOR DATA RECEPTION
        string l_debug = "Waiting for Player Snapshot WITH ID:" + fireBaseClient.getUserId();
        while (l_task.IsCompleted != true && l_task.IsCanceled != true && l_task.IsFaulted != true)
        {
            //Debug.Log(l_debug);
            //l_debug += ".";
            yield return null;
        }
        //DATA RECEIVED
        //FETCH PLAYER
        if (l_task.Result != null && l_task.Result.Exists)
        {
            GameManager.singleton.playerManager.BuildPlayerEntityFromFireBase(l_task.Result.GetRawJsonValue());
        }
        else
        {
            Debug.LogWarning("PLAYER GATEWAY -> Can't find PLAYER'S ID in Firebase");
            GameManager.singleton.playerManager.playerEntityFireBase = new PlayerEntity();
            GameManager.singleton.playerManager.userIsLoaded = true;
        }
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
    public void SubscribeToDebugLogs()
    {
        OnStartInitializing += (string p_args) =>
        {
            Debug.Log("PLAYER GATEWAY => START INITIALIZATION.");
        };
        OnEndInitializing += (string p_args) =>
        {
            Debug.Log("PLAYER GATEWAY => END INITIALIZATION.");
        };
        OnStartLoading += (string p_args) =>
        {
            Debug.Log("PLAYER GATEWAY => START LOADING.");
        };
        OnEndLoading += (string p_args) =>
        {
            Debug.Log("PLAYER GATEWAY => END LOADING.");
        };
        OnStartBeginning += (string p_args) =>
        {
            Debug.Log("PLAYER GATEWAY => START BEGINNING.");
        };
        OnEndBeginning += (string p_args) =>
        {
            Debug.Log("PLAYER GATEWAY => END BEGINNING.");
        };
        OnStartUpdating += (string p_args) =>
        {
            Debug.Log("PLAYER GATEWAY => START UPDATING.");
        };
        OnEndUpdating += (string p_args) =>
        {
            Debug.Log("PLAYER GATEWAY => END UPDATING.");
        };
        OnStartResumeing += (string p_args) =>
        {
            Debug.Log("PLAYER GATEWAY => START RESUMEING.");
        };
        OnEndResumeing += (string p_args) =>
        {
            Debug.Log("PLAYER GATEWAY => END RESUMEING.");
        };
        OnStartStoping += (string p_args) =>
        {
            Debug.Log("PLAYER GATEWAY => START STOPING.");
        };
        OnEndStoping += (string p_args) =>
        {
            Debug.Log("PLAYER GATEWAY => END STOPING.");
        };
        OnStartUnloading += (string p_args) =>
        {
            Debug.Log("PLAYER GATEWAY => START UNLOADING.");
        };
        OnEndUnloading += (string p_args) =>
        {
            Debug.Log("PLAYER GATEWAY => END UNLOADING.");
        };
        OnStartDestroying += (string p_args) =>
        {
            Debug.Log("PLAYER GATEWAY => START DESTROYING.");
        };
        OnEndDestroying += (string p_args) =>
        {
            Debug.Log("PLAYER GATEWAY => END DESTROYING.");
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

    public PlayerEntity fetchPLayerFromJson()
    {
        string jsonString = "{ \"currentLevel\": 1 ,\"trophy\":0,\"clan\":\"clanTest\",\"error\":\"Login Name\",\"gas\":0,\"level\":1,\"xp\":0.0,\"securityPoint\":0,\"wellBeingPoint\":0,\"greenGasPoint\":0,\"customerRelationshipPoint\":0,\"lastCoDate\":\"7/23/2018 3:37:35 AM\",\"lastDecoDate\":\"7/23/2018 3:37:41 AM\"}";
        return PlayerEntity.CreateFromJSON(jsonString);
    }

    public PlayerEntity SavePlayerIntoJson(PlayerEntity playerEntity)
    {
        string dataAsJson = JsonUtility.ToJson(playerEntity);
        string filePath = Application.streamingAssetsPath + "/" + dataFileName;

        if (!File.Exists(filePath))
        {
            throw new Exception("data file don't exist SAVE");
        }
        File.WriteAllText(filePath, dataAsJson);
        return playerEntity;
    }

    public PlayerEntity BuildEntityFromFireBase(string json)
    {
        return JsonUtility.FromJson<PlayerEntity>(json);
    }

    public void SaveEntityIntoFireBase(PlayerEntity playerEntity)
    {
        if (fireBaseClient.fireBaseIsInitialised)
        {
            Debug.LogWarning("Saving player from PlayerGateway");
            fireBaseClient.SavePlayerEntity(playerEntity);
        }
    }

    public Task<DataSnapshot> GET_PlayerSnapshot()
    {
        return fireBaseClient.WIP_GET_PlayerWholeData(fireBaseClient.getUserId());
    }
    public Task<DataSnapshot> GET_ExperienceTableSnapshot()
    {
        return fireBaseClient.GET_ExperienceTable();
    }
    #endregion
    #region CONSTRUCTORS
    public PlayerGateway()
    {
        SubscribeToDebugLogs();
    }
    public PlayerGateway(FireBaseClient p_firebaseClient)
    {
        SubscribeToDebugLogs();
        SubscribeToLifeCycle();
        fireBaseClient = p_firebaseClient;
    }
    #endregion
}
