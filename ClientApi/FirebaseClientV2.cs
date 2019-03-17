using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

public class FirebaseClientV2 : AbstractClient {
    #region EVENTS
    public delegate void FirebaseClientV2Delegate(object p_object, EventArgs p_eventArgs);
    #endregion
    #region PROTECTED PROPERTIES
    protected FirebaseApp _app;
    protected DatabaseReference _rootReference;
    protected Dictionary<string,DatabaseReference> _tablesReferences = new Dictionary<string, DatabaseReference>();
    protected bool _dependenciesCheckedAndFixed;
    #endregion
    #region PUBLIC PROPERTIES
    #endregion
    #region PROTECTED METHODS
    protected void SET_FirebaseApp()
    {
        _app = FirebaseApp.DefaultInstance;
        Debug.Log("FIREBASECLIENTV2->AppInstance SET.");
    }
    protected void SET_FirebaseTablesReferences()
    {
        //ACCESS LEVEL = PUBLIC - CLAN - FRIEND - SELF
        _tablesReferences = new Dictionary<string, DatabaseReference>();
        _rootReference = FirebaseDatabase.DefaultInstance.RootReference;
        //PUBLIC
        _tablesReferences["PUBLIC/USERS/USERNAMES"] = _rootReference.Child("PUBLIC/USERS/USERNAMES");
        _tablesReferences["PUBLIC/CLANS/MEMBERS/USERNAMES"] = _rootReference.Child("PUBLIC/CLANS/MEMBERS/USERNAMES");
        _tablesReferences["PUBLIC/CLANS/PROFILES"] = _rootReference.Child("PUBLIC/CLANS/PROFILES");
        //SELF_ONLY
        _tablesReferences["SELF_ONLY/PLAYER_PROFILE"] = _rootReference.Child("SELF_ONLY/PLAYER_PROFILE");
        //CLAN
        _tablesReferences["CLAN/CLANS/MESSAGES/CONTENTS"] = _rootReference.Child("CLAN/CLANS/MESSAGES/CONTENTS");
        //FRIEND
    }
    #region LIFE ASSISTANT
    //Set Values from configuration file
    protected IEnumerator _Initialize()
    {
        //SET DEPENDANCIES
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            DependencyStatus ds = task.Result;
            if (ds == DependencyStatus.Available)
            {
                _dependenciesCheckedAndFixed = true;
                Debug.Log("FIREBASECLIENTV2 -> Dependencies are checked and fixed");
            }
            else
            {
                _dependenciesCheckedAndFixed = false;
                GET_LifeCycleAssistant().SET_Initialized(false);
                Debug.LogError("FIREBASECLIENTV2-> _Initialize, DependenciesStatus != Available. Return.");
                return;
            }
        });
        yield return new WaitUntil(() => {
            Debug.Log("FIREBASECLIENTV2-> Is Waiting for Dependencies Checked and Fixed.");
            return _dependenciesCheckedAndFixed;
        });
        Debug.Log("FIREBASECLIENTV2-> Dependencies Checked and Fixed, is OK.");
        //SET APP
        SET_FirebaseApp();
        SET_FirebaseTablesReferences();
        isConnected = true;
        
        yield return new WaitUntil(() => {
            Debug.Log("FIREBASECLIENTV2-> Is Waiting for server to be connected");
            return isConnected;
        });
        Debug.Log("FIREBASECLIENTV2-> Server is connected, is OK.");
        GET_LifeCycleAssistant().SET_Initialized(true);
    }
    //Set Values from external components
    protected IEnumerator _Load()
    {
        //LOAD EXISTING DATA FROM PLAYER PROFILE
        //Building Request
        FirebaseRequest l_fbReq = new FirebaseRequest();
        FirebaseRequestBuilder.SET_Path(l_fbReq, "SELF_ONLY/PLAYER_PROFILE");
        FirebaseRequestBuilder.SET_Key(l_fbReq, "TIM");
        FirebaseRequestBuilder.SET_Type(l_fbReq, AbstractRequest.REQUEST_TYPE.READ);
        FirebaseRequestBuilder.SET_Result(l_fbReq, AbstractRequest.RESULT_STATUS.NULL);
        FirebaseRequestBuilder.SET_Permission(l_fbReq, AbstractRequest.REQUEST_PERMISSION.SELF_ONLY);
        FirebaseRequestBuilder.SET_Status(l_fbReq, AbstractRequest.REQUEST_STATUS.WAITING);
        FirebaseRequestBuilder.SET_Value(l_fbReq, "");
        //Execution
        _HandleReadRequest(l_fbReq);
        //Waiting Result
        yield return new WaitUntil(() => { return l_fbReq.GET_Status() == AbstractRequest.REQUEST_STATUS.FINISHED; });
        //Clearing Request
        l_fbReq = null;
        GET_LifeCycleAssistant().SET_Loaded(true);
    }
    //Enable use of the object
    protected IEnumerator _Start()
    {
        yield return 0;
    }
    //Disable use of the object
    protected IEnumerator _Pause()
    {
        yield return 0;
    }
    //Re-Enable use of the object
    protected IEnumerator _Resume()
    {
        yield return 0;
    }
    //Disable use of the object
    protected IEnumerator _Stop()
    {
        yield return 0;
    }
    //Unset values related to external components
    protected IEnumerator _Unload()
    {
        yield return 0;
    }
    //Unset values related to internal configuration of the component
    protected IEnumerator _Quit()
    {
        yield return 0;
    }
    //Destroy component
    protected IEnumerator _Kill()
    {
        yield return 0;
        Debug.Log("");
    }
    #endregion
    #region REQUEST MANAGEMENT
    protected bool _HandleReadRequest(FirebaseRequest p_request)
    {
        //IS SERVER ACCESSIBLE ?
        if (!isConnected)
        {
            Debug.Log("FIREBASECLIENTV2->Server is not connected request is aborted");
            p_request.SET_Result(AbstractRequest.RESULT_STATUS.ABORTED);
            return false;
        }
        //IS REQUEST PERMITTED BY CREDENTIALS ?
        if (!IsAccessPermitted(userCredentials, p_request))
        {
            Debug.Log("FIREBASECLIENTV2->Access denied for this request.");
            p_request.SET_Result(AbstractRequest.RESULT_STATUS.ABORTED);
            return false;
        }
        //SET REQUEST
        DatabaseReference l_requestReference = GET_FirebaseDatabaseReference(GET_TableReference(p_request.GET_Permission(), p_request.GET_Path()),p_request.GET_Key());
        if(l_requestReference == null)
        {
            Debug.Log("FIREBASECLIENTV2->Request reference is not valid, request error.");
            p_request.SET_Result(AbstractRequest.RESULT_STATUS.ERROR);
            return false;
        }
        FirebaseRequestBuilder.SET_Status(p_request, AbstractRequest.REQUEST_STATUS.STARTED);
        //EXECUTE
        Task<DataSnapshot> l_task = l_requestReference.GetValueAsync();
        FirebaseRequestBuilder.SET_Status(p_request, AbstractRequest.REQUEST_STATUS.PROCESSING);
        l_task.ContinueWith(task =>
        {
            FirebaseRequestBuilder.SET_Status(p_request, AbstractRequest.REQUEST_STATUS.FINISHED);
            if (task.IsCanceled)
            {
                FirebaseRequestBuilder.SET_Result(p_request, AbstractRequest.RESULT_STATUS.ABORTED);
            }
            if (task.IsFaulted)
            {
                FirebaseRequestBuilder.SET_Result(p_request, AbstractRequest.RESULT_STATUS.ERROR);
            }
            if (task.IsCompleted)
            {
                FirebaseRequestBuilder.SET_Result(p_request, AbstractRequest.RESULT_STATUS.COMPLETED);
                FirebaseRequestBuilder.SET_Value(p_request, task.Result.GetRawJsonValue());
            }
            //CONVERT RESULTS
            p_request.Convert();
            //SEND RESULT TO LISTENERS
            p_request.Notify();
        });
        return true;
    }
    protected bool _HandleWriteRequest(FirebaseRequest p_request)
    {
        //IS SERVER ACCESSIBLE ?
        if (!isConnected)
        {
            Debug.Log("FIREBASECLIENTV2->Server is not connected request is aborted");
            p_request.SET_Result(AbstractRequest.RESULT_STATUS.ABORTED);
            return false;
        }
        //IS REQUEST PERMITTED BY CREDENTIALS ?
        if (!IsAccessPermitted(userCredentials, p_request))
        {
            Debug.Log("FIREBASECLIENTV2->Access denied for this request.");
            p_request.SET_Result(AbstractRequest.RESULT_STATUS.ABORTED);
            return false;
        }
        //SET REQUEST
        DatabaseReference l_requestReference = GET_FirebaseDatabaseReference(GET_TableReference(p_request.GET_Permission(), p_request.GET_Path()), p_request.GET_Key());
        if (l_requestReference == null)
        {
            Debug.Log("FIREBASECLIENTV2->Request reference is not valid, request error.");
            p_request.SET_Result(AbstractRequest.RESULT_STATUS.ERROR);
            return false;
        }
        //ON RESULT
        FirebaseRequestBuilder.SET_Status(p_request, AbstractRequest.REQUEST_STATUS.STARTED);
        //EXECUTE
        Task l_task = l_requestReference.SetRawJsonValueAsync(p_request.GET_Value());
        FirebaseRequestBuilder.SET_Status(p_request, AbstractRequest.REQUEST_STATUS.PROCESSING);
        l_task.ContinueWith(task =>
        {
            FirebaseRequestBuilder.SET_Status(p_request, AbstractRequest.REQUEST_STATUS.FINISHED);
            if (task.IsCanceled)
            {
                FirebaseRequestBuilder.SET_Result(p_request, AbstractRequest.RESULT_STATUS.ABORTED);
            }
            if (task.IsFaulted)
            {
                FirebaseRequestBuilder.SET_Result(p_request, AbstractRequest.RESULT_STATUS.ERROR);
            }
            if (task.IsCompleted)
            {
                FirebaseRequestBuilder.SET_Result(p_request, AbstractRequest.RESULT_STATUS.COMPLETED);
            }
            //SEND RESULT TO LISTENERS
            p_request.Notify();
        });
        return true;
    }
    #endregion
    #endregion
    #region PUBLIC METHODS
    #region GET
    public override AbstractAuthentication GET_UserCredentials()
    {
        return userCredentials;
    }
    public override void GET_Data(Dictionary<string,string> p_args, AbstractRequest.AbstractRequestDelegate p_delegate)
    {
        //BUILD REQUEST
        FirebaseRequest l_tmp = new FirebaseRequest();
        FirebaseRequestBuilder.SET_Key(l_tmp, p_args["Key"]);
        FirebaseRequestBuilder.SET_Value(l_tmp, "");
        FirebaseRequestBuilder.SET_Path(l_tmp, p_args["Path"]);
        FirebaseRequestBuilder.SET_Result(l_tmp, AbstractRequest.RESULT_STATUS.NULL);
        FirebaseRequestBuilder.SET_Status(l_tmp, AbstractRequest.REQUEST_STATUS.WAITING);
        FirebaseRequestBuilder.SET_Type(l_tmp, AbstractRequest.REQUEST_TYPE.READ);
        FirebaseRequestBuilder.SET_Permission(l_tmp, AbstractRequest.StringToPermission(p_args["Permission"]));
        
        //CHECK IF REQUEST IS ALLOWED FOR CREDENTIALS
        if (!IsAccessPermitted(userCredentials, l_tmp))
        {
            Debug.LogError("FIREBASECLIENTV2 -> GET_Data, User Credentials for Request is not permitted.");
            return;
        }
        //EXECUTE REQUEST
        _HandleReadRequest(l_tmp);
    }
    public override void GET_Data(AbstractRequest p_request)
    {
        FirebaseRequest l_request = p_request as FirebaseRequest;
        if (!IsAccessPermitted(GET_UserCredentials(), l_request))
        {
            Debug.LogError("FIREBASECLIENTV2 -> GET_Data, User Credentials for Request is not permitted.");
            return;
        }
        _HandleReadRequest(l_request);
    }
    public FirebaseApp GET_FirebaseApp()
    {
        return _app;
    }
    public DatabaseReference GET_TableReference(AbstractRequest.REQUEST_PERMISSION p_permission, string p_path)
    {
        string l_permission = AbstractRequest.PermissionToString(p_permission);
        string l_path = l_permission + "/" + p_path;
        DatabaseReference l_output=null;

        if (!_tablesReferences.ContainsKey(l_path))
        {
            Debug.Log("FIREBASECLIENTV2-> GET_TableReference, Table seek doesn't exist, return null. PATH:"+l_path);
        }
        else
        {
            l_output = _tablesReferences[l_path];
        }

        return l_output;
    }
    public DatabaseReference GET_FirebaseDatabaseReference(DatabaseReference p_reference, string p_key)
    {
        if(p_reference == null)
        {
            return null;
        }
        return p_reference.Child(p_key);
    }
    #endregion
    #region SET
    public override AbstractAuthentication SET_UserCredentials(AbstractAuthentication p_abstractAuthentication)
    {
        //VALIDATE NEW CREDENTIALS
        //SET NEW CREDENTIALS
        userCredentials = p_abstractAuthentication;
        return userCredentials;
    }
    public override void SET_Data(Dictionary<string, string> p_args, AbstractRequest.AbstractRequestDelegate p_delegate)
    {
        //BUILD REQUEST
        FirebaseRequest l_tmp = new FirebaseRequest();
        FirebaseRequestBuilder.SET_Key(l_tmp, p_args["Key"]);
        FirebaseRequestBuilder.SET_Value(l_tmp, p_args["Value"]);
        FirebaseRequestBuilder.SET_Path(l_tmp, p_args["Path"]);
        FirebaseRequestBuilder.SET_Result(l_tmp, AbstractRequest.RESULT_STATUS.NULL);
        FirebaseRequestBuilder.SET_Status(l_tmp, AbstractRequest.REQUEST_STATUS.WAITING);
        FirebaseRequestBuilder.SET_Type(l_tmp, AbstractRequest.REQUEST_TYPE.WRITE);
        FirebaseRequestBuilder.SET_Permission(l_tmp, AbstractRequest.StringToPermission(p_args["Permission"]));
        //CHECK IF REQUEST IS ALLOWED FOR CREDENTIALS
        if (!IsAccessPermitted(userCredentials, l_tmp))
        {
            Debug.LogError("FIREBASECLIENTV2 -> Acces DENIED for request :"+l_tmp.ToString());
            return;
        }
        //EXECUTE REQUEST
        _HandleWriteRequest(l_tmp);
    }
    public override void SET_Data(AbstractRequest p_request)
    {
        FirebaseRequest l_request = p_request as FirebaseRequest;
        if (!IsAccessPermitted(GET_UserCredentials(), l_request))
        {
            Debug.LogError("FIREBASECLIENTV2 -> SET_Data, User Credentials for Request is not permitted.");
            return;
        }
        _HandleWriteRequest(l_request);
    }
    public override string SET_URLToDatabase(string p_url)
    {
        string l_output = base.SET_URLToDatabase(p_url);
        Debug.Log("FIREBASECLIENTV2-> URL To Database has been set to :" + l_output);
        return l_output;
    }
    #endregion
    #region ACTIONS
    public override void ConnectToDatabase()
    {
        //TRY TO ENABLE CONNECTION
        throw new System.NotImplementedException();
    }
    public override void DisconnectFromDatabase()
    {
        //TRY TO DISCONNECT FROM DATABASE PROPERLY
        throw new System.NotImplementedException();
    }
    public override bool IsAccessPermitted(AbstractAuthentication p_credentials, AbstractRequest p_request)
    {
        if(p_credentials.GET_ID() == "TIM" && p_credentials.GET_Password() == "ROOT")
        {
            return true;
        }
        return false;
    }
    #endregion
    #region LIFE ASSISTANT
    //Set Values from configuration file
    public override void Initialize()
    {
        GameObject.Find("FirebaseClientV2").GetComponent<ClientBehaviour>().StartCoroutine(_Initialize());
    }
    //Set Values from external components
    public override void Load()
    {
        GameManager.singleton.StartCouroutineInGameManager(_Load());
    }
    //Enable use of the object
    public override void Start()
    {
        GameManager.singleton.StartCouroutineInGameManager(_Start());
    }
    //Disable use of the object
    public override void Pause()
    {
        GameManager.singleton.StartCouroutineInGameManager(_Pause());
    }
    //Re-Enable use of the object
    public override void Resume()
    {
        GameManager.singleton.StartCouroutineInGameManager(_Resume());
    }
    //Disable use of the object
    public override void Stop()
    {
        GameManager.singleton.StartCouroutineInGameManager(_Stop());
    }
    //Unset values related to external components
    public override void Unload()
    {
        GameManager.singleton.StartCouroutineInGameManager(_Unload());
    }
    //Unset values related to internal configuration of the component
    public override void Quit()
    {
        GameManager.singleton.StartCouroutineInGameManager(_Quit());
    }
    //Destroy component
    public override void Kill()
    {
        GameManager.singleton.StartCouroutineInGameManager(_Kill());
    }
    #endregion
    #endregion
    #region CONSTRUCTORS
    public FirebaseClientV2(string p_urlToDatabase) : base(p_urlToDatabase)
    {
    }
    #endregion
}