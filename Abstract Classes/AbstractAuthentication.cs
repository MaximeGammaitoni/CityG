using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractAuthentication {
    #region EVENTS
    public delegate void AuthenticationDelegate (object p_object, EventArgs p_args);
    public event AuthenticationDelegate OnIDSet;
    public event AuthenticationDelegate OnPasswordSet;
    #endregion
    #region PROTECTED PROPERTIES
    protected string _ID;
    protected string _password;
    #endregion
    #region PUBLIC PROPERTIES
    #endregion
    #region PROTECTED METHODS
    protected void E_OnIDSet(object p_object = null, EventArgs p_args = null)
    {
        if (OnIDSet != null)
        {
            OnIDSet(p_object, p_args);
        }
    }
    protected void E_OnPasswordSet(object p_object = null, EventArgs p_args = null)
    {
        if(OnPasswordSet!= null)
        {
            OnPasswordSet(p_object, p_args);
        }
    }
    #endregion
    #region PUBLIC METHODS
    #region SET
    public abstract string SET_ID(string p_id);
    public abstract string SET_Password(string p_password);
    #endregion
    #region GET
    public abstract string GET_ID();
    public abstract string GET_Password();
    #endregion
    #region ACTIONS
    #endregion
    #endregion
    #region CONSTRUCTORS
    #endregion
}
