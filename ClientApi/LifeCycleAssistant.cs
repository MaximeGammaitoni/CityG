using System.Collections.Generic;
using UnityEngine;

public class LifeCycleAssistant {
    #region EVENTS
    public delegate void LifeCycleAssistantDelegate(object p_object);
    public event LifeCycleAssistantDelegate OnInitialized;
    public event LifeCycleAssistantDelegate OnStarted;
    public event LifeCycleAssistantDelegate OnLoaded;
    public event LifeCycleAssistantDelegate OnRunning;
    public event LifeCycleAssistantDelegate OnPaused;
    public event LifeCycleAssistantDelegate OnResumed;
    public event LifeCycleAssistantDelegate OnStopped;
    public event LifeCycleAssistantDelegate OnUnloaded;
    public event LifeCycleAssistantDelegate OnUninitialized;
    #endregion
    #region PUBLIC PROPERTIES
    public Dictionary<string, LifeCycleAssistantDelegate> delegates;
    #endregion
    #region PROTECTED PROPERTIES
    protected bool _isInitialized;
    protected bool _isLoaded;
    protected bool _isRunning;
    protected bool _isPaused;
    #endregion
    #region PUBLIC METHODS
    #region GET
    public bool GET_Initialized()
    {
        return _isInitialized;
    }
    public bool GET_Loaded()
    {
        return _isLoaded;
    }
    public bool GET_Running()
    {
        return _isRunning;
    }
    public bool GET_Paused()
    {
        return _isPaused;
    }
    #endregion
    #region SET
    public bool SET_Initialized(bool p_value)
    {
        if (p_value == _isInitialized)
            return _isInitialized;

        _isInitialized = p_value;
        if (_isInitialized)
            E_OnInitialized();
        else
            E_OnUninitialized();
        return _isInitialized;
    }
    public bool SET_Loaded(bool p_value)
    {
        if (p_value == _isLoaded)
            return _isLoaded;

        _isLoaded = p_value;
        if (_isLoaded)
            E_OnLoaded();
        else
            E_OnUnloaded();
        return _isLoaded;
    }
    public bool SET_Running(bool p_value)
    {
        if (p_value == _isRunning)
            return _isRunning;

        _isRunning = p_value;
        if (_isRunning)
            E_OnStarted();
        else
            E_OnStopped();
        return _isRunning;
    }
    public bool SET_Paused(bool p_value)
    {
        if (p_value == _isPaused)
            return _isPaused;

        _isPaused = p_value;

        if (_isPaused)
            E_OnPaused();
        else
            E_OnResumed();
        return _isPaused;
    }
    #endregion
    #region ACTIONS
    public void AddListenerToInitialize(string p_delegateID, LifeCycleAssistantDelegate p_delegate)
    {
        delegates.Add(p_delegateID, p_delegate);
        OnInitialized += p_delegate;
    }
    public void AddListenerToLoad(string p_delegateID, LifeCycleAssistantDelegate p_delegate)
    {
        delegates.Add(p_delegateID, p_delegate);
        OnLoaded += p_delegate;
    }
    public void AddListenerToStart(string p_delegateID, LifeCycleAssistantDelegate p_delegate)
    {
        delegates.Add(p_delegateID, p_delegate);
        OnStarted += p_delegate;
    }
    public void AddListenerToPause(string p_delegateID, LifeCycleAssistantDelegate p_delegate)
    {
        delegates.Add(p_delegateID, p_delegate);
        OnPaused += p_delegate;
    }
    public void AddListenerToResume(string p_delegateID, LifeCycleAssistantDelegate p_delegate)
    {
        delegates.Add(p_delegateID, p_delegate);
        OnResumed += p_delegate;
    }
    public void AddListenerToStop(string p_delegateID, LifeCycleAssistantDelegate p_delegate)
    {
        delegates.Add(p_delegateID, p_delegate);
        OnStopped += p_delegate;
    }
    public void AddListenerToUninitialize(string p_delegateID, LifeCycleAssistantDelegate p_delegate)
    {
        delegates.Add(p_delegateID, p_delegate);
        OnUninitialized += p_delegate;
    }
    #endregion
    #endregion
    #region PROTECTED METHODS
    protected void E_OnInitialized(object p_object = null)
    {
        if (OnInitialized != null)
        {
            OnInitialized(p_object);
        }
    }
    protected void E_OnStarted(object p_object = null)
    {
        if (OnStarted != null)
        {
            OnStarted(p_object);
        }
    }
    protected void E_OnLoaded(object p_object = null)
    {
        if (OnLoaded != null)
        {
            OnLoaded(p_object);
        }
    }
    protected void E_OnRunning(object p_object = null)
    {
        if (OnRunning != null)
        {
            OnRunning(p_object);
        }
    }
    protected void E_OnPaused(object p_object = null)
    {
        if (OnPaused != null)
        {
            OnPaused(p_object);
        }
    }
    protected void E_OnResumed(object p_object = null)
    {
        if (OnResumed != null)
        {
            OnResumed(p_object);
        }
    }
    protected void E_OnUnloaded(object p_object = null)
    {
        if (OnUnloaded != null)
        {
            OnUnloaded(p_object);
        }
    }
    protected void E_OnStopped(object p_object = null)
    {
        if (OnStopped != null)
        {
            OnStopped(p_object);
        }
    }
    protected void E_OnUninitialized(object p_object = null)
    {
        if (OnUninitialized != null)
        {
            OnUninitialized(p_object);
        }
    }
    #endregion
    #region CONSTRUCTORS
    public LifeCycleAssistant()
    {
        delegates = new Dictionary<string, LifeCycleAssistantDelegate>();
    }
    #endregion
}