using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using Firebase.Database;

public class PlayerBonusesGateway : AbstractLifeCycle
{
    #region EVENTS
    public delegate void PlayerBonusesGatewayEvent(string[] p_args);
    //GET
        //LIST
    public event PlayerBonusesGatewayEvent OnWaitingListReceived;
    public event PlayerBonusesGatewayEvent OnUnlockedListReceived;
    //SET
        //ADDED
    public event PlayerBonusesGatewayEvent OnWaitingListAddedBonus;
    public event PlayerBonusesGatewayEvent OnUnlockedListAddedBonus;
        //REMOVED
    public event PlayerBonusesGatewayEvent OnWaitingListRemovedBonus;
    public event PlayerBonusesGatewayEvent OnUnlockedListRemovedBonus;
        //UPDATE
    public event PlayerBonusesGatewayEvent OnWaitingListUpdatedBonus;
    public event PlayerBonusesGatewayEvent OnUnlockedListUpdatedBonus;
    #endregion
    #region PROTECTED PROPERTIES
    protected FireBaseClient _firebaseClient;
    #endregion
    #region PUBLIC PROPERTIES
    #endregion
    #region PROTECTED METHODS
    protected void E_OnWaitingListReceived(string[] p_args = null)
    {
        if (OnWaitingListReceived != null)
        {
            OnWaitingListReceived(p_args);
        }
    }
    protected void E_OnUnlockedListReceived(string[] p_args = null)
    {
        if (OnUnlockedListReceived != null)
        {
            OnUnlockedListReceived(p_args);
        }
    }
    protected void E_OnWaitingListAddedBonus(string[] p_args = null)
    {
        if (OnWaitingListAddedBonus != null)
        {
            OnWaitingListAddedBonus(p_args);
        }
    }
    protected void E_OnUnlockedListAddedBonus(string[] p_args = null)
    {
        if (OnUnlockedListAddedBonus != null)
        {
            OnUnlockedListAddedBonus(p_args);
        }
    }
    protected void E_OnWaitingListRemovedBonus(string[] p_args = null)
    {
        if (OnWaitingListRemovedBonus != null)
        {
            OnWaitingListRemovedBonus(p_args);
        }
    }
    protected void E_OnUnlockedListRemovedBonus(string[] p_args = null)
    {
        if (OnUnlockedListRemovedBonus != null)
        {
            OnUnlockedListRemovedBonus(p_args);
        }
    }
    protected void E_OnWaitingListUpdatedBonus(string[] p_args = null)
    {
        if (OnWaitingListUpdatedBonus != null)
        {
            OnWaitingListUpdatedBonus(p_args);
        }
    }
    protected void E_OnUnlockedListUpdatedBonus(string[] p_args = null)
    {
        if (OnUnlockedListUpdatedBonus != null)
        {
            OnUnlockedListUpdatedBonus(p_args);
        }
    }
    #region LIFECYCLE IENUMERATOR METHODS
    protected IEnumerator _Init()
    {
        E_OnStartInitializing("");
        yield return null;
        //Code Here...
        //GET_WAITING
        ASK_PlayerWaitingList();
        //GET_UNLOCKED
        ASK_PlayerUnlockedList();
        yield return null;
        E_OnEndInitializing("");
    }
    protected IEnumerator _Load()
    {
        E_OnStartLoading("");
        yield return null;
        //CODE HERE...
        yield return null;
        E_OnEndLoading("");
    }
    protected IEnumerator _Begin()
    {
        E_OnStartBeginning("");
        yield return null;
        E_OnEndBeginning("");
    }
    protected IEnumerator _Pause()
    {
        E_OnStartPausing("");
        yield return null;
        E_OnEndPausing("");
    }
    protected IEnumerator _Update()
    {
        E_OnStartUpdating("");
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
        E_OnEndUnloading("");
    }
    #endregion
    protected IEnumerator _ASK_PlayerWaitingList()
    {
        //GET
        Task<DataSnapshot> l_task = _firebaseClient.GET_UserBonusesWaitingList();
        yield return new WaitUntil(()=> { return l_task.IsCanceled || l_task.IsFaulted || l_task.IsCompleted; });
        if (l_task.IsCompleted && l_task.Result!=null &&l_task.Result.Exists)
        {
            //CONVERT
            Dictionary<string, WaitingBonusEntity> l_tmpDictionaryResult = JsonConvert.DeserializeObject<Dictionary<string, WaitingBonusEntity>>(l_task.Result.GetRawJsonValue());
            //TREAT RESULT
            List<CityGazBonus> l_listToAssign = new List<CityGazBonus>();
            CityGazBonus l_tmpCityGazBonus;
            foreach (KeyValuePair<string, WaitingBonusEntity> current in l_tmpDictionaryResult)
            {
                l_tmpCityGazBonus = new CityGazBonus(
                        CityGazBonus.CreateAbstractBonus(current.Value.bonusID), //Bonus
                        new CityGazBonusTimer(current.Value.timeToUnlock, current.Value.startedUnlockDate.Subtract(DateTime.Now)),  //Timer
                        new CityGazBonusLock(true)  //Lock
                    );

                l_tmpCityGazBonus.GET_Bonus().SET_KEYID(current.Key);
                l_tmpCityGazBonus.GET_Bonus().SET_ReceivedDateTime(current.Value.receivedTime);
                l_tmpCityGazBonus.GET_Lock().SET_StartedUnlockDateTime(current.Value.startedUnlockDate);

                Debug.Log("TMP CityGazBonus WaitingList => NAME :" + l_tmpCityGazBonus.GET_Bonus().GET_Name() + "   Locked :" + l_tmpCityGazBonus.GET_Lock().GET_Locked() + "  TimeStart:" + l_tmpCityGazBonus.GET_Bonus().GET_ReceivedDateTime());

                if (l_tmpCityGazBonus != null)
                    l_listToAssign.Add(l_tmpCityGazBonus); //ADDED 
            }
            //TELL
            E_OnWaitingListReceived();
            //ASSIGN
            GameManager.singleton.playerManager.playerBonusesManager.SET_WaitingList(l_listToAssign);
        }        
    }
    protected IEnumerator _ASK_PlayerUnlockedList()
    {
        Task<DataSnapshot> l_task = _firebaseClient.GET_UserBonusesUnlockedList();
        yield return new WaitUntil(() => { return l_task.IsCanceled || l_task.IsFaulted || l_task.IsCompleted; });
        if (l_task.IsCompleted && l_task.Result != null && l_task.Result.Exists)
        {
            //CONVERT
            Dictionary<string, UnlockedBonusEntity> l_tmpDictionaryResult = JsonConvert.DeserializeObject<Dictionary<string, UnlockedBonusEntity>>(l_task.Result.GetRawJsonValue());
            //TREAT
            List<CityGazBonus> l_listToAssign = new List<CityGazBonus>();
            CityGazBonus l_tmpCityGazBonus;
            foreach (KeyValuePair<string,UnlockedBonusEntity> current in l_tmpDictionaryResult)
            {
                l_tmpCityGazBonus = new CityGazBonus(
                        CityGazBonus.CreateAbstractBonus(current.Value.bonusID, current.Value.usageAmount), //Bonus
                        null,  //Timer
                        new CityGazBonusLock(false));  //Lock

                l_tmpCityGazBonus.GET_Bonus().SET_KEYID(current.Key);

                if (l_tmpCityGazBonus!=null)
                    l_listToAssign.Add(l_tmpCityGazBonus); //ADDED
            }
            //TELL
            E_OnUnlockedListReceived();
            //ASSIGN
            GameManager.singleton.playerManager.playerBonusesManager.SET_UnlockedList(l_listToAssign);
        }
    }
    protected IEnumerator _TELL_AddBonusToPlayerWaitingList(CityGazBonus p_bonus)
    {
        WaitingBonusEntity l_waitingBonusEntity = new WaitingBonusEntity();
        l_waitingBonusEntity.bonusID = p_bonus.GET_Bonus().GET_ID();
        l_waitingBonusEntity.receivedTime = p_bonus.GET_Bonus().GET_ReceivedDateTime();
        l_waitingBonusEntity.startedUnlockDate = DateTime.MinValue;
        l_waitingBonusEntity.timeToUnlock = p_bonus.GET_Timer().GET_BaseTime();
        l_waitingBonusEntity.keyID = p_bonus.GET_Bonus().GET_KEYID();

        Debug.Log("WaitingBonusEntity=> ID:" + l_waitingBonusEntity.bonusID + " / ReceivedTime:" + l_waitingBonusEntity.receivedTime.ToString() + " / startUnlockDate:" + l_waitingBonusEntity.startedUnlockDate.ToString() + " / timeToUnlock:" + l_waitingBonusEntity.timeToUnlock.ToString());

        Task l_task = _firebaseClient.SET_UserBonusWaitingListAddBonus(WaitingBonusEntity.EntityToJSON(l_waitingBonusEntity));

        yield return new WaitUntil(()=> { return l_task.IsCanceled || l_task.IsFaulted || l_task.IsCompleted; });
        if (l_task.IsCompleted)
        {
            E_OnWaitingListAddedBonus();
            Debug.Log("TELL ADD BONUS TO PLAYER WAITING LIST => COMPLETED.");
        }
        
    }
    protected IEnumerator _TELL_AddBonusToPlayerUnlockedList(CityGazBonus p_bonus)
    {
        UnlockedBonusEntity l_unlockedBonusEntity = new UnlockedBonusEntity();
        l_unlockedBonusEntity.bonusID = p_bonus.GET_Bonus().GET_ID();
        l_unlockedBonusEntity.description = p_bonus.GET_Bonus().GET_Description();
        l_unlockedBonusEntity.usageAmount = p_bonus.GET_Bonus().GET_AmountUsage();
        l_unlockedBonusEntity.keyID = p_bonus.GET_Bonus().GET_KEYID();

        Debug.Log("UnlockedBonusEntity=> ID:" + l_unlockedBonusEntity.bonusID + " / Description:" + l_unlockedBonusEntity.description + " / Amount Usage :" + l_unlockedBonusEntity.usageAmount);

        Task l_task = _firebaseClient.SET_UserBonusUnlockedListAddBonus(l_unlockedBonusEntity.bonusID.ToString(), UnlockedBonusEntity.EntityToJSON(l_unlockedBonusEntity));

        yield return new WaitUntil(()=> { return l_task.IsCanceled || l_task.IsFaulted || l_task.IsCompleted; });
        if (l_task.IsCompleted)
        {
            TELL_RemoveBonusFromPlayerWaitingList(p_bonus.GET_Bonus().GET_KEYID());
            E_OnUnlockedListAddedBonus(new string[1] { p_bonus.GET_Bonus().GET_KEYID()});
            Debug.Log("TELL ADD BONUS TO PLAYER UNLOCKED LIST => COMPLETED");
        }
        
    }
    protected IEnumerator _TELL_RemoveBonusFromPlayerWaitingList(string p_bonusKEYID)
    {
        Task l_task = _firebaseClient.SET_UserBonusWaitingListRemoveBonus(p_bonusKEYID);
        yield return new WaitUntil(() => { return l_task.IsCompleted||l_task.IsFaulted||l_task.IsCanceled; });
        if (l_task.IsCompleted)
        {
            E_OnWaitingListRemovedBonus();
            Debug.Log("Waiting List Bonus Removed => COMPLETED");
        }
    }
    protected IEnumerator _TELL_RemoveBonusFromPlayerUnlockedList(string p_bonusKEYID)
    {
        Task l_task = _firebaseClient.SET_UserBonusUnlockedListRemoveBonus(p_bonusKEYID);
        yield return new WaitUntil(() => { return l_task.IsCompleted||l_task.IsFaulted||l_task.IsCanceled; });
        if (l_task.IsCompleted)
        {
            E_OnUnlockedListRemovedBonus();
            Debug.Log("Unlocked List Bonus Removed => COMPLETED");
        }
        
    }
    //Maybe should be legacy ?
    protected IEnumerator _TELL_StartTimerDateTimeForPlayerWaitingList(string p_bonusKEYID)
    {
        Dictionary<string, object> l_properties = new Dictionary<string, object>();
        l_properties["startedUnlockDate"] = DateTime.Now.ToString();
        Task l_task = _firebaseClient.SET_UserBonusWaitingListUpdateBonus(p_bonusKEYID, l_properties);
        yield return new WaitUntil(() => { return l_task.IsCompleted || l_task.IsFaulted || l_task.IsCanceled; });
        if (l_task.IsCompleted)
        {
            E_OnWaitingListUpdatedBonus(new string[3] { p_bonusKEYID, "startedUnlockDate", l_properties["startedUnlockDate"].ToString() });
            Debug.Log("Waiting List Bonus Updated (startedUnlockDate)=> Completed, BONUS KEYID:" + p_bonusKEYID);
        }
    }
    protected IEnumerator _TELL_UpdateBonusInPlayerWaitingList(string p_bonusKEYID, Dictionary<string,object> p_propertiesToUpdate)
    {
        Task l_task = _firebaseClient.SET_UserBonusWaitingListUpdateBonus(p_bonusKEYID, p_propertiesToUpdate);
        yield return new WaitUntil(() => { return l_task.IsCompleted || l_task.IsFaulted || l_task.IsCanceled; });
        if (l_task.IsCompleted)
        {
            E_OnWaitingListUpdatedBonus(new string[1] { p_bonusKEYID });
            Debug.Log("Waiting List Bonus Updated => Completed, BONUS KEYID:" + p_bonusKEYID);
        }
    }
    protected IEnumerator _TELL_UpdateBonusInPlayerUnlockedList(string p_bonusKEYID, Dictionary<string,object> p_propertiesToUpdate)
    {
        Task l_task = _firebaseClient.SET_UserBonusUnlockedListUpdateBonus(p_bonusKEYID, p_propertiesToUpdate);
        yield return new WaitUntil(() => { return l_task.IsCompleted || l_task.IsFaulted || l_task.IsCanceled; });
        if (l_task.IsCompleted)
        {
            E_OnUnlockedListUpdatedBonus(new string[1] { p_bonusKEYID });
            Debug.Log("Unlocked List Bonus Updated => Completed, BONUS KEYID:" + p_bonusKEYID);
        }
    }
    #endregion
    #region PUBLIC METHODS
    public void SubscribeToDebugLogs()
    {
        OnStartInitializing += (string p_args) =>
        {
            Debug.Log("BONUS GATEWAY => START INITIALIZATION.");
        };
        OnEndInitializing += (string p_args) =>
        {
            Debug.Log("BONUS GATEWAY => END INITIALIZATION.");
        };
        OnStartLoading += (string p_args) =>
        {
            Debug.Log("BONUS GATEWAY => START LOADING.");
        };
        OnEndLoading += (string p_args) =>
        {
            Debug.Log("BONUS GATEWAY => END LOADING.");
        };
        OnStartBeginning += (string p_args) =>
        {
            Debug.Log("BONUS GATEWAY => START BEGINNING.");
        };
        OnEndBeginning += (string p_args) =>
        {
            Debug.Log("BONUS GATEWAY => END BEGINNING.");
        };
        OnStartUpdating += (string p_args) =>
        {
            Debug.Log("BONUS GATEWAY => START UPDATING.");
        };
        OnEndUpdating += (string p_args) =>
        {
            Debug.Log("BONUS GATEWAY => END UPDATING.");
        };
        OnStartResumeing += (string p_args) =>
        {
            Debug.Log("BONUS GATEWAY => START RESUMEING.");
        };
        OnEndResumeing += (string p_args) =>
        {
            Debug.Log("BONUS GATEWAY => END RESUMEING.");
        };
        OnStartStoping += (string p_args) =>
        {
            Debug.Log("BONUS GATEWAY => START STOPING.");
        };
        OnEndStoping += (string p_args) =>
        {
            Debug.Log("BONUS GATEWAY => END STOPING.");
        };
        OnStartUnloading += (string p_args) =>
        {
            Debug.Log("BONUS GATEWAY => START UNLOADING.");
        };
        OnEndUnloading += (string p_args) =>
        {
            Debug.Log("BONUS GATEWAY => END UNLOADING.");
        };
        OnStartDestroying += (string p_args) =>
        {
            Debug.Log("BONUS GATEWAY => START DESTROYING.");
        };
        OnEndDestroying += (string p_args) =>
        {
            Debug.Log("BONUS GATEWAY => END DESTROYING.");
        };
    }
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
    //GET
        //LISTS
    public void ASK_PlayerWaitingList()
    {
        GameManager.singleton.StartCouroutineInGameManager(_ASK_PlayerWaitingList(), "Ask_PlayerWaitingList");      
    }
    public void ASK_PlayerUnlockedList()
    {
        GameManager.singleton.StartCouroutineInGameManager(_ASK_PlayerUnlockedList(), "Ask_UnlockedList");
    }
    //SET
        //ADD
    public void TELL_AddBonusToPlayerWaitingList(CityGazBonus p_bonus)
    {
        //CANT BE USED SIMULTANEOUSLY WITH ANOTHER CALL TO TELL_AddBonusToPlayerWaitingList
        //Because of GameManager coroutine management
        GameManager.singleton.StartCouroutineInGameManager(_TELL_AddBonusToPlayerWaitingList(p_bonus), "Tell_AddBonusToPlayerWaitingList");
    }
    public void TELL_AddBonusToPlayerUnlockedList(CityGazBonus p_bonus)
    {
        //CANT BE USED SIMULTANEOUSLY WITH ANOTHER CALL TO TELL_AddBonusToPlayerUnlockedList
        //Because of GameManager coroutine management
        GameManager.singleton.StartCouroutineInGameManager(_TELL_AddBonusToPlayerUnlockedList(p_bonus), "Tell_AddBonusToPlayerUnlockedList");
    }
        //REMOVE
    public void TELL_RemoveBonusFromPlayerWaitingList(string p_bonusKEYID)
    {
        GameManager.singleton.StartCouroutineInGameManager(_TELL_RemoveBonusFromPlayerWaitingList(p_bonusKEYID), "TELL_RemoveBonusFromPlayerWaitingList_"+p_bonusKEYID);
    }
    public void TELL_RemoveBonusFromPlayerUnlockedList(string p_bonusKEYID)
    {
        GameManager.singleton.StartCouroutineInGameManager(_TELL_RemoveBonusFromPlayerUnlockedList(p_bonusKEYID), "TELL_RemoveBonusFromPlayerUnlockedList_"+p_bonusKEYID);
    }
        //UPDATE
    public void TELL_PlayerBonusWaitingListUpdated(string p_bonusKEYID, Dictionary<string,object> p_propertiesToUpdate)
    {
        GameManager.singleton.StartCouroutineInGameManager(_TELL_UpdateBonusInPlayerWaitingList(p_bonusKEYID, p_propertiesToUpdate), "TELL_PlayerBonusWaitingListUpdated_" + p_bonusKEYID);
    }
    public void TELL_PlayerBonusUnlockedListUpdated(string p_bonusKEYID, Dictionary<string,object> p_propertiesToUpdate)
    {
        GameManager.singleton.StartCouroutineInGameManager(_TELL_UpdateBonusInPlayerUnlockedList(p_bonusKEYID,p_propertiesToUpdate),"TELL_PlayerBonusUnlockedListUpdated_"+p_bonusKEYID);
    }
            // SPECIFIC WITH MANAGEMENT (LEGACY ?)
    public void TELL_StartTimerDateTimeForPlayerWaitingList(string p_bonusKEYID)
    {
        /*Dictionary<string, string> l_properties = new Dictionary<string, string>();
        l_properties["startedUnlockDate"] = DateTime.Now.ToString();
        GameManager.singleton.StartCouroutineInGameManager(_TELL_UpdateBonusInPlayerWaitingList(p_bonusKEYID,l_properties), "TELL_StartTimerDateForPlayerWaitingList_" + p_bonusKEYID);
        */
        GameManager.singleton.StartCouroutineInGameManager(_TELL_StartTimerDateTimeForPlayerWaitingList(p_bonusKEYID), "TELL_StartTimerDateTimeForPlayerWaitingList_" + p_bonusKEYID);
    }
    public void TELL_BonusTotallyConsumed(string p_bonusKEYID)
    {
        TELL_RemoveBonusFromPlayerUnlockedList(p_bonusKEYID);
    }
    public void TELL_BonusPartiallyConsumed(string p_bonusKEYID, string p_newAmountUsageValue)
    {
        Dictionary<string, object> l_properties = new Dictionary<string, object>();
        l_properties["usageAmount"] = p_newAmountUsageValue;
        _TELL_UpdateBonusInPlayerUnlockedList(p_bonusKEYID, l_properties);
    }
    public void TELL_BonusUnlocked(CityGazBonus p_bonus)
    {
        TELL_RemoveBonusFromPlayerWaitingList(p_bonus.GET_Bonus().GET_KEYID());
        TELL_AddBonusToPlayerUnlockedList(p_bonus);
    }
    public void TELL_BonusReceived(CityGazBonus p_bonus)
    {
        TELL_AddBonusToPlayerWaitingList(p_bonus);
    }
    public void TELL_BonusStartUnlocking(string p_bonusKEYID)
    {
        GameManager.singleton.StartCouroutineInGameManager(_TELL_StartTimerDateTimeForPlayerWaitingList(p_bonusKEYID), "TELL_StartTimerDateTimeForPlayerWaitingList_" + p_bonusKEYID);
    }
    #endregion
    #region CONSTRUCTORS
    public PlayerBonusesGateway(FireBaseClient p_firebaseClient)
    {
        SubscribeToLifeCycle();
        SubscribeToDebugLogs();
        _firebaseClient = p_firebaseClient;
    }
    #endregion
}