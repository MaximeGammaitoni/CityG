using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Newtonsoft.Json;

using Firebase.Analytics;

//TODO wrong way/refacto : is a manager not a client 
public class FireBaseClient  {
    
    DependencyStatus dependencyStatus;
    protected bool isCreate = false;
    public bool fireBaseIsInitialised = false;
    public string currentUserId = PlayerPrefs.GetString("currentUserId", null);
    protected string jsonToGet;
    Dictionary<string, System.Object> allValues;
    #region USERS
    protected DatabaseReference _playersTableReference;
    #endregion
    #region CLANS PUBLIC VALUES
    protected DatabaseReference _clansPublicValues;
    protected DatabaseReference _clansTotalGreenGaz;
    protected DatabaseReference _clansName;
    #endregion
    #region CLAN PROPERTIES
    //The RealTime Database's reference (Root reference)
    protected DatabaseReference _currentRealTimeDatabase = FirebaseDatabase.DefaultInstance.RootReference;
    //The Clan's table reference in the Firebase Database
    protected DatabaseReference _clanTableReference;
    //The User's Clan reference in the FireBase Database
    protected DatabaseReference _userClanReference;
    //The Active Listeners
    protected Dictionary<string, FirebaseListenerValueChanged> _activeListenersValueChanged;
    protected Dictionary<string, FirebaseListenerChildChanged> _activeListenersChildChanged;
    #endregion
    #region GAZ TIPS
    protected DatabaseReference _gazTipsTableReference;
    #endregion
    #region EXPERIENCE_TABLE
    protected DatabaseReference _experienceTableReference;
    #endregion
    #region BONUSES
    protected DatabaseReference _bonusesTemplateReference;
    protected DatabaseReference _userBonusesReference;
    #endregion

    public FireBaseClient()
    {
        _activeListenersValueChanged = new Dictionary<string, FirebaseListenerValueChanged>();
        _activeListenersChildChanged = new Dictionary<string, FirebaseListenerChildChanged>();
        fireBaseIsInitialised = false;
        GameManager.SystemOnInit += StartCheckingForNewEntity; 
        allValues = new Dictionary<string, System.Object>();
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError(
                  "Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }
    protected virtual void InitializeFirebase()
    {
        FirebaseApp app = FirebaseApp.DefaultInstance;
        Debug.Log("Initiating Firebase");
        app.SetEditorDatabaseUrl("https://city-gaz.firebaseio.com/");
        _playersTableReference = FirebaseDatabase.DefaultInstance.RootReference.Child("users");
        _clanTableReference = FirebaseDatabase.DefaultInstance.RootReference.Child("DEBUG_BRANCH_CLANS");
        _gazTipsTableReference = FirebaseDatabase.DefaultInstance.RootReference.Child("DEBUG_BRANCH_GAZTIPS");
        _experienceTableReference = FirebaseDatabase.DefaultInstance.RootReference.Child("DEBUG_XP_TABLE");
        _bonusesTemplateReference = FirebaseDatabase.DefaultInstance.RootReference.Child("DEBUG_BRANCH_BONUSES");
        _userBonusesReference = FirebaseDatabase.DefaultInstance.RootReference.Child("DEBUG_BRANCH_USERS_BONUSES");
        _clansPublicValues = FirebaseDatabase.DefaultInstance.RootReference.Child("DEBUG_BRANCH_CLANS_PUBLIC_DATAS");
        _clansTotalGreenGaz = _clansPublicValues.Child("CLANS_TOTALGREENGAZ");
        _clansName = _clansPublicValues.Child("CLANS_NAME");
        //app.SetEditorDatabaseUrl(app.Options.DatabaseUrl);
        fireBaseIsInitialised = true;
        Debug.Log("Firebase is READY");   
    }
    
    /** 
        User management 
    **/
    public string getUserId()
    {
        Debug.Log("FirebaseClient->GetUserID()-> PlayerPrefs'currentUserId': "+PlayerPrefs.GetString("currentUserId", "NOT ASSIGNED !"));
        if (PlayerPrefs.HasKey("currentUserId"))
        {
            return PlayerPrefs.GetString("currentUserId", null);
        }
        else
        {
            string l_output = FirebaseDatabase.DefaultInstance.RootReference.Child("users").Push().Key;
            Debug.LogWarning("FirebaseClient->GetUserID()->Creating new Key ID : "+l_output);
            PlayerPrefs.SetString("currentUserId", l_output);
            return l_output;
        }
    }
    public Task CreateNewUser(PlayerEntity p_newPlayer)
    {
        string l_json = JsonUtility.ToJson(p_newPlayer);
        if(p_newPlayer.name == "NOT ASSIGNED")
        {
            Debug.LogError("FIREBASE CLIENT-> CreateNewUser, player name is not assigned.");
            return null;
        }
        string l_playerID = getUserId();

        // TODO timoté : is it the best place to log the analytics event of "account creation"
        FirebaseAnalytics.SetUserId(l_playerID);
        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventSignUp);

        return FirebaseDatabase.DefaultInstance.RootReference.Child("users").Child(l_playerID).SetRawJsonValueAsync(l_json);
    }
    
    public void BuildAndSavePlayerEntityAndTriggerAllUiManagerEvent(string login)
    {
        //open loadingScreen
        if (!fireBaseIsInitialised)
        {
            Debug.LogWarning("Try to connect into firebase before initialisation");
            return;
        }
        string playerKey = getUserId();

        FirebaseDatabase.DefaultInstance.RootReference
        .Child("users").Child(playerKey)
        .GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("LOGIN NAME DOESNT EXIST");
                Debug.LogError(task.Exception.ToString());
            }
            else if (task.IsCompleted)
            {
                Debug.LogWarning("Success creating or getting user");
                DataSnapshot snapshot = task.Result;
                this.jsonToGet = snapshot.GetRawJsonValue();
                Debug.Log(this.jsonToGet);

                if (jsonToGet != null)
                {
                    GameManager.singleton.playerManager.BuildPlayerEntityFromFireBase(this.jsonToGet);
                }
                else 
                {
                    FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventSignUp);
                }
                
                FirebaseAnalytics.SetUserId(playerKey);

                FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLogin);

                GameManager.singleton.playerManager.SetPlayerWithoutSave(login);

                Debug.LogWarning("Saving player from FireBaseClient");
                SavePlayerEntity(GameManager.singleton.playerManager.playerEntityFireBase);
            }
        });
    }
    public Task<DataSnapshot> WIP_GET_PlayerWholeData(string p_playerID)
    {
        Task<DataSnapshot> l_output;
        // TODO timoté : is it the best place to log the analytics event of "login"
        FirebaseAnalytics.SetUserId(p_playerID);
        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLogin);
        
        l_output = FirebaseDatabase.DefaultInstance.RootReference.Child("users").Child(p_playerID).GetValueAsync();
        return l_output;
    }
    public void BuildPlayerEntity(string playerId)
    {
        if (!fireBaseIsInitialised)
        {
            Debug.LogError("Try to connect into firebase before initialisation");
            return;
        }
        string playerKey = getUserId();
        FirebaseDatabase.DefaultInstance.RootReference
        .Child("users").Child(playerKey)
        .GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("LOGIN NAME DOESNT EXIST");
                Debug.LogError(task.Exception.ToString());
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                this.jsonToGet = snapshot.GetRawJsonValue();
                Debug.Log(jsonToGet);
                if (jsonToGet.IsNullOrEmpty())
                {
                    if(GameManager.singleton.playerManager.playerEntityFireBase==null)
                        GameManager.singleton.playerManager.BuildNewPlayer();
                }
                else
                {
                    GameManager.singleton.playerManager.BuildPlayerEntityFromFireBase(this.jsonToGet);
                }
                GameManager.singleton.playerManager.userIsLoaded = true;
                if (GameManager.singleton.uiManager != null)
                {
                    GameManager.singleton.uiManager.TriggerAll();
                    GameManager.singleton.uiManager.LoadingIsComplete();
                }
            }
        });
    }
    
    public void SavePlayerEntity(PlayerEntity playerEntity)
    {
        if (!fireBaseIsInitialised)
        {
            if (!fireBaseIsInitialised)
                Debug.LogWarning("Try to connect into firebase before initialisation");
            return;
        }
        string json = JsonUtility.ToJson(playerEntity);

        string playerKey = getUserId();
        
        FirebaseDatabase.DefaultInstance.RootReference
            .Child("users").Child(playerKey)
            .SetRawJsonValueAsync(json).ContinueWith(task => {
                if (task.IsFaulted)
                {
                    Debug.LogError(task.Exception.ToString());
                }
                else if (task.IsCompleted)
                {
                    Debug.LogWarning("Saving user to PlayerPrefs");
                    PlayerPrefs.SetString("currentUserId", playerKey);
                    PlayerPrefs.Save();
                    GameManager.singleton.uiManager.TriggerAll();
                }
                
        });
    }
    private void StartCheckingForNewEntity()
    {
        GameManager.singleton.StartCouroutineInGameManager(CheckForNewEntity(), "CheckForNewEntity");
    }

    //TODO Maybe isn't usefull : build player entity don't need a coroutine
    IEnumerator CheckForNewEntity()
    {
        while (true)
        {
            while (!isCreate)
            {
                yield return 0;
            }
            Debug.Break();
            GameManager.singleton.playerManager.BuildPlayerEntityFromFireBase(this.jsonToGet);
            this.isCreate = false;

            if (GameManager.singleton.uiManager != null)
            {
                GameManager.singleton.uiManager.TriggerAll();
                GameManager.singleton.uiManager.LoadingIsComplete();
            }
               
            yield return 0;
        }
    }
    #region GAZ TIPS
    public Task<DataSnapshot> GET_GazTipsList()
    {
        return _gazTipsTableReference.GetValueAsync();
    }
    #endregion

    #region EXPERIENCE TABLE
    public Task<DataSnapshot> GET_ExperienceTable()
    {
        return _experienceTableReference.GetValueAsync();
    }
    #endregion

    #region BONUSES MANAGEMENT
    //GET
    public Task<DataSnapshot> GET_TemplateBonusInformations(string p_id)
    {
        return _bonusesTemplateReference.Child(p_id).GetValueAsync();
    }
    public Task<DataSnapshot> GET_UserBonusesWaitingList(string p_userID = null)
    {
        if (p_userID.IsNullOrEmpty())
            p_userID = getUserId();
        return _userBonusesReference.Child(p_userID).Child("waitingList").GetValueAsync();
    }
    public Task<DataSnapshot> GET_UserBonusesUnlockedList(string p_userID = null)
    {
        if (p_userID.IsNullOrEmpty())
            p_userID = getUserId();
        return _userBonusesReference.Child(p_userID).Child("unlockedList").GetValueAsync();
    }
    //SET
        //LIST
    public Task SET_UserBonusesWaitingList(string p_JSON, string p_userID = null)
    {
        return _userBonusesReference.Child(p_userID).Child("waitingList").SetRawJsonValueAsync(p_JSON);
    }
    public Task SET_UserBonusesUnlockedList(string p_JSON, string p_userID = null)
    {
        return _userBonusesReference.Child(p_userID).Child("unlockedList").SetRawJsonValueAsync(p_JSON);
    }
        //ADD
    public Task SET_UserBonusWaitingListAddBonus(string p_bonusJSON, string p_userID = null)
    {
        if (p_userID.IsNullOrEmpty())
        {
            p_userID = getUserId();
        }
        return _userBonusesReference.Child(p_userID).Child("waitingList").Push().SetRawJsonValueAsync(p_bonusJSON);
    }
    public Task SET_UserBonusUnlockedListAddBonus(string p_bonusID, string p_bonusJSON, string p_userID = null)
    {
        if (p_userID.IsNullOrEmpty())
        {
            p_userID = getUserId();
        }
        return _userBonusesReference.Child(p_userID).Child("unlockedList").Push().SetRawJsonValueAsync(p_bonusJSON);
    }
        //REMOVE
    public Task SET_UserBonusWaitingListRemoveBonus(string p_bonusKEYID, string p_userID = null)
    {
        if (p_userID.IsNullOrEmpty())
        {
            p_userID = getUserId();
        }
        return _userBonusesReference.Child(p_userID).Child("waitingList").Child(p_bonusKEYID).RemoveValueAsync();
    }
    public Task SET_UserBonusUnlockedListRemoveBonus(string p_bonusKEYID, string p_userID = null)
    {
        if (p_userID.IsNullOrEmpty())
        {
            p_userID = getUserId();
        }
        return _userBonusesReference.Child(p_userID).Child("unlockedList").Child(p_bonusKEYID).RemoveValueAsync();
    }
        //UPDATE
    public Task SET_UserBonusWaitingListUpdateBonus(string p_bonusKEYID, Dictionary<string,object> p_bonusPropertiesToUpdate, string p_userID = null)
    {
        if (p_userID.IsNullOrEmpty())
        {
            p_userID = getUserId();
        }
        return _userBonusesReference.Child(p_userID).Child("waitingList").Child(p_bonusKEYID).UpdateChildrenAsync(p_bonusPropertiesToUpdate);
    }
    public Task SET_UserBonusUnlockedListUpdateBonus(string p_bonusKEYID, Dictionary<string,object> p_bonusPropertiesToUpdate, string p_userID = null)
    {
        if (p_userID.IsNullOrEmpty())
        {
            p_userID = getUserId();
        }
        return _userBonusesReference.Child(p_userID).Child("unlockedList").Child(p_bonusKEYID).UpdateChildrenAsync(p_bonusPropertiesToUpdate);
    }
    #endregion

    #region CLAN PUBLIC VALUES
    #region TOTAL GREEN GAZ
    //GET
    public Task<DataSnapshot> GET_ClansTotalGreenGaz()
    {
        return _clansTotalGreenGaz.GetValueAsync();
    }
    //SET
    public Task SET_PublicValues_TotalGreenGaz(string p_clanKeyID, string p_value)
    {
        return null;
    }
    #endregion
    #region NAME
    public Task<DataSnapshot> GET_ClansName()
    {
        return _clansName.GetValueAsync();
    }
    public Task SET_PublicValues_ClanName(string p_clanKeyID, string p_value)
    {
        return null;
    }
    #endregion
    #endregion

    #region CLAN MANAGEMENT
    /*
     *                          Guide d'utilisation des méthodes de gestion de données du modèle "CLAN"
     * 
     *      - Comment récupérer l'ensemble des données d'un clan ?
     *          - Quelqu'un demande les informations d'un clan à la CLAN GATEWAY et s'abonne à la réception des données.
     *          - La CLAN GATEWAY demande au client qu'elle connaît de récupérer les informations (GET_ClanSnapshot)
     *          - Le Client (FIREBASECLIENT) utilise la méthode GET_ClanWholeDatas(string leNomDuClanDontOnVeutLesInfos) pour interroger la BDD.
     *          - Le Client (FIREBASECLIENT) renvoi le résultat à la CLAN GATEWAY à travers la méthode OnWholeClanDataSnapshotReceived(string leJSONDuClan).
     *          - La CLAN GATEWAY informe ses abonnées des résultats obtenus
     *          
     *      - Comment récupérer une partie des données d'un clan ?
     *          - Quelqu'un demande une information spécifique pour un clan à la CLAN GATEWAY et s'abonne à la réception des données.
     *          - La CLAN GATEWAY demande au client qu'elle connaît de récupérer les informations (GET_KeyValue)
     *          - Le Client (FIREBASECLIENT) utilise la méthode GET_ClanSpecificData(string leNomDuClanDontOnVeutLesInfos, string laCléDeLaValeur) pour interroger la BDD.
     *          - Le Client (FIREBASECLIENT) renvoi le résultat à la CLAN GATEWAY à travers la méthode OnSpecificClanDataReceived(string laCléDeLaValeur, string laValeur).
     *          - La CLAN GATEWAY informe ses abonnées des résultats obtenus.
     * 
     *      - Comment envoyer l'ensemble des données d'un clan ?
     *          - Quelqu'un demande d'envoyer l'ensemble des informations d'un clan à la CLAN GATEWAY et s'abonne à l'état d'avancement de l'envoi des données.
     *          - La CLAN GATEWAY demande au client qu'elle connaît d'envoyer les informations (SET_ClanSnapshot)
     *          - Le Client (FIREBASECLIENT) utilise la méthode SET_ClanWholeDatas(string leNomDuClanDontOnVeutInscrireLesInfos) pour inscrire les informations dans la BDD.
     *          - Le Client (FIREBASECLIENT) renvoi le résultat de l'opération à la CLAN GATEWAY à travers la méthode OnWholeClanDataSnapshotSended(string leRésultat).
     *          - La CLAN GATEWAY informe ses abonnées du résultat de l'opération.
     *          
     *      - Comment envoyer une paire clé/valeur d'un clan ?
     *          - Quelqu'un demande d'envoyer un ensemble clé/valeur correspondant à une propriété du modèle de données d'un clan à la CLAN GATEWAY.
     *          - La CLAN GATEWAY demande au client qu'elle connaît d'envoyer les informations (AssignData)
     *          - Le Client (FIREBASECLIENT) utilise la méthode SET_ClanSpecificData(string leNomDuClanAAssignerLaValeur, string laCléDeLaPropriété, string laValeurAAssigner) pour inscrire les informations dans la BDD
     *          - Le Client (FIREBASECLIENT) renvoi le résultat de l'opération à la CLAN GATEWAY à travers la méthode OnSpecificClanDataSended(string leRésultat)
     *          - La CLAN GATEWAY informe ses abonnées du résultat de l'opération
     * 
     *      - Comment écouter en temps réel un élément du modèle de données "Clan" ?
     *          - Quelqu'un demande d'écouter le changement de valeur d'un élément du modèle de données Clan à la CLAN GATEWAY.
     *          - La CLAN GATEWAY demande au client qu'elle connaît de s'abonner aux modifications apportées à l'élément (ListenForChangedValueOf) dans la BDD.
     *          - Le Client (FIREBASECLIENT) utilise la méthode AddListenerForValueChanged(DBRef laTableContenantLElement, string leNomDeLElement,string leNomDuListener, GatewayEvent laDelegateAExecuterLorsqueLaValeurAChangé)
     *          - Le Listener (FIREBASELISTENERVALUECHANGED) assigne une delegate a exécuter au changement, cette delegate contiendra des informations sur le changement apporté pour pouvoir rester à jour (exemple, le nouveau nom du joueur).
     * */
    public string getClanId()
    {
        string l_userId = GameManager.singleton.playerManager.GetPlayerClan();
        return l_userId;
    }
    public Task CreateClan(ClanEntity p_clanToCreate)
    {
        if (p_clanToCreate.clanID.IsNullOrEmpty())
            p_clanToCreate.clanID = _clanTableReference.Push().Key;
        if (p_clanToCreate.isValid())
        {
            Debug.LogError("FIREBASE CLIENT -> CreateClan clanJSON:" + p_clanToCreate.GetJSON());
            return _clanTableReference.Child(p_clanToCreate.clanID).SetRawJsonValueAsync(p_clanToCreate.GetJSON());
        }
        Debug.Log("Can't create clan to firebase");
        return null;
    }
    public Task DeleteClan(string p_clanToDelete)
    {
        if (!p_clanToDelete.IsNullOrEmpty())
        {
            Debug.LogError("FIREBASE CLIENT -> DeleteClan clanToDelete:" + p_clanToDelete);
            return _clanTableReference.Child(p_clanToDelete).RemoveValueAsync();
        }
        Debug.Log("Can't delete Clan from firebase");
        return null;
    }
    public Task JoinClan(string p_clanToJoin)
    {
        if (!p_clanToJoin.IsNullOrEmpty())
            return _clanTableReference.Child(p_clanToJoin).Child("usersList").Child(getUserId()).SetValueAsync("true");
        Debug.LogError("FIREBASE CLIENT -> JoinClan clanToJoin:" + p_clanToJoin + " playerID:" + getUserId());
        return null;
    }
    public Task LeaveClan(string p_clanToLeave)
    {
        if (!p_clanToLeave.IsNullOrEmpty())
            return _clanTableReference.Child(p_clanToLeave).Child("usersList").Child(getUserId()).RemoveValueAsync();
        Debug.LogError("FIREBASE CLIENT -> LeaveClan clanToLeave:" + p_clanToLeave + " playerID:" + getUserId());
        return null;
    }
    public Task UpdateClan(string p_clanToUpdate, Dictionary<string,object> p_childsToUpdate)
    {
        if (!p_clanToUpdate.IsNullOrEmpty())
            return _clanTableReference.Child(p_clanToUpdate).UpdateChildrenAsync(p_childsToUpdate);
        Debug.LogError("FIREBASE CLIENT -> UpdateClan clanToUpdate:" + p_clanToUpdate + " childsToUpdate:" + p_childsToUpdate.ToString());
        return null;
    }
    public Task SetClanName(string p_oldClanName, string p_newClanName)
    {
        if(!p_newClanName.IsNullOrEmpty() && !p_oldClanName.IsNullOrEmpty())
        {
            return _clanTableReference.Child(p_oldClanName).Child("displayName").SetValueAsync(p_newClanName);
        }
        Debug.LogError("FIREBASE CLIENT -> SetClanName oldClanName:" + p_oldClanName + " newClanName:" + p_newClanName);
        return null;
    }
    public Task SetClanLevel(string p_clanName, string p_clanLevelToSet)
    {
        if (!p_clanName.IsNullOrEmpty() && !p_clanLevelToSet.IsNullOrEmpty())
        {
            return _clanTableReference.Child(p_clanName).Child("level").SetValueAsync(p_clanLevelToSet);
        }
        Debug.LogError("FIREBASE CLIENT -> SetClanLevel clanName:" + p_clanName + " clanLevelToSet:" + p_clanLevelToSet);
        return null;
    }
    public Task SendMessage(string p_clanToSendMessage, string p_messageContent)
    {
        string l_datekey = DateTime.Now.ToString("yyyyMMddHHmmss");
        l_datekey = string.Join("", l_datekey.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
        l_datekey = Regex.Replace(l_datekey, @"[^A-Za-z0-9]+", ""); ;
        if (!p_clanToSendMessage.IsNullOrEmpty() && !p_messageContent.IsNullOrEmpty())
            return _clanTableReference.Child(p_clanToSendMessage).Child(l_datekey).SetValueAsync(p_messageContent);
        Debug.LogError("FIREBASE CLIENT-> SendMessage clanToSendMessage:" + p_clanToSendMessage + " messageContent:" + p_messageContent);
        return null;
    }
    #region GET CLAN DATAS
    //GET WHOLE DATAS FOR A GIVEN CLAN
    public Task<DataSnapshot> WIP_GET_ClanList()
    {
        return _clanTableReference.GetValueAsync();
    }
    public Task<DataSnapshot> WIP_GET_PlayersList()
    {
        return _playersTableReference.GetValueAsync();
    }
    public Task<DataSnapshot> WIP_GET_ClanWholeDatas(string p_clanName)
    {
        return _clanTableReference.Child(p_clanName).GetValueAsync();
    }
    public void GET_ClanWholeDatas(string p_clanName)
    {
        _clanTableReference.Child(p_clanName).GetValueAsync().ContinueWith(task => {
            //FAULTED
            if (task.IsFaulted)
            {
                //Handle error :(
                Debug.LogError("FireBaseClient->GET_ClanWholeDatas has encountered a problem.");
            }
            //CANCELED
            if (task.IsCanceled)
            {
                //Handle cancelation :(
                Debug.LogWarning("FireBaseClient->GET_ClanWholeDatas has been canceled.");
            }
            //COMPLETED
            if (task.IsCompleted)
            {
                //Handle complete ! :)
                Debug.Log("FireBaseClient->GET_ClanWholeDatas has successfully received the datas.");
                GameManager.singleton.clanManager.clanGateway.OnWholeClanDataSnapshotReceived(task.Result.GetRawJsonValue());
            }
        });
    }
    //GET SPECIFIC DATA FOR A GIVEN CLAN
    public void GET_ClanSpecificData(string p_clanName, string p_dataKey)
    {
        _clanTableReference.Child(p_clanName).Child(p_dataKey).GetValueAsync().ContinueWith(task => {
            //FAULTED
            if (task.IsFaulted)
            {
                //Handle error :(
                Debug.LogError("FireBaseClient->GET_ClanSpecificData has encountered an error.");
            }
            //CANCELED
            if (task.IsCanceled)
            {
                //Handle cancelation :(
                Debug.LogWarning("FireBaseClient->GET_ClanSpecificData has been canceled.");
            }
            //COMPLETED
            if (task.IsCompleted)
            {
                //Handle complete ! :)
                Debug.Log("FireBaseClient->GET_ClanSpecificData has successfully received the datas.");
                GameManager.singleton.clanManager.clanGateway.OnSpecificClanDataReceived(p_dataKey, task.Result.GetRawJsonValue());
            }
        });
    }
    #endregion
    #region SET CLAN DATAS
    //SET WHOLE (PUBLIC/PRIVATE) DATAS FOR A GIVEN CLAN
    public Task WIP_SET_ClanWholeDatas (ClanEntity p_clanToCreate)
    {
        if (p_clanToCreate.clanID.IsNullOrEmpty())
            p_clanToCreate.clanID = _clanTableReference.Push().Key;
        if (p_clanToCreate.isValid())
        {
            Debug.Log("FIREBASE CLIENT -> CreateClan clanJSON:" + p_clanToCreate.GetJSON());
            return _clanTableReference.Child(p_clanToCreate.clanID).SetRawJsonValueAsync(p_clanToCreate.GetJSON());
        }
        Debug.Log("Can't create clan to firebase");
        return null;
    }
    public void SET_ClanWholeDatas(ClanEntity p_clanEntity)
    {
        Debug.LogWarning("FIREBASE SET ClanWholeDatas");
        _clanTableReference.Child(p_clanEntity.clanID).SetRawJsonValueAsync(JsonUtility.ToJson(p_clanEntity)).ContinueWith(task => {
            //FAULTED
            if (task.IsFaulted)
            {
                //Handle error :(
                Debug.LogError("FireBaseClient->SET_ClanWholeDatas has encountered an error.");
            }
            //CANCELED
            if (task.IsCanceled)
            {
                //Handle cancelation :(
                Debug.LogWarning("FireBaseClient->SET_ClanWholeDatas has been canceled.");
            }
            //COMPLETED
            if (task.IsCompleted)
            {
                //Handle complete ! :)
                Debug.Log("FireBaseClient->SET_ClanWholeDatas has successfully set the datas.");
            }
        });
    }
    //SET SPECIFIC (PUBLIC/PRIVATE) DATAS FOR A GIVEN CLAN
    public void SET_ClanSpecificData(string p_clanName, string p_key, string p_value)
    {
        Debug.LogWarning("FIREBASE SET ClanSpecificData");
        _clanTableReference.Child(p_clanName).Child(p_key).SetRawJsonValueAsync(p_value).ContinueWith(task => {
            //FAULTED
            if (task.IsFaulted)
            {
                //Handle error :(
                Debug.LogError("FireBaseClient->SET_ClanSpecificData has encountered an error.");
            }
            //CANCELED
            if (task.IsCanceled)
            {
                //Handle cancelation :(
                Debug.LogWarning("FireBaseClient->SET_ClanSpecificData has been canceled.");
            }
            //COMPLETED
            if (task.IsCompleted)
            {
                //Handle complete ! :)
                Debug.Log("FireBaseClient->SET_ClanSpecificData has successfully set the data.");
            }
        });
    }
    public void SET_AppEndClanSpecificData(string p_clanName, string p_key, string p_value)
    {
        Debug.LogWarning("FIREBASE SET AppEndClanSpecificData");
        string datekey = DateTime.Now.ToString("yyyyMMddHHmmss");
        datekey = string.Join("", datekey.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
        datekey = Regex.Replace(datekey, @"[^A-Za-z0-9]+", "");
        _clanTableReference.Child(p_clanName).Child(p_key).Child(datekey).SetValueAsync(p_value).ContinueWith(task =>
        {
            //FAULTED
            if (task.IsFaulted)
            {
                //Handle error :(
                Debug.LogError("FireBaseClient->SET_AppEndClanSpecificData has encountered an error.");
            }
            //CANCELED
            if (task.IsCanceled)
            {
                //Handle cancelation :(
                Debug.LogWarning("FireBaseClient->SET_AppEndClanSpecificData has been canceled.");
            }
            //COMPLETED
            if (task.IsCompleted)
            {
                //Handle complete ! :)
                Debug.Log("FireBaseClient->SET_AppEndClanSpecificData has successfully set the data.");
            }
        });
    }
    public void WIP_SET_ClanNewMember(string p_clanName, string p_userID)
    {   
        if (!p_clanName.IsNullOrEmpty() && !p_userID.IsNullOrEmpty())
        {
            Debug.LogWarning("FIREBASE SET WIP_SET_ClanNewMember CLAN:"+p_clanName+" USER:"+p_userID);
            _clanTableReference.Child(p_clanName).Child("usersList").Child(p_userID).SetValueAsync("true");
        }
    }
    public void WIP_SET_ClanRemoveMember(string p_clanName, string p_userID)
    {
        if (!p_clanName.IsNullOrEmpty() && !p_userID.IsNullOrEmpty())
        {
            Debug.LogWarning("FIREBASE SET WIP_REMOVE_CLANMEMBER CLAN:"+p_clanName+" USER:"+p_userID);
            _clanTableReference.Child(p_clanName).Child("usersList").Child(p_userID).RemoveValueAsync();
        }   
    }
    #endregion
    #region LISTENERS MANAGEMENT
    //ADD
    //INTERNALLY USED TO MANAGE LISTENERS FOR VALUE CHANGE
    protected FirebaseListenerValueChanged AddListenerForValueChanged(DatabaseReference p_tableToListenOn, string p_elementToListenOn, string p_listenerName, ClanGateway.ClanGatewayEvent p_gatewayDelegate)
    {
        if (_activeListenersValueChanged.ContainsKey(p_listenerName))
        {
            Debug.LogWarning("FIREBASE CLIENT -> VALUE CHANGED LISTENER KEY NAME IS ALREADY EXISTING !");
            return null;
        }
        FirebaseListenerValueChanged l_fbl = new FirebaseListenerValueChanged(p_listenerName, p_tableToListenOn, p_elementToListenOn, (object o, ValueChangedEventArgs e) => {
            if (p_gatewayDelegate == null)
            {
                Debug.LogError(p_listenerName + "delegate not set.");
                return;
            }
            if (e.Snapshot == null)
            {
                Debug.LogError(p_listenerName+" snapshot = null");
                return;
            }
            if (e.Snapshot.Key == null)
            {
                Debug.LogWarning(p_listenerName+" Key:" + e.Snapshot.Key + " Value:" + e.Snapshot.Value);
                return;
            }
            if (e.Snapshot.Value == null)
            {
                Debug.LogWarning(p_listenerName+" Key:"+e.Snapshot.Key+" Value:"+e.Snapshot.Value);
                return;
            }
            p_gatewayDelegate(e.Snapshot.Key.ToString(), (e.Snapshot.Value!=null)?e.Snapshot.Value.ToString():"");
        });
        Debug.Log("ADD LISTENER FOR VALUE CHANGED CHILD:" + p_elementToListenOn + " table:" + p_tableToListenOn.Key.ToString() + " listenerName:" + p_listenerName);
        //MUST CHECK IF ACTIVE LISTENERS IS NOT NULL OR MAJOR BUG APPEAR UNDETECTABLE !!!!!! WATCH OUT
        if(l_fbl!=null && _activeListenersValueChanged!=null)
            _activeListenersValueChanged.Add(p_listenerName, l_fbl);
        return l_fbl;
    }
    //INTERNALLY USED TO MANAGE LISTENERS FOR CHILD CHANGE
    protected FirebaseListenerChildChanged AddListenerForChildChanged(DatabaseReference p_tableToListenOn, string p_elementToListenOn, string p_listenerName, ClanGateway.ClanGatewayEvent p_gatewayDelegate, string p_event)
    {
        if (_activeListenersChildChanged.ContainsKey(p_listenerName))
        {
            Debug.LogWarning("FIREBASE CLIENT -> CHILD CHANGED LISTENER KEY NAME IS ALREADY EXISTING !");
            return null;
        }

        FirebaseListenerChildChanged l_fbl = new FirebaseListenerChildChanged(p_listenerName, p_tableToListenOn, p_elementToListenOn, (object o, ChildChangedEventArgs e) => {
            if (p_gatewayDelegate == null)
            {
                Debug.LogError(p_listenerName + "delegate not set.");
                return;
            }
            if (e.Snapshot == null)
            {
                Debug.LogError(p_listenerName + " snapshot = null");
                return;
            }
            if (e.Snapshot.Key == null)
            {
                Debug.LogWarning(p_listenerName + " Key:" + e.Snapshot.Key + " Value:" + e.Snapshot.Value);
                return;
            }
            if (e.Snapshot.Value == null)
            {
                Debug.LogWarning(p_listenerName + " Key:" + e.Snapshot.Key + " Value:" + e.Snapshot.Value);
                return;
            }
            p_gatewayDelegate(e.Snapshot.Reference.Parent.Parent.Key+"#"+e.Snapshot.Reference.Parent.Key+"#"+e.Snapshot.Key, e.Snapshot.Value.ToString());
        }, p_event);
        if(l_fbl!=null && _activeListenersChildChanged!=null)
            _activeListenersChildChanged.Add(p_listenerName, l_fbl);
        return l_fbl;
    }
    #endregion
    #region SUBSCRIBE / UNSUBSCRIBE
    #region SUBSCRIBE
    //CALLED EXTERNALLY TO KEEP UP TO DATE FOR VALUE CHANGE
    public void ListenForChangesOf(string p_clanName, string p_key, ClanGateway.ClanGatewayEvent p_delegate)
    {
        AddListenerForValueChanged(_clanTableReference.Child(p_clanName), p_key, "L_VALUE_CHANGED_OF_" + p_key.ToUpper() + "_FOR_CLAN_" + p_clanName.ToUpper(), p_delegate);
    }
    //CALLED EXTERNALLY TO KEEP UP TO DATE FOR CHILD CHANGE
    public void ListenChildFor (string p_clanName, string p_key, ClanGateway.ClanGatewayEvent p_delegate, string p_event)
    {
        AddListenerForChildChanged(_clanTableReference.Child(p_clanName),p_key,"L_"+p_event.ToUpper().ToString()+"_OF_"+p_key.ToUpper()+"_FOR_CLAN_"+p_clanName.ToUpper(), p_delegate, p_event);
    }
    #endregion
    #region UNSUBSCRIBE
    //REMOVE
    public void RemoveAllListeners()
    {
        _activeListenersValueChanged.ForEach<KeyValuePair<string, FirebaseListenerValueChanged>>((x) =>
        {
            x.Value.Remove();
        });
        _activeListenersChildChanged.ForEach<KeyValuePair<string, FirebaseListenerChildChanged>>((x) =>
        {
            if (!((x.Value.eventType == "REMOVED" || x.Value.eventType == "ADDED") && x.Value.childToListenOn == "usersList"))
            x.Value.Remove();
        });
        _activeListenersValueChanged = new Dictionary<string, FirebaseListenerValueChanged>();
        _activeListenersValueChanged.Clear();
        _activeListenersChildChanged = new Dictionary<string, FirebaseListenerChildChanged>();
        _activeListenersChildChanged.Clear();
        Debug.LogWarning("ALL FIREBASE LISTENERS REMOVED !");
    }
    public void RemoveChildForListener(string p_clanName, string p_key, string p_event)
    {
        string l_listenerID = "L_" + p_event.ToUpper().ToString() + "_OF_" + p_key.ToUpper() + "_FOR_CLAN_" + p_clanName.ToUpper();
        if (_activeListenersChildChanged.ContainsKey(l_listenerID))
        {
            _activeListenersChildChanged[l_listenerID].Remove();
            _activeListenersChildChanged.Remove(l_listenerID);
            return;
        }
        Debug.LogWarning("FIREBASE -> Can't find and remove the listener Clan:" + p_clanName + " key:" + p_key + " event:" + p_event);
    }
    public void RemoveForChangesOfListener(string p_clanName, string p_key)
    {
        string l_listenerID = "L_VALUE_CHANGED_OF_" + p_key.ToUpper() + "_FOR_CLAN_" + p_clanName.ToUpper();
        if (_activeListenersValueChanged.ContainsKey(l_listenerID))
        {
            _activeListenersValueChanged[l_listenerID].Remove();
            _activeListenersValueChanged.Remove(l_listenerID);
        }
        Debug.LogWarning("FIREBASE -> Can't find and remove the listener Clan:" + p_clanName + " key:" + p_key + " event: VALUE CHANGES");
    }
    #endregion
    #endregion
    #endregion
}

//USED IN CLAN DATA MANAGEMENT
public class FirebaseListenerValueChanged {
    //TO RETRIEVE THE LISTENER
    public string listenerName;
    //THE POSITION IN THE DATABASE
    public DatabaseReference dbRef;
    //THE CHILD TO LISTEN ON 
    public string childToListenOn;
    //THE CLIENT DELEGATE TO FIRE
    public EventHandler<ValueChangedEventArgs> clientDelegate;
    public void Remove()
    {
        dbRef.Child(childToListenOn).ValueChanged -= clientDelegate;
    }
    public void Add()
    {
        dbRef.Child(childToListenOn).ValueChanged += clientDelegate;
    }
    public FirebaseListenerValueChanged(string p_listenerName, DatabaseReference p_dbRef, string p_childToListenOn, EventHandler<ValueChangedEventArgs> p_clientDelegate)
    {
        listenerName = p_listenerName;
        dbRef = p_dbRef;
        clientDelegate = p_clientDelegate;
        dbRef.Child(p_childToListenOn).ValueChanged += clientDelegate;
        
        childToListenOn = p_childToListenOn;
    }
    public FirebaseListenerValueChanged(string p_listenerName)
    {

    }
}
public class FirebaseListenerChildChanged
{
    //TO RETRIEVE THE LISTENER
    public string listenerName;
    //THE POSITION IN THE DATABASE
    public DatabaseReference dbRef;
    //THE CHILD TO LISTEN ON 
    public string childToListenOn;
    //THE CLIENT DELEGATE TO FIRE
    public System.EventHandler<ChildChangedEventArgs> clientDelegate;
    //THE GATEWAY DELEGATE TO FIRE
    public ClanGateway.ClanGatewayEvent gatewayDelegate;
    public string eventType;
    public void Remove()
    {
        switch (eventType)
        {
            case "ADDED":
                dbRef.Child(childToListenOn).ChildAdded -= clientDelegate;
                break;
            case "REMOVED":
                dbRef.Child(childToListenOn).ChildRemoved += clientDelegate;
                break;
            case "CHANGED":
                dbRef.Child(childToListenOn).ChildChanged += clientDelegate;
                break;
            case "MOVED":
                dbRef.Child(childToListenOn).ChildMoved += clientDelegate;
                break;
        }
    }
    public void Add()
    {
        switch (eventType)
        {
            case "ADDED":
                dbRef.Child(childToListenOn).ChildAdded += clientDelegate;
                break;
            case "REMOVED":
                dbRef.Child(childToListenOn).ChildRemoved += clientDelegate;
                break;
            case "CHANGED":
                dbRef.Child(childToListenOn).ChildChanged += clientDelegate;
                break;
            case "MOVED":
                dbRef.Child(childToListenOn).ChildMoved += clientDelegate;
                break;
        }
    }
    public FirebaseListenerChildChanged (string p_listenerName, DatabaseReference p_dbRef, string p_childToListenOn, System.EventHandler<ChildChangedEventArgs> p_clientDelegate, string p_event)
    {
        listenerName = p_listenerName;
        dbRef = p_dbRef;
        clientDelegate = p_clientDelegate;
        childToListenOn = p_childToListenOn;
        eventType = p_event;
        switch (p_event)
        {
            case "ADDED":
                dbRef.Child(p_childToListenOn).ChildAdded += clientDelegate;
                break;
            case "REMOVED":
                dbRef.Child(p_childToListenOn).ChildRemoved += clientDelegate;
                break;
            case "CHANGED":
                dbRef.Child(p_childToListenOn).ChildChanged += clientDelegate;
                break;
            case "MOVED":
                dbRef.Child(p_childToListenOn).ChildMoved += clientDelegate;
                break;
        }
    }

}