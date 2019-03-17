using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractRequest {
    #region EVENTS
    public delegate void AbstractRequestDelegate(object p_object,EventArgs p_eventArgs);
    public event AbstractRequestDelegate OnNotify;
    public event AbstractRequestDelegate OnConvert;
    #endregion
    #region PROTECTED PROPERTIES
    protected string _path;
    protected string _key;
    protected string _value;
    protected object _valueConverted;
    protected REQUEST_TYPE _requestType;
    protected REQUEST_STATUS _requestStatus;
    protected RESULT_STATUS _resultStatus;
    protected REQUEST_PERMISSION _requestPermission;
    #endregion
    #region PUBLIC PROPERTIES
    public enum REQUEST_TYPE { NULL, READ, WRITE, EXECUTE};
    public enum REQUEST_STATUS { NULL, WAITING, STARTED, PROCESSING, FINISHED};
    public enum RESULT_STATUS { NULL, ABORTED, ERROR, COMPLETED};
    public enum REQUEST_PERMISSION { NULL, SELF_ONLY, PUBLIC, CLAN, FRIEND };
    #endregion
    #region PROTECTED METHODS
    #endregion
    #region PUBLIC METHODS
    #region GET
    public virtual string GET_Path()
    {
        return _path;
    }
    public virtual string GET_Key()
    {
        return _key;
    }
    public virtual string GET_Value()
    {
        return _value;
    }
    public virtual REQUEST_TYPE GET_Type()
    {
        return _requestType;
    }
    public virtual REQUEST_STATUS GET_Status()
    {
        return _requestStatus;
    }
    public virtual RESULT_STATUS GET_Result()
    {
        return _resultStatus;
    }
    public virtual REQUEST_PERMISSION GET_Permission()
    {
        return _requestPermission;
    }
    #endregion
    #region SET
    public virtual string SET_Path(string p_path)
    {
        _path = p_path;
        return _path;
    }
    public virtual string SET_Key(string p_key)
    {
        _key = p_key;
        return _key;
    }
    public virtual string SET_Value(string p_value)
    {
        _value = p_value;
        return _value;
    }
    public virtual string SET_Type(REQUEST_TYPE p_type)
    {
        _requestType = p_type;
        return _requestType.ToString();
    }
    public virtual string SET_Status(REQUEST_STATUS p_status)
    {
        _requestStatus = p_status;
        return _requestStatus.ToString();
    }
    public virtual string SET_Result(RESULT_STATUS p_result)
    {
        _resultStatus = p_result;
        return _resultStatus.ToString();
    }
    public virtual string SET_Permission(REQUEST_PERMISSION p_permission)
    {
        _requestPermission = p_permission;
        return _requestPermission.ToString();
    }
    #endregion
    #region ACTIONS
    public virtual void Notify()
    {
        if (OnNotify != null)
        {
            OnNotify(_valueConverted,null);
            OnNotify = null;
        }
    }
    public void ClearListeners()
    {
        OnNotify = null;
    }
    public virtual void Convert()
    {
        if (OnConvert != null)
        {
            OnConvert(_valueConverted, null);
            OnConvert = null;
        }
    }
    #endregion
    #region STATICS
    public static REQUEST_PERMISSION StringToPermission(string p_string)
    {
        REQUEST_PERMISSION l_output = REQUEST_PERMISSION.NULL;
        switch (p_string)
        {
            case "PUBLIC":
                l_output = REQUEST_PERMISSION.PUBLIC;
                break;
            case "SELF_ONLY":
                l_output = REQUEST_PERMISSION.SELF_ONLY;
                break;
            case "CLAN":
                l_output = REQUEST_PERMISSION.CLAN;
                break;
            case "FRIEND":
                l_output = REQUEST_PERMISSION.FRIEND;
                break;
        }
        return l_output;
    }
    public static string PermissionToString(REQUEST_PERMISSION p_permission)
    {
        string l_output = "";
        switch (p_permission)
        {
            case REQUEST_PERMISSION.PUBLIC:
                l_output = "PUBLIC";
                break;
            case REQUEST_PERMISSION.SELF_ONLY:
                l_output = "SELF_ONLY";
                break;
            case REQUEST_PERMISSION.CLAN:
                l_output = "CLAN";
                break;
            case REQUEST_PERMISSION.FRIEND:
                l_output = "FRIEND";
                break;
        }
        return l_output;
    }
    #endregion
    #endregion
    #region CONSTRUCTORS
    #endregion
}

public class FirebaseRequest : AbstractRequest
{
    #region PROTECTED PROPERTIES
    #endregion
    #region PUBLIC PROPERTIES
    #endregion
    #region PROTECTED METHODS
    #endregion
    #region PUBLIC METHODS
    #endregion
    #region CONSTRUCTORS
    #endregion
}

public class FirebaseRequestBuilder
{
    #region SET
    public static string SET_Path(FirebaseRequest p_request, string p_path)
    {
        p_request.SET_Path(p_path);
        return p_request.GET_Path();
    }
    public static string SET_Key(FirebaseRequest p_request, string p_key)
    {
        p_request.SET_Key(p_key);
        return p_request.GET_Key();
    }
    public static string SET_Value(FirebaseRequest p_request, string p_value)
    {
        p_request.SET_Value(p_value);
        return p_request.GET_Value();
    }
    public static string SET_Type(FirebaseRequest p_request, FirebaseRequest.REQUEST_TYPE p_type)
    {
        string l_output = "REQUEST TYPE NOT SET";
        switch (p_type)
        {
            case AbstractRequest.REQUEST_TYPE.NULL:
                l_output = "REQUEST TYPE IS NOW NULL";
                break;
            case AbstractRequest.REQUEST_TYPE.READ:
                l_output = "REQUEST TYPE IS NOW READ";
                break;
            case AbstractRequest.REQUEST_TYPE.WRITE:
                l_output = "REQUEST TYPE IS NOW WRITE";
                break;
            case AbstractRequest.REQUEST_TYPE.EXECUTE:
                l_output = "REQUEST TYPE IS NOW EXECUTE";
                break;
        }
        return l_output;
    }
    public static string SET_Status(FirebaseRequest p_request, FirebaseRequest.REQUEST_STATUS p_status)
    {
        string l_output = "STATUS NOT SET";
        switch (p_status)
        {
            case AbstractRequest.REQUEST_STATUS.NULL:
                l_output = "STATUS IS NOW NULL";
                break;
            case AbstractRequest.REQUEST_STATUS.WAITING:
                l_output = "STATUS IS NOW WAITING";
                break;
            case AbstractRequest.REQUEST_STATUS.PROCESSING:
                l_output = "STATUS IS NOW PROCESSING";
                break;
            case AbstractRequest.REQUEST_STATUS.STARTED:
                l_output = "STATUS IS NOW STARTED";
                break;
            case AbstractRequest.REQUEST_STATUS.FINISHED:
                l_output = "STATUS IS NOW FINISHED";
                break;
        }
        p_request.SET_Status(p_status);
        return l_output;
    }
    public static string SET_Result(FirebaseRequest p_request, FirebaseRequest.RESULT_STATUS p_result)
    {
        string l_output = "RESULT TYPE NOT SET";

        switch (p_result)
        {
            case AbstractRequest.RESULT_STATUS.NULL:
                l_output = "RESULT IS NOW NULL";
                break;
            case AbstractRequest.RESULT_STATUS.ABORTED:
                l_output = "RESULT IS NOW ABORTED";
                break;
            case AbstractRequest.RESULT_STATUS.COMPLETED:
                l_output = "RESULT IS NOW COMPLETED";
                break;
            case AbstractRequest.RESULT_STATUS.ERROR:
                l_output = "RESULT IS NOW ERROR";
                break;
        }
        p_request.SET_Result(p_result);
        return l_output;
    }
    public static string SET_Permission(FirebaseRequest p_request, FirebaseRequest.REQUEST_PERMISSION p_permissionLevel)
    {
        p_request.SET_Permission(p_permissionLevel);
        return AbstractRequest.PermissionToString(p_request.GET_Permission());
    }
    public static void SET_Listener(FirebaseRequest p_request, AbstractRequest.AbstractRequestDelegate p_delegate)
    {
        p_request.OnNotify += p_delegate;
        return;
    }
    #endregion
}