using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using Newtonsoft.Json;

public class ClanGateway : AbstractLifeCycle {
    #region PROTECTED PROPERTIES
    protected string _dataFileName;
    protected FireBaseClient _fireBaseClient;
    protected Dictionary<string, IEnumerator> _coroutines;
    #endregion
    #region PUBLIC PROPERTIES
    public delegate void ClanGatewayEvent(string p_index,string p_data);
    public event ClanGatewayEvent OnMessageReceived;
    public event ClanGatewayEvent OnMessageSended;
    public event ClanGatewayEvent OnDataReceived;
    public event ClanGatewayEvent OnDataSend;
    #endregion
    #region PROTECTED METHODS
    #region LIFECYCLE IENUMERATOR METHODS
    protected IEnumerator _Init()
    {
        E_OnStartInitializing("");
        yield return null;
        //Code Here...
        if (_dataFileName == "")
            _dataFileName = "Not Assigned";
        if (_fireBaseClient == null)
            _fireBaseClient = GameManager.singleton.fireBaseClient;
        _fireBaseClient.RemoveAllListeners();
        if (_coroutines == null)
            _coroutines = new Dictionary<string, IEnumerator>();
        //SUBSCRIBE TO ALL DATA UPDATES
        System.Threading.Tasks.Task<DataSnapshot> l_clanList = GET_ClanList();
        //WAIT FOR CLAN DICTIONARY
        while (l_clanList.IsCompleted != true && l_clanList.IsFaulted != true && l_clanList.IsCanceled != true)
        {
            //Debug.Log(l_debug);
            //l_debug += ".";
            yield return null;
        }
        //CONVERT REPONSE
        if (!l_clanList.Result.GetRawJsonValue().IsNullOrEmpty())
        {
            Dictionary<string, ClanEntity> l_dictionaryClan = JsonConvert.DeserializeObject<Dictionary<string, ClanEntity>>(l_clanList.Result.GetRawJsonValue());
            foreach (KeyValuePair<string, ClanEntity> currentPair in l_dictionaryClan)
            {
                SubscribeToClanPublicPropertiesUpdate(currentPair.Value.clanID);
            }
        }
        
        ClanManager.OnIJoinedAClan += HandleIJoinedAClan;
        ClanManager.OnILeavedAClan += HandleILeavedAClan;
        yield return null;
        E_OnEndInitializing("");
    }
    protected IEnumerator _Load()
    {
        E_OnStartLoading("");
        yield return null;
            //ASK FOR CLAN SNAPSHOT
            string l_debug = "Waiting for Clan Snapshot CLAN ID:" + GameManager.singleton.playerManager.GetPlayerClan();
            
            if(!GameManager.singleton.playerManager.GetPlayerClan().IsNullOrEmpty())
            {
                //WAIT FOR DATA RECEPTION
                Debug.Log(l_debug);
                System.Threading.Tasks.Task<DataSnapshot> l_task = GET_ClanSnapshot(GameManager.singleton.playerManager.GetPlayerClan());
                while (l_task.IsCompleted != true && l_task.IsFaulted != true && l_task.IsCanceled != true)
                {
                    //Debug.Log(l_debug);
                    //l_debug += ".";
                    yield return null;
                }
                //DATA RECEIVED
                //FETCH CLAN
                if (l_task.Result != null)
                {
                    //Create Clan from JSON
                    ClanEntity l_clanReceivedFromJSON = ClanEntity.CreateFromJSON(l_task.Result.GetRawJsonValue());
                //Assign Clan to Clan Manager
                    GameManager.singleton.clanManager.clanEntityFirebase = l_clanReceivedFromJSON;
                    //GameManager.singleton.clanManager.JoinClan(l_clanReceivedFromJSON);
                    //LISTEN FOR PRIVATE EVENTS
                    SubscribeToClanPrivatePropertiesUpdate(l_clanReceivedFromJSON.clanID);
                }
                else
                {
                    Debug.LogWarning("CLAN GATEWAY -> Can't find PLAYER'S CLAN-ID in Firebase");
                    GameManager.singleton.clanManager.clanEntityFirebase = new ClanEntity();
                }
            }
            else
            {
                Debug.LogWarning("CLAN GATEWAY -> PLAYER MANAGER.GetClan() return null or empty !");
            }
        
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
    #endregion
    #region PUBLIC METHODS
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
    public void SubscribeToDebugLogs()
    {
        OnStartInitializing += (string p_args) =>
        {
            Debug.Log("CLAN GATEWAY => START INITIALIZATION.");
        };
        OnEndInitializing += (string p_args) =>
        {
            Debug.Log("CLAN GATEWAY => END INITIALIZATION.");
        };
        OnStartLoading += (string p_args) =>
        {
            Debug.Log("CLAN GATEWAY => START LOADING.");
        };
        OnEndLoading += (string p_args) =>
        {
            Debug.Log("CLAN GATEWAY => END LOADING.");
        };
        OnStartBeginning += (string p_args) =>
        {
            Debug.Log("CLAN GATEWAY => START BEGINNING.");
        };
        OnEndBeginning += (string p_args) =>
        {
            Debug.Log("CLAN GATEWAY => END BEGINNING.");
        };
        OnStartUpdating += (string p_args) =>
        {
            Debug.Log("CLAN GATEWAY => START UPDATING.");
        };
        OnEndUpdating += (string p_args) =>
        {
            Debug.Log("CLAN GATEWAY => END UPDATING.");
        };
        OnStartResumeing += (string p_args) =>
        {
            Debug.Log("CLAN GATEWAY => START RESUMEING.");
        };
        OnEndResumeing += (string p_args) =>
        {
            Debug.Log("CLAN GATEWAY => END RESUMEING.");
        };
        OnStartStoping += (string p_args) =>
        {
            Debug.Log("CLAN GATEWAY => START STOPING.");
        };
        OnEndStoping += (string p_args) =>
        {
            Debug.Log("CLAN GATEWAY => END STOPING.");
        };
        OnStartUnloading += (string p_args) =>
        {
            Debug.Log("CLAN GATEWAY => START UNLOADING.");
        };
        OnEndUnloading += (string p_args) =>
        {
            Debug.Log("CLAN GATEWAY => END UNLOADING.");
        };
        OnStartDestroying += (string p_args) =>
        {
            Debug.Log("CLAN GATEWAY => START DESTROYING.");
        };
        OnEndDestroying += (string p_args) =>
        {
            Debug.Log("CLAN GATEWAY => END DESTROYING.");
        };
    }
    public void UnsubscribeToClanDataEvents()
    {
        //GameManager.singleton.playerManager.SetPlayerClan("");
        GameManager.singleton.uiManager.messagesManager.CleanMessages();
        GameManager.singleton.uiManager.messagesManager.isInitialized = false;
        GameManager.singleton.uiManager.messagesManager.Init();
        Debug.LogWarning("UNSUBSCRIBE TO CLAN DATA EVENTS");
        _fireBaseClient.RemoveAllListeners();
        Debug.LogWarning("OnClanJoined += SubscribeToClanDataEvents");
        /*ClanManager.OnClanJoined += (key, value) => {
            SubscribeToClanDataEvents();
            GameManager.singleton.playerManager.SetPlayerClan(GameManager.singleton.clanManager.clanEntityFirebase.displayName);
            GameManager.singleton.uiManager.messagesManager.CleanMessages();
            GameManager.singleton.uiManager.messagesManager.CreateMessages(GameManager.singleton.clanManager.clanEntityFirebase.messagesList);
        };*/
        WIP_SET_RemoveMember(GameManager.singleton.clanManager.GET_ClanID(), GameManager.singleton.fireBaseClient.getUserId(),"");
    }


    public void SubscribeToClanPublicPropertiesUpdate(string p_clanName)
    {
        if (p_clanName.IsNullOrEmpty())
        {
            Debug.LogWarning("Can't subscribe to clan public events, p_clanName is null or empty");
            return;
        }
        Debug.Log("Start Listening for public properties of CLAN: " + p_clanName);
        //NAME
        ListenForChangedValue(p_clanName, "displayName", HandleSomeoneUpdatedAClanName);
        //LEVEL
        ListenForChangedValue(p_clanName, "level", HandleSomeoneUpdatedAClanLevel);
        //DESCRIPTION
        ListenForChangedValue(p_clanName, "description", HandleSomeoneUpdatedAClanDescription);
        //REGION
        ListenForChangedValue(p_clanName, "region", HandleSomeoneUpdatedAClanRegion);
        //USERS
        ListenChildFor(p_clanName, "usersList", HandleSomeoneJoinedAClan, "ADDED");
        ListenChildFor(p_clanName, "usersList", HandleSomeoneLeavedAClan, "REMOVED");
        Debug.Log("Success");
    }
    public void UnsubscribeToClanPublicPropertiesUpdate(string p_clanName)
    {
        if (p_clanName.IsNullOrEmpty())
        {
            Debug.LogWarning("Can't unsubscribe to clan public events, p_clanName is null or empty");
            return;
        }
        Debug.Log("Start removing listeners for public properties of CLAN: " + p_clanName);
        //NAME
        RemoveForChangedValueListener(p_clanName, "name");
        //LEVEL
        RemoveForChangedValueListener(p_clanName, "level");
        //DESCRIPTION
        RemoveForChangedValueListener(p_clanName, "description");
        //REGION
        RemoveForChangedValueListener(p_clanName, "region");
        //USERS
        RemoveChildForListener(p_clanName, "usersList", "ADDED");
        RemoveChildForListener(p_clanName, "usersList", "REMOVED");
        Debug.Log("Success");
    }
    public void SubscribeToClanPrivatePropertiesUpdate(string p_clanName)
    {
        if (p_clanName.IsNullOrEmpty())
        {
            Debug.LogWarning("Can't subscribe to clan private events, p_clanName is null or empty");
            return;
        }
        Debug.Log("Start Listening for private properties of CLAN: " + p_clanName);
        //NAME
        ListenForChangedValue(p_clanName, "displayName", HandleSomeoneUpdatedAClanName);
        //LEVEL
        ListenForChangedValue(p_clanName, "level", HandleSomeoneUpdatedAClanLevel);
        //DESCRIPTION
        ListenForChangedValue(p_clanName, "description", HandleSomeoneUpdatedAClanDescription);
        //REGION
        ListenForChangedValue(p_clanName, "region", HandleSomeoneUpdatedAClanRegion);
        //FOR NEW USERS
        ListenChildFor(p_clanName, "usersList", HandleSomeoneJoinedAClan, "ADDED");
        ListenChildFor(p_clanName, "usersList", HandleSomeoneLeavedAClan, "REMOVED");
        //FOR MESSAGES
        ListenChildFor(p_clanName, "messagesList", HandleSomeoneSentAMessageInMyClan, "ADDED");
        ListenChildFor(p_clanName, "messagesList", HandleSomeoneRemovedAMessageInMyClan, "REMOVED");
        Debug.Log("Success");
    }
    public void UnsubscribeToClanPrivatePropertiesUpdate(string p_clanName)
    {
        if (p_clanName.IsNullOrEmpty())
        {
            Debug.LogWarning("Can't unsubscribe to clan private properties, p_clanName is null or empty");
            return;
        }
        Debug.Log("Start removing listening for private properties of clan: " + p_clanName);
        //NAME
        RemoveForChangedValueListener(p_clanName, "displayName");
        //LEVEL
        RemoveForChangedValueListener(p_clanName, "level");
        //DESCRIPTION
        RemoveForChangedValueListener(p_clanName, "description");
        //REGION
        RemoveForChangedValueListener(p_clanName, "region");
        //USERS
        RemoveChildForListener(p_clanName, "usersList", "ADDED");
        RemoveChildForListener(p_clanName, "usersList", "REMOVED");
        //MESSAGES
        RemoveChildForListener(p_clanName, "messagesList", "ADDED");
        RemoveChildForListener(p_clanName, "messagesList", "REMOVED");
        Debug.Log("Success");
    }
    
    
    /*public void SubscribeToClanDataEvents()
    {
        GameManager.singleton.uiManager.messagesManager.CleanMessages();
        GameManager.singleton.uiManager.messagesManager.isInitialized = false;
        GameManager.singleton.uiManager.messagesManager.Init();
        Debug.LogWarning("SUBSCRIBE TO CLAN DATA EVENTS");
        /*ListenForChangedValue(GameManager.singleton.clanManager.GET_ClanName(), "displayName", (string p_key, string p_value) =>
        {
            GameManager.singleton.clanManager.SET_ClanName(p_value);
            return;
        });
        //FOR LEVEL
        ListenForChangedValue(GameManager.singleton.clanManager.GET_ClanName(), "level", (string p_key, string p_value) =>
        {
            int l_level = -1;
            int.TryParse(p_value, out l_level);
            GameManager.singleton.clanManager.SET_ClanLevel(l_level);
            return;
        });
        //FOR NEW MESSAGES
        ListenChildFor(GameManager.singleton.clanManager.GET_ClanName(), "messagesList", (string p_key, string p_value) =>
        {
            GameManager.singleton.clanManager.SET_AppEnd_Messages(p_key, p_value);
            return;
        }, "ADDED");
        ListenChildFor(GameManager.singleton.clanManager.GET_ClanName(), "messagesList", (string p_key, string p_value) =>
        {
            GameManager.singleton.clanManager.SET_Remove_Messages(p_key);
            return;
        }, "REMOVED");
        //FOR NEW USERS
        ListenChildFor(GameManager.singleton.clanManager.GET_ClanName(), "usersList", (string p_key, string p_value) =>
        {
            GameManager.singleton.clanManager.E_OnClanMemberAdded(p_key, p_value);
        }, "ADDED");
        ListenChildFor(GameManager.singleton.clanManager.GET_ClanName(), "usersList", (string p_key, string p_value) =>
        {
            GameManager.singleton.clanManager.E_OnClanMemberRemoved(p_key,p_value);
        }, "REMOVED");
        Debug.LogWarning("OnClanLeaved += UnSubscribeToClanDataEvents");
        ClanManager.OnClanLeaved += (key, value) => {
            UnsubscribeToClanDataEvents();
        };
        //WIP_SET_AppEndNewMember(GameManager.singleton.clanManager.GET_ClanName(), GameManager.singleton.fireBaseClient.getUserId(), "");
    }*/
    //WIP
    public void HandleIJoinedAClan(string p_key, string p_value)
    {
        SubscribeToClanPrivatePropertiesUpdate(p_key);
        WIP_SET_AppEndNewMember(p_key, _fireBaseClient.getUserId(), "true");
    }
    public void HandleILeavedAClan(string p_key, string p_value)
    {
        UnsubscribeToClanPrivatePropertiesUpdate(p_key);
        WIP_SET_RemoveMember(p_key, _fireBaseClient.getUserId(), "false");
    }
    //WIP
    public void HandleJoinClanEvents(string p_key, string p_value)
    {
        /*_fireBaseClient.RemoveAllListeners();
        ListenForChangedValue(GameManager.singleton.clanManager.GET_ClanName(), "displayName", (string l_key, string l_value) =>
        {
            GameManager.singleton.clanManager.SET_ClanName(l_value);
            return;
        });
        //FOR LEVEL
        ListenForChangedValue(GameManager.singleton.clanManager.GET_ClanName(), "level", (string l_key, string l_value) =>
        {
            int l_level = -1;
            int.TryParse(l_value, out l_level);
            GameManager.singleton.clanManager.SET_ClanLevel(l_level);
            return;
        });
        //FOR NEW MESSAGES
        ListenChildFor(GameManager.singleton.clanManager.GET_ClanName(), "messagesList", (string l_key, string l_value) =>
        {
            GameManager.singleton.clanManager.SET_AppEnd_Messages(l_key, l_value);
            return;
        }, "ADDED");
        ListenChildFor(GameManager.singleton.clanManager.GET_ClanName(), "messagesList", (string l_key, string l_value) =>
        {
            GameManager.singleton.clanManager.SET_Remove_Messages(l_key);
            return;
        }, "REMOVED");
        //FOR NEW USERS
        ListenChildFor(GameManager.singleton.clanManager.GET_ClanName(), "usersList", (string l_key, string l_value) =>
        {
            GameManager.singleton.clanManager.E_OnClanMemberAdded(l_key, l_value);
        }, "ADDED");
        ListenChildFor(GameManager.singleton.clanManager.GET_ClanName(), "usersList", (string l_key, string l_value) =>
        {
            GameManager.singleton.clanManager.E_OnClanMemberRemoved(l_key, l_value);
        }, "REMOVED");*/
    }
    //WIP
    public void HandleLeaveClanEvents(string p_key, string p_value)
    {
        _fireBaseClient.RemoveAllListeners();
    }
    //WIP HANDLERS
    public void HandleSomeoneCreatedAClan(string p_clanCreatedName, string p_clanCreatedJSON)
    {
        GameManager.singleton.clanManager.E_SomeoneCreatedAClan(p_clanCreatedName, p_clanCreatedJSON);
    }
    public void HandleSomeoneDeletedAClan(string p_clanDeletedName, string p_playerWhoDeletedName)
    {
        GameManager.singleton.clanManager.E_SomeoneDeletedAClan(p_clanDeletedName, p_playerWhoDeletedName);
    }
    public void NOTSUPPORTEDHandleSomeoneUpdatedAClan(string p_key, string p_value)
    {
        //GameManager.singleton.clanManager.E_SomeoneUpda(string p_key, string p_value);
    }
    public void NOTSUPPORTEDHandleMyClanKeyUpdated(string p_key, string p_value)
    {
        switch (p_key)
        {
            case "displayName":
                break;
            case "description":
                break;
            case "region":
                break;
            case "level":
                break;
            case "messagesList":
                break;
            case "usersList":
                //Debug.LogWarning("CLAN GATEWAY -> HandleClanUpdated, clan update ON but key not supported. KEY:" + p_key + " VALUE:" + p_value);
                break;
            default:
                Debug.LogWarning("CLAN GATEWAY -> HandleClanUpdated, clan update ON but key not supported. KEY:" + p_key + " VALUE:" + p_value);
                break;
        }
    }
    public void HandleSomeoneJoinedAClan(string p_clanJoined, string p_playerWhoJoinedName)
    {
        GameManager.singleton.clanManager.E_SomeoneJoinedAClan(p_clanJoined,p_playerWhoJoinedName);
    }
    public void HandleSomeoneLeavedAClan(string p_clanLeavedName, string p_playerWhoLeavedName)
    {
        GameManager.singleton.clanManager.E_SomeoneLeavedAClan(p_clanLeavedName, p_playerWhoLeavedName);
    }
    /*public void NOTSUPPORTEDHandleMyClanMemberUpdated(string p_key, string p_value)
    {
        switch (p_key)
        {
            case "name":
                GameManager.singleton.clanManager.E_OnMyClanMemberNameChanged(p_key, p_value);
                break;
            case "level":
                GameManager.singleton.clanManager.E_OnMyClanLevelChanged(p_key, p_value);
                break;
            case "trophy":
                break;
            default:
                Debug.LogWarning("CLAN GATEWAY -> HandleMemberUpdated, listener ON but key not supported. KEY:" + p_key + " VALUE:" + p_value);
                break;
        }
    }*/
    public void HandleSomeoneSentAMessageInMyClan(string p_messageKey, string p_messageContent)
    {
        GameManager.singleton.clanManager.E_SomeoneSentAMessageInMyClan(p_messageKey, p_messageContent);
    }
    public void HandleSomeoneRemovedAMessageInMyClan(string p_messageKey, string p_playerWhoRemoved)
    {
        GameManager.singleton.clanManager.E_SomeoneRemovedAMessageInMyClan(p_messageKey, p_playerWhoRemoved);
    }
    public void NOTSUPPORTEDHandleMyClanMessageUpdated(string p_key, string p_value)
    {
        //GameManager.singleton.clanManager.E_SomeoneUpda(p_key, p_value);
    }
    public void HandleSomeoneUpdatedAClanDescription(string p_clanUpdatedName, string p_clanUpdatedNewDescription)
    {
        GameManager.singleton.clanManager.E_SomeoneUpdatedAClanDescription(p_clanUpdatedName, p_clanUpdatedNewDescription);
    }
    public void HandleSomeoneUpdatedAClanName(string p_clanUpdatedName, string p_clanUpdatedNewName)
    {
        GameManager.singleton.clanManager.E_SomeoneUpdatedAClanName(p_clanUpdatedName, p_clanUpdatedNewName);
    }
    public void HandleSomeoneUpdatedAClanLevel(string p_clanUpdatedName, string p_clanUpdatedNewLevel)
    {
        GameManager.singleton.clanManager.E_SomeoneUpdatedAClanLevel(p_clanUpdatedName, p_clanUpdatedNewLevel);
    }
    public void HandleSomeoneUpdatedAClanRegion(string p_clanUpdatedName, string p_clanUpdatedRegion)
    {
        GameManager.singleton.clanManager.E_SomeoneUpdatedAClanRegion(p_clanUpdatedName, p_clanUpdatedRegion);
    }
    public void NOTSUPPORTEDHandleMyClanLevelUpdated(string p_key, string p_value)
    {
        GameManager.singleton.clanManager.E_OnMyClanLevelUpdated(p_key, p_value);
    }
    public void NOTSUPPORTEDHandleMyClanDescriptionUpdated(string p_key, string p_value)
    {
        GameManager.singleton.clanManager.E_OnMyClanDescriptionUpdated(p_key, p_value);
    }
    public void NOTSUPPORTEDHandleMyClanRegionUpdated(string p_key, string p_value)
    {
        GameManager.singleton.clanManager.E_OnMyClanRegionUpdated(p_key, p_value);
    }
    //HANDLING DATA RECEIVED
    public void OnWholeClanDataSnapshotReceived (string p_datas)
    {
        GameManager.singleton.clanManager.JoinClan(ClanEntity.CreateFromJSON(p_datas));
        GameManager.singleton.uiManager.messagesManager.CreateMessages(GameManager.singleton.clanManager.GET_messages());
    }
    //HANDLING PARTIAL DATA RECEIVED
    public void OnSpecificClanDataReceived (string p_key,string p_value)
    {
        Debug.Log("ClanGateway->OnSpecificClanDataReceived, KEY:"+p_key + " Value:" + p_value);
        if(OnDataReceived!=null)
            OnDataReceived(p_key, p_value);
    }

    /*
     *      GET SNAPSHOT
     * */
    //GET Whole Data Snapshot in Firebase
    public System.Threading.Tasks.Task<DataSnapshot> GET_ClanSnapshot (string p_clanName)
    {
        Debug.Log("ClanGateway->GET_ClanSnapshot, asking for Clan Snapshot.");
        //_fireBaseClient.GET_ClanWholeDatas(p_clanName);
        return _fireBaseClient.WIP_GET_ClanWholeDatas(p_clanName);
    }
    //GET Specific KeyValue in Firebase
    public void GET_KeyValue (string p_index)
    {
        Debug.Log("ClanGateway->GET_ClanSnapshot, asking for specific key value.");
        //OnDataReceived += AssignData;
        _fireBaseClient.GET_ClanSpecificData(GameManager.singleton.clanManager.GET_ClanID(), p_index);
    }
    public System.Threading.Tasks.Task<DataSnapshot> GET_ClanList ()
    {
        Debug.Log("ClanGateway->GET_ClanList, asking for the list of clan.");
        return _fireBaseClient.WIP_GET_ClanList();
    }

    /*
     *      REAL TIME
     * */
    //LISTENING FOR REAL TIME UPDATE
    public void ListenForChangedValue(string p_clanName, string p_key, ClanGatewayEvent p_delegate)
    {
        _fireBaseClient.ListenForChangesOf(p_clanName, p_key, p_delegate);
    }
    public void RemoveForChangedValueListener(string p_clanName, string p_key)
    {
        _fireBaseClient.RemoveForChangesOfListener(p_clanName, p_key);
    }
    /*
    public void ListenForAddedChild(string p_clanName, string p_key, ClanGatewayEvent p_delegate)
    {
        _fireBaseClient.ListenChildFor(p_clanName, p_key, p_delegate, "ADDED");
    }
    public void ListenForRemovedChild(string p_clanName, string p_key, ClanGatewayEvent p_delegate)
    {
        _fireBaseClient.ListenChildFor(p_clanName, p_key, p_delegate, "REMOVED");
    }*/
    public void ListenChildFor(string p_clanName, string p_key, ClanGatewayEvent p_delegate, string p_event)
    {
        _fireBaseClient.ListenChildFor(p_clanName, p_key, p_delegate, p_event);
    }
    public void RemoveChildForListener(string p_clanName, string p_key, string p_event)
    {
        _fireBaseClient.RemoveChildForListener(p_clanName, p_key, p_event);
    }
    /*
     *      SET DATAS
     * */
    //SET Whole Data Snapshot
    public void SET_ClanSnapshot (ClanEntity p_clan, System.Func<string> p_delegate)
    {
        _fireBaseClient.WIP_SET_ClanWholeDatas(p_clan).ContinueWith(task=> {
            p_delegate();
        });
    }
    //SET Specific KeyValue in Firebase
    public void SET_KeyValue (string p_key, string p_value)
    {
        _fireBaseClient.SET_ClanSpecificData(GameManager.singleton.clanManager.GET_ClanID(), p_key, p_value);
    }
    //SET AppEnd Value
    public void SET_AppEndValue (string p_key, string p_value)
    {
        _fireBaseClient.SET_AppEndClanSpecificData(GameManager.singleton.clanManager.GET_ClanID(), p_key, GameManager.singleton.playerManager.GetPlayerName()+"#"+p_value);
    }
    public void WIP_SET_AppEndNewMember (string p_clanName, string p_userID, string p_value)
    {
        _fireBaseClient.WIP_SET_ClanNewMember(p_clanName, p_userID);
    }
    public void WIP_SET_RemoveMember (string p_clanName, string p_userID, string p_value)
    {
        _fireBaseClient.WIP_SET_ClanRemoveMember(p_clanName, p_userID);
    }

    /*
     *      CONVERT VALUES
     * */
    //Create a new Clan Entity
    public ClanEntity ClanSnapshotToClanEntity (string p_JSON)
    {
        return ClanEntity.CreateFromJSON(p_JSON);
    }
    //Creating Clan Entity from Firebase
    public ClanEntity CreateClanEntity (string p_clanDatas)
    {
        return ClanSnapshotToClanEntity(p_clanDatas);
    }
    #endregion
    #region CONSTRUCTORS
    public ClanGateway()
    {
        SubscribeToDebugLogs();
    }
    public ClanGateway(FireBaseClient p_fbc)
    {
        SubscribeToDebugLogs();
        SubscribeToLifeCycle();
        _fireBaseClient = p_fbc;
    }
    #endregion
}