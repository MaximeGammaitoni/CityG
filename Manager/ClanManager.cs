using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClanManager : AbstractLifeCycle  {
    #region PROTECTED PROPERTIES
    protected Dictionary<string, IEnumerator> _coroutines;
    #endregion
    #region PUBLIC PROPERTIES
    public ClanEntity clanEntityFirebase;
    public ClanGateway clanGateway;
    #region CLAN EVENTS
    //CLAN DATA EVENTS
    public delegate void ClanDataEvent(string p_key, string p_value);
    //MESSAGES
    public static event ClanDataEvent OnMyClanMessagesListSet;
    public static event ClanDataEvent OnMyClanNewMessageReceived;
    public static event ClanDataEvent OnMyClanMessageRemoved;
    public static event ClanDataEvent OnMyClanMessageUpdated;
    //PROPERTIES GENERAL
    public static event ClanDataEvent OnMyClanNameUpdated;
    public static event ClanDataEvent OnMyClanDescriptionUpdated;
    public static event ClanDataEvent OnMyClanRegionUpdated;
    public static event ClanDataEvent OnMyClanLevelUpdated;
    //MEMBERS
    public static event ClanDataEvent OnMyClanMembersListSet;
    public static event ClanDataEvent OnMyClanMemberRemoved;
    public static event ClanDataEvent OnMyClanMemberAdded;
    public static event ClanDataEvent OnMyClanMemberNameUpdated;
    public static event ClanDataEvent OnMyClanMemberLevelUpdated;
    //I DID
    public static event ClanDataEvent OnIJoinedAClan;
    public static event ClanDataEvent OnILeavedAClan;
    public static event ClanDataEvent OnICreatedAClan;
    public static event ClanDataEvent OnIDeletedAClan;
    public static event ClanDataEvent OnISentAMessage;
    public static event ClanDataEvent OnIUpdatedMyClanName;
    public static event ClanDataEvent OnIUpdatedMyClanLevel;
    public static event ClanDataEvent OnIUpdatedMyClanDescription;
    public static event ClanDataEvent OnIUpdatedMyClanRegion;
    //OTHER CLAN
    public static event ClanDataEvent OnSomeoneJoinedAClan;
    public static event ClanDataEvent OnSomeoneLeavedAClan;
    public static event ClanDataEvent OnSomeoneCreatedAClan;
    public static event ClanDataEvent OnSomeoneDeletedAClan;
    public static event ClanDataEvent OnSomeoneSentAMessageInMyClan;
    public static event ClanDataEvent OnSomeoneRemovedAMessageInMyClan;
    public static event ClanDataEvent OnSomeoneUpdatedAClanName;
    public static event ClanDataEvent OnSomeoneUpdatedAClanLevel;
    public static event ClanDataEvent OnSomeoneUpdatedAClanDescription;
    public static event ClanDataEvent OnSomeoneUpdatedAClanRegion;
    #endregion
    #endregion
    #region PROTECTED METHODS
    protected void AssignClanEntity(ClanEntity p_clanEntity)
    {
        if(p_clanEntity.clanID.IsNullOrEmpty())
        {
            Debug.LogWarning("TRYING TO ASSIGN CLAN ENTITY NOT SET, STOPPING ASSIGN");
            return;
        }
        Debug.Log("CLAN MANAGER->ASSIGN CLAN ENTITY:"+p_clanEntity.clanID + " OLD CLAN ENTITY NAME:"+clanEntityFirebase.clanID);
        if (!clanEntityFirebase.clanID.IsNullOrEmpty())
            UnassignClanEntity(p_clanEntity);

        clanEntityFirebase = p_clanEntity;
        E_IJoinedAClan(p_clanEntity.clanID, p_clanEntity.GetJSON());
    }
    protected void UnassignClanEntity(ClanEntity p_clanEntity)
    {
        Debug.Log("CLAN MANAGER->UNASSIGN CLAN ENTITY:" + clanEntityFirebase.clanID);
        if (!clanEntityFirebase.clanID.IsNullOrEmpty())
            E_ILeavedAClan(clanEntityFirebase.clanID, p_clanEntity.GetJSON());
        clanEntityFirebase = new ClanEntity();
        Debug.LogWarning("OnMyClanLeaved = null");
    }
    #region LIFECYCLE IENUMERATOR METHODS
    //CALLED BY PUBLIC L_Init()
    protected IEnumerator _Init()
    {
        E_OnStartInitializing("");
        yield return null;
        //Code Here...
        if (clanGateway == null)
            clanGateway = new ClanGateway();
        if(clanEntityFirebase == null)
            clanEntityFirebase = new ClanEntity();
        clanGateway.Init();
        while(clanGateway.currentLifeCycleState != LIFECYCLESTATE.INITIALIZED)
        {
            yield return null;
        }
        yield return null;
        E_OnEndInitializing("");
    }
    //CALLED BY PUBLIC L_Load()
    protected IEnumerator _Load()
    {
        E_OnStartLoading("");
        yield return null;
        //Code Here...
        clanGateway.Load();
        while (clanGateway.currentLifeCycleState != LIFECYCLESTATE.LOADED)
        {
            yield return null;
        }
        yield return null;
        E_OnEndLoading("");
    }
    //CALLED BY PUBLIC L_Begin()
    protected IEnumerator _Begin()
    {
        E_OnStartBeginning("");
        yield return null;
        E_OnEndBeginning("");
    }
    //CALLED BY PUBLIC L_Pause()
    protected IEnumerator _Pause()
    {
        E_OnStartPausing("");
        yield return null;
        E_OnEndPausing("");
    }
    //CALLED BY PUBLIC L_Update()
    protected IEnumerator _Update()
    {
        E_OnStartUpdating("");
        yield return null;
        E_OnEndUpdating("");
    }
    //CALLED BY PUBLIC L_Resume()
    protected IEnumerator _Resume()
    {
        E_OnStartResumeing("");
        yield return null;
        E_OnEndResumeing("");
    }
    //CALLED BY PUBLIC L_Stop()
    protected IEnumerator _Stop()
    {
        E_OnStartStoping("");
        yield return null;
        E_OnEndStoping("");
    }
    //CALLED BY PUBLIC L_Unload()
    protected IEnumerator _Unload()
    {
        E_OnStartUnloading("");
        yield return null;
        clanGateway.Unload();
        while(clanGateway.currentLifeCycleState != LIFECYCLESTATE.UNLOADED)
        {
            yield return null;
        }
        E_OnEndUnloading("");
    }
    //CALLED BY PUBLIC L_Destroy()
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
    #region SUBSCRIBES
    public void SubscribeToDebugLogs()
    {
        OnStartInitializing += (string p_args) =>
        {
            Debug.Log("CLAN MANAGER => START INITIALIZATION.");
        };
        OnEndInitializing += (string p_args) =>
        {
            Debug.Log("CLAN MANAGER => END INITIALIZATION.");
        };
        OnStartLoading += (string p_args) =>
        {
            Debug.Log("CLAN MANAGER => START LOADING.");
        };
        OnEndLoading += (string p_args) =>
        {
            Debug.Log("CLAN MANAGER => END LOADING.");
        };
        OnStartBeginning += (string p_args) =>
        {
            Debug.Log("CLAN MANAGER => START BEGINNING.");
        };
        OnEndBeginning += (string p_args) =>
        {
            Debug.Log("CLAN MANAGER => END BEGINNING.");
        };
        OnStartUpdating += (string p_args) =>
        {
            Debug.Log("CLAN MANAGER => START UPDATING.");
        };
        OnEndUpdating += (string p_args) =>
        {
            Debug.Log("CLAN MANAGER => END UPDATING.");
        };
        OnStartResumeing += (string p_args) =>
        {
            Debug.Log("CLAN MANAGER => START RESUMEING.");
        };
        OnEndResumeing += (string p_args) =>
        {
            Debug.Log("CLAN MANAGER => END RESUMEING.");
        };
        OnStartStoping += (string p_args) =>
        {
            Debug.Log("CLAN MANAGER => START STOPING.");
        };
        OnEndStoping += (string p_args) =>
        {
            Debug.Log("CLAN MANAGER => END STOPING.");
        };
        OnStartUnloading += (string p_args) =>
        {
            Debug.Log("CLAN MANAGER => START UNLOADING.");
        };
        OnEndUnloading += (string p_args) =>
        {
            Debug.Log("CLAN MANAGER => END UNLOADING.");
        };
        OnStartDestroying += (string p_args) =>
        {
            Debug.Log("CLAN MANAGER => START DESTROYING.");
        };
        OnEndDestroying += (string p_args) =>
        {
            Debug.Log("CLAN MANAGER => END DESTROYING.");
        };
    }
    #endregion
    #region CLAN MANAGER EVENTS FIRE
    //ON MY CLAN
    public void E_OnMyClanNameUpdated(string p_key,string p_value)
    {
        if (OnMyClanNameUpdated != null)
            OnMyClanNameUpdated(p_key, p_value);
    }
    public void E_OnMyClanDescriptionUpdated(string p_key, string p_value)
    {
        if (OnMyClanDescriptionUpdated != null)
            OnMyClanDescriptionUpdated(p_key,p_value);
    }
    public void E_OnMyClanLevelUpdated(string p_key, string p_value)
    {
        if (OnMyClanLevelUpdated != null)
            OnMyClanLevelUpdated(p_key, p_value);
    }
    public void E_OnMyClanMessagesListSet(string p_key, string p_value)
    {
        if (OnMyClanMessagesListSet != null)
            OnMyClanMessagesListSet(p_key, p_value);
    }
    public void E_OnMyClanNewMessageReceived(string p_key, string p_value)
    {
        if (OnMyClanNewMessageReceived != null)
            OnMyClanNewMessageReceived(p_key, p_value);
    }
    public void E_OnMyClanMessageRemoved(string p_key, string p_value)
    {
        if (OnMyClanMessageRemoved != null)
            OnMyClanMessageRemoved(p_key, p_value);
    }
    public void E_OnMyClanMessageUpdated(string p_key, string p_value)
    {
        if (OnMyClanMessageUpdated != null)
            OnMyClanMessageUpdated(p_key, p_value);
    }
    public void E_OnMyClanMembersListSet(string p_key, string p_value)
    {
        if (OnMyClanMembersListSet != null)
            OnMyClanMembersListSet(p_key, p_value);
    }
    public void E_OnMyClanMemberAdded(string p_key,string p_value)
    {
        if (OnMyClanMemberAdded != null)
            OnMyClanMemberAdded(p_key, p_value);
    }
    public void E_OnMyClanMemberRemoved(string p_key, string p_value)
    {
        if (OnMyClanMemberRemoved != null)
            OnMyClanMemberRemoved(p_key, p_value);
    }
    public void E_OnMyClanMemberNameUpdated(string p_key, string p_value)
    {
        if (OnMyClanMemberNameUpdated != null)
            OnMyClanMemberNameUpdated(p_key, p_value);
    }
    public void E_OnMyClanMemberLevelUpdated(string p_key, string p_value)
    {
        if (OnMyClanMemberLevelUpdated != null)
            OnMyClanMemberLevelUpdated(p_key, p_value);
    }
    public void E_OnMyClanRegionUpdated(string p_key, string p_value)
    {
        if (OnMyClanRegionUpdated != null)
            OnMyClanRegionUpdated(p_key, p_value);
    }
    //ON I DID
    public void E_IJoinedAClan(string p_clanJoinedName, string p_clanJSON)
    {
        if (OnIJoinedAClan != null)
            OnIJoinedAClan(p_clanJoinedName, p_clanJSON);
    }
    public void E_ILeavedAClan(string p_clanLeavedName, string p_clanJSON)
    {
        if (OnILeavedAClan != null)
            OnILeavedAClan(p_clanLeavedName, p_clanJSON);
    }
    //ON SOMEONE DID
    public void E_SomeoneJoinedAClan(string p_clanJoinedName, string p_playerWhoJoinedName)
    {
        if (OnSomeoneJoinedAClan != null)
            OnSomeoneJoinedAClan(p_clanJoinedName, p_playerWhoJoinedName);
    }
    public void E_SomeoneLeavedAClan(string p_clanLeavedName, string p_playerWhoLeavedName)
    {
        if (OnSomeoneLeavedAClan != null)
            OnSomeoneLeavedAClan(p_clanLeavedName, p_playerWhoLeavedName);
    }
    public void E_SomeoneCreatedAClan(string p_clanCreatedName, string p_clanEntityJSON)
    {
        if (OnSomeoneCreatedAClan != null)
            OnSomeoneCreatedAClan(p_clanCreatedName, p_clanEntityJSON);
    }
    public void E_SomeoneDeletedAClan(string p_clanDeletedName, string p_playerWhoDeletedName)
    {
        if (OnSomeoneDeletedAClan != null)
            OnSomeoneDeletedAClan(p_clanDeletedName, p_playerWhoDeletedName);
    }
    public void E_SomeoneSentAMessageInMyClan(string p_messageKey, string p_messageContent)
    {
        if (OnSomeoneSentAMessageInMyClan != null)
            OnSomeoneSentAMessageInMyClan(p_messageKey, p_messageContent);
    }
    public void E_SomeoneRemovedAMessageInMyClan(string p_messageKey, string p_playerWhoRemovedName)
    {
        if (OnSomeoneRemovedAMessageInMyClan != null)
            OnSomeoneRemovedAMessageInMyClan(p_messageKey, p_playerWhoRemovedName);
    }
    public void E_SomeoneUpdatedAClanName(string p_clanUpdatedName, string p_clanUpdatedNewName)
    {
        if (OnSomeoneUpdatedAClanName != null)
            OnSomeoneUpdatedAClanName(p_clanUpdatedName, p_clanUpdatedNewName);
    }
    public void E_SomeoneUpdatedAClanLevel(string p_clanUpdatedName, string p_clanUpdatedNewLevel)
    {
        if (OnSomeoneUpdatedAClanLevel != null)
            OnSomeoneUpdatedAClanLevel(p_clanUpdatedName, p_clanUpdatedNewLevel);
    }
    public void E_SomeoneUpdatedAClanRegion(string p_clanUpdatedName, string p_clanUpdatedNewRegion)
    {
        if (OnSomeoneUpdatedAClanRegion != null)
            OnSomeoneUpdatedAClanRegion(p_clanUpdatedName, p_clanUpdatedNewRegion);
    }
    public void E_SomeoneUpdatedAClanDescription(string p_clanUpdatedName, string p_clanUpdatedNewDescription)
    {
        if (OnSomeoneUpdatedAClanDescription != null)
            OnSomeoneUpdatedAClanDescription(p_clanUpdatedName, p_clanUpdatedNewDescription);
    }
    #endregion
    public void JoinClan(ClanEntity p_clanEntity)
    {
        AssignClanEntity(p_clanEntity);
    }
    public void LeaveClan(ClanEntity p_clanEntity)
    {
        UnassignClanEntity(p_clanEntity);
    }

    //DEBUG LOG OF THE CLAN ENTITY FORMATTED
    public void LOG_ClanEntity ()
    {
        string l_debug = "";
        l_debug += "Clan_Name : "+ GET_ClanName() + "\n";
        l_debug += "Clan_Level : " + GET_ClanLevel() + "\n";
        l_debug += "\nClan_Messages : \n";
        foreach (KeyValuePair<string,string> current in GET_messages())
        {
            l_debug += current.Value+"\n";
        }
        l_debug += "\nClan_Users : \n";
        foreach (KeyValuePair<string, string> current in GET_users())
        {
            l_debug += current.Value+"\n";
        }
        
        Debug.Log(l_debug);
    }
    #region GET/SET
    //ID
    public string GET_ClanID()
    {
        return clanEntityFirebase.clanID;
    }
    public void SET_ClanID(string p_value)
    {
        if (!p_value.IsNullOrEmpty())
        {
            clanEntityFirebase.clanID = p_value;
        }
    }
    //NAME
    public string GET_ClanName ()
    {
        return clanEntityFirebase.displayName;
    }
    public void SET_ClanName (string p_value)
    {
        if (p_value == null)
        {
            Debug.LogError("CLAN MANAGER->SET_ClanName-> VALUE= NULL, Can't set clanEntity.displayName to Null...");
            clanEntityFirebase.displayName = "ERROR";
            E_OnMyClanNameUpdated("displayName", p_value);
            return;
        }
        Debug.Log("CLAN MANAGER->SET_ClanName-> VALUE= " + p_value.ToString());
        clanEntityFirebase.displayName = p_value;
        E_OnMyClanNameUpdated("displayName", p_value);
    }
    //LEVEL
    public int GET_ClanLevel ()
    {
        return clanEntityFirebase.level;
    }
    public void SET_ClanLevel (int p_value)
    {
        if (p_value  <= 0)
        {
            Debug.LogError("CLAN MANAGER->SET_ClanLevel-> VALUE= NULL, Can't set clanEntity.level to 0 or less...");
            clanEntityFirebase.level = -1;
            E_OnMyClanLevelUpdated("level", p_value.ToString());
            return;
        }
        Debug.Log("CLAN MANAGER->SET_ClanLevel-> VALUE= " + p_value.ToString());
        clanEntityFirebase.level = p_value;
        E_OnMyClanLevelUpdated("level", p_value.ToString());
    }
    //USERS
    public Dictionary<string, string> GET_users ()
    {
        return clanEntityFirebase.usersList;
    }
    public void SET_users (Dictionary<string, string> p_value)
    {
        if (p_value == null)
        {
            Debug.LogError("CLAN MANAGER->SET_ClanUsers-> VALUE= NULL, Can't set clanEntity.users to NULL...");
            clanEntityFirebase.usersList = new Dictionary<string, string>();
            clanEntityFirebase.usersList.Add("ErrorKey", "ErrorValue");
            E_OnMyClanMembersListSet("usersList", p_value.ToString());
            return;
        }
        clanEntityFirebase.usersList = p_value;
        E_OnMyClanMembersListSet("usersList", p_value.ToString());
    }
    public void SET_AppEnd_Users (string p_key, string p_value)
    {
        GET_users().Add(p_key, p_value);
        E_OnMyClanMemberAdded(p_key, p_value);
    }
    public void SET_Remove_Users (string p_key)
    {
        GET_users().Remove(p_key);
        E_OnMyClanMemberRemoved("usersList", p_key);
    }
    //MESSAGES
    public Dictionary<string, string> GET_messages ()
    {
        return clanEntityFirebase.messagesList;
    }
    public void SET_Messages (Dictionary<string, string> p_value)
    {
        if (p_value == null)
        {
            Debug.LogError("CLAN MANAGER->SET_Messages-> VALUE= NULL, Can't set clanEntity.messagesList to NULL...");
            clanEntityFirebase.messagesList = new Dictionary<string, string>();
            clanEntityFirebase.messagesList.Add("ErrorKey", "ErrorValue");
            E_OnMyClanMessagesListSet("messagesList", p_value.ToString());
            return;
        }
        clanEntityFirebase.messagesList = p_value;
        E_OnMyClanMessagesListSet("messagesList", p_value.ToString());
    }
    public void SET_AppEnd_Messages (string p_key, string p_value)
    {
        Debug.Log("TRY SET APPEND MESSAGES KEY:" + p_key + "   VALUE:" + p_value);
        if (clanEntityFirebase.messagesList.ContainsKey(p_key))
        {
            Debug.Log("clanEntityFirebase.messagesList.ContainsKey(p_key) = true; RETURN;");
            return;
        }
            

        clanEntityFirebase.messagesList.Add(p_key,p_value);
        E_OnMyClanNewMessageReceived(p_key, p_value);
    }
    public void SET_Remove_Messages (string p_key)
    {
        GET_messages().Remove(p_key);
        E_OnMyClanMessageRemoved(p_key, "");
    }
    public void SendMessage(string p_message)
    {
        clanGateway.SET_AppEndValue("messagesList", p_message);
    }
    #endregion
    #endregion
    #region CONSTRUCTORS
    public ClanManager(FireBaseClient p_firebaseClient)
    {
        SubscribeToDebugLogs();
        SubscribeToLifeCycle();
        clanGateway = new ClanGateway(p_firebaseClient);
    }
    #endregion
}