using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseAuthenticationCredentials : AbstractAuthentication {
    #region EVENTS
    #endregion
    #region PROTECTED PROPERTIES
    #endregion
    #region PUBLIC PROPERTIES
    #endregion
    #region PROTECTED METHODS
    #endregion
    #region PUBLIC METHODS
    #region SET
    public override string SET_ID(string p_id)
    {
        _ID = p_id;
        E_OnIDSet(_ID, null);
        return _ID;
    }
    public override string SET_Password(string p_password)
    {
        _password = p_password;
        E_OnPasswordSet(_password, null);
        return _password;
    }
    #endregion
    #region GET
    public override string GET_ID()
    {
        return _ID;
    }
    public override string GET_Password()
    {
        return _password;
    }
    #endregion
    #region ACTIONS
    #endregion
    #endregion
    #region CONSTRUCTORS
    public FirebaseAuthenticationCredentials(string p_id, string p_password)
    {
        SET_ID(p_id);
        SET_Password(p_password);
    }
    #endregion
}
