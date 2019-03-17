using System;
public interface ILockable
{
    #region SET
    void SET_Locked(bool p_value);
    void SET_StartedUnlockDateTime(DateTime p_value);
    #endregion
    #region GET
    bool GET_Locked();
    DateTime GET_StartedUnlockDateTime();
    #endregion
    #region ACTIONS
    void Lock();
    void Unlock();
    #endregion
}