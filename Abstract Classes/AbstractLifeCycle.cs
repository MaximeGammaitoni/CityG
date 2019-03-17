using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractLifeCycle {
    public enum LIFECYCLESTATE { NULL, INITIALIZED, LOADED, RUNNING, PAUSED, STOPED, UNLOADED, KILLED};
    public LIFECYCLESTATE oldLifeCycleState = LIFECYCLESTATE.NULL;
    public LIFECYCLESTATE currentLifeCycleState = LIFECYCLESTATE.NULL;
    public delegate void LifeCycleEvent(string p_args);
    public event LifeCycleEvent OnStartInitializing;
    public event LifeCycleEvent OnEndInitializing;
    public event LifeCycleEvent OnStartLoading;
    public event LifeCycleEvent OnEndLoading;
    public event LifeCycleEvent OnStartBeginning;
    public event LifeCycleEvent OnEndBeginning;
    public event LifeCycleEvent OnStartPausing;
    public event LifeCycleEvent OnEndPausing;
    public event LifeCycleEvent OnStartUpdating;
    public event LifeCycleEvent OnEndUpdating;
    public event LifeCycleEvent OnStartResumeing;
    public event LifeCycleEvent OnEndResumeing;
    public event LifeCycleEvent OnStartStoping;
    public event LifeCycleEvent OnEndStoping;
    public event LifeCycleEvent OnStartUnloading;
    public event LifeCycleEvent OnEndUnloading;
    public event LifeCycleEvent OnStartDestroying;
    public event LifeCycleEvent OnEndDestroying;

    #region LIFECYCLE FIRE EVENTS
    //INITIALIZING
    protected void E_OnStartInitializing(string p_args)
    {
        if (OnStartInitializing != null)
            OnStartInitializing(p_args);
    }
    protected void E_OnEndInitializing(string p_args)
    {
        if (OnEndInitializing != null)
            OnEndInitializing(p_args);
    }
    //LOADING
    protected void E_OnStartLoading(string p_args)
    {
        if (OnStartLoading != null)
            OnStartLoading(p_args);
    }
    protected void E_OnEndLoading(string p_args)
    {
        if (OnEndLoading != null)
            OnEndLoading(p_args);
    }
    //BEGINNING
    protected void E_OnStartBeginning(string p_args)
    {
        if (OnStartBeginning != null)
            OnStartBeginning(p_args);
    }
    protected void E_OnEndBeginning(string p_args)
    {
        if (OnEndBeginning != null)
            OnEndBeginning(p_args);
    }
    //PAUSING
    protected void E_OnStartPausing(string p_args)
    {
        if (OnStartPausing != null)
            OnStartPausing(p_args);
    }
    protected void E_OnEndPausing(string p_args)
    {
        if (OnEndPausing != null)
            OnEndPausing(p_args);
    }
    //UPDATING
    protected void E_OnStartUpdating(string p_args)
    {
        if (OnStartUpdating != null)
            OnStartUpdating(p_args);
    }
    protected void E_OnEndUpdating(string p_args)
    {
        if (OnEndUpdating != null)
            OnEndUpdating(p_args);
    }
    //RESUMEING
    protected void E_OnStartResumeing(string p_args)
    {
        if (OnStartResumeing != null)
            OnStartResumeing(p_args);
    }
    protected void E_OnEndResumeing(string p_args)
    {
        if (OnEndResumeing != null)
            OnEndResumeing(p_args);
    }
    //STOPING
    protected void E_OnStartStoping(string p_args)
    {
        if (OnStartStoping != null)
            OnStartStoping(p_args);
    }
    protected void E_OnEndStoping(string p_args)
    {
        if (OnEndStoping != null)
            OnEndStoping(p_args);
    }
    //UNLOADING
    protected void E_OnStartUnloading(string p_args)
    {
        if (OnStartUnloading != null)
            OnStartUnloading(p_args);
    }
    protected void E_OnEndUnloading(string p_args)
    {
        if (OnEndUnloading != null)
            OnEndUnloading(p_args);
    }
    //DESTROYING
    protected void E_OnStartDestroying(string p_args)
    {
        if (OnStartDestroying != null)
            OnStartDestroying(p_args);
    }
    protected void E_OnEndDestroying(string p_args)
    {
        if (OnEndDestroying != null)
            OnEndDestroying(p_args);
    }
    #endregion
    #region LIFECYCLE FIRE COROUTINES
    protected void FireRoutine(IEnumerator p_routine)
    {
        GameManager.singleton.StartCouroutineInGameManager(p_routine);
    }
    public virtual void Init()
    {
        
    }
    public virtual void Load()
    {
        
    }
    public virtual void Begin()
    {
        
    }
    public virtual void Pause()
    {
        
    }
    public virtual void Update()
    {
        
    }
    public virtual void Resume()
    {
        
    }
    public virtual void Stop()
    {
        
    }
    public virtual void Unload()
    {
        
    }
    public virtual void Destroy()
    {
        
    }
    #endregion
    public void SubscribeToLifeCycle()
    {
        OnEndInitializing += (string p_args) =>
        {
            currentLifeCycleState = LIFECYCLESTATE.INITIALIZED;
        };
        OnEndLoading += (string p_args) =>
        {
            currentLifeCycleState = LIFECYCLESTATE.LOADED;
        };
        OnEndPausing += (string p_args) =>
        {
            currentLifeCycleState = LIFECYCLESTATE.PAUSED;
        };
        OnEndResumeing += (string p_args) =>
        {
            currentLifeCycleState = LIFECYCLESTATE.RUNNING;
        };
        OnEndStoping += (string p_args) =>
        {
            currentLifeCycleState = LIFECYCLESTATE.STOPED;
        };
        OnEndUnloading += (string p_args) =>
        {
            currentLifeCycleState = LIFECYCLESTATE.KILLED;
        };
    }
    /*public AbstractLifeCycle()
    {
        SubscribeToLifeCycle();
    }*/
}
