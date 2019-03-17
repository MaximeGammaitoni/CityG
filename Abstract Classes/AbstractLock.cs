using System.Collections.Generic;
using System.Collections;
using System;

public abstract class AbstractLock : ILockable
{
    #region EVENTS
    public delegate void AbstractLockEvent(string[] p_args);
    public event AbstractLockEvent OnUnlock;
    public event AbstractLockEvent OnLock;
    #endregion
    #region PROTECTED PROPERTIES
    protected bool _locked;
    protected DateTime _startedUnlockDateTime;
    protected Dictionary<string, IEnumerator> _coroutines;
    #endregion
    #region PUBLIC PROPERTIES
    #endregion
    #region PROTECTED METHODS
    protected void E_OnLock(string[] p_args = null)
    {
        if (OnLock != null)
        {
            OnLock(p_args);
        }
    }
    protected void E_OnUnlock(string[] p_args = null)
    {
        if (OnUnlock != null)
        {
            OnUnlock(p_args);
        }
    }
    #endregion
    #region PUBLIC METHODS
    #region GET
    public virtual bool GET_Locked()
    {
        return _locked;
    }
    public DateTime GET_StartedUnlockDateTime()
    {
        return _startedUnlockDateTime;
    }
    #endregion
    #region SET
    public virtual void SET_Locked(bool p_value)
    {
        if (p_value != _locked)
            _locked = p_value;
        return;
    }
    public void SET_StartedUnlockDateTime(DateTime p_value)
    {
        _startedUnlockDateTime = p_value;
    }
    #endregion
    #region ACTIONS
    public virtual void Lock()
    {
        SET_Locked(true);
        E_OnLock();
    }
    public virtual void Unlock()
    {
        SET_Locked(false);
        E_OnUnlock();
    }
    #endregion
    public virtual void ClearCoroutines()
    {
        /*foreach(KeyValuePair<string,IEnumerator> current in _coroutines)
        {
            current.Value.Reset();
        }*/
        _coroutines = new Dictionary<string, IEnumerator>();
    }
    public virtual void ClearDelegates()
    {
        OnUnlock = null;
        OnLock = null;
    }
    #endregion
    #region CONSTRUCTORS
    public AbstractLock(bool p_locked = true)
    {
        SET_Locked(p_locked);
        SET_StartedUnlockDateTime(DateTime.MinValue);
    }
    #endregion
}