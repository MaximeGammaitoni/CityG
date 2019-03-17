using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractTimer : ITimeable
{
    #region EVENTS
    public delegate void AbstractTimerEvent(string[] p_args);
    public event AbstractTimerEvent OnStarted;
    public event AbstractTimerEvent OnPaused;
    public event AbstractTimerEvent OnResumed;
    public event AbstractTimerEvent OnStopped;
    public event AbstractTimerEvent OnBaseTimeUpdated;
    public event AbstractTimerEvent OnRemainingTimeUpdated;
    public event AbstractTimerEvent OnTimerReachedZero;
    #endregion
    #region PROTECTED PROPERTIES
    protected TimeSpan _baseTime;
    protected TimeSpan _remainingTime;
    protected TimeSpan _deltaTime;
    protected TIMER_STATES _currentState;
    protected Dictionary<string, IEnumerator> _coroutines;
    #endregion
    #region PUBLIC PROPERTIES
    public enum TIMER_STATES { NULL, RUNNING, STOPPED, PAUSED }
    #endregion
    #region PROTECTED METHODS
    protected void E_OnStarted(string[] p_args = null)
    {
        if (OnStarted != null)
        {
            OnStarted(p_args);
        }
    }
    protected void E_OnPaused(string[] p_args = null)
    {
        if (OnPaused != null)
        {
            OnPaused(p_args);
        }
    }
    protected void E_OnResumed(string[] p_args = null)
    {
        if (OnResumed != null)
        {
            OnResumed(p_args);
        }
    }
    protected void E_OnStopped(string[] p_args = null)
    {
        if (OnStopped != null)
        {
            OnStopped(p_args);
        }
    }
    protected void E_OnBaseTimeUpdated(string[] p_args = null)
    {
        if (OnBaseTimeUpdated != null)
        {
            OnBaseTimeUpdated(p_args);
        }
    }
    protected void E_OnRemainingTimeUpdated(string[] p_args = null)
    {
        if (OnRemainingTimeUpdated != null)
        {
            OnRemainingTimeUpdated(p_args);
        }
    }
    protected void E_OnTimerReachedZero(string[] p_args = null)
    {
        if (OnTimerReachedZero != null)
        {
            OnTimerReachedZero(p_args);
        }
    }
    protected void ChangeState(TIMER_STATES p_newState)
    {
        if (_currentState == p_newState)
        {
            return;
        }

        switch (p_newState)
        {
            case TIMER_STATES.NULL:
                break;
            case TIMER_STATES.RUNNING:
                break;
            case TIMER_STATES.PAUSED:
                break;
            case TIMER_STATES.STOPPED:
                break;
        }

        _currentState = p_newState;
    }
    protected virtual void Update()
    {
        if (_deltaTime != null)
        {
            _deltaTime = CalculateDeltaTime();
        }
        UpdateRemainingTime();
    }
    protected virtual TimeSpan CalculateDeltaTime()
    {
        TimeSpan l_output = TimeSpan.FromSeconds((double)Time.deltaTime);
        return l_output;
    }
    protected virtual void UpdateRemainingTime()
    {
        _remainingTime = _remainingTime.Subtract(_deltaTime);
        if (_remainingTime.TotalMilliseconds<=0)
        {
            E_OnTimerReachedZero();
            Stop();
        }
    }
    #endregion
    #region PUBLIC METHODS
    #region GET
    public TimeSpan GET_BaseTime()
    {
        return _baseTime;
    }
    public TimeSpan GET_RemainingTime()
    {
        return _remainingTime;
    }
    public TIMER_STATES GET_CurrentState()
    {
        return _currentState;
    }
    #endregion
    #region SET
    public void SET_BaseTime(TimeSpan p_value)
    {
        if (p_value!= null)
        {
            _baseTime = p_value;
        }
    }
    public void SET_RemainingTime(TimeSpan p_value)
    {
        if (p_value <= GET_BaseTime())
        {
            _remainingTime = p_value;
        }
        else
        {
            _remainingTime = GET_BaseTime();
        }
    }
    #endregion
    #region ACTIONS
    public virtual void Pause()
    {
        E_OnPaused();
        ChangeState(TIMER_STATES.PAUSED);
    }
    public virtual void Reset()
    {
        ChangeState(TIMER_STATES.NULL);
    }
    public virtual void Resume()
    {
        E_OnResumed();
        ChangeState(TIMER_STATES.RUNNING);
    }
    public virtual void Start()
    {
        E_OnStarted();
        ChangeState(TIMER_STATES.RUNNING);
    }
    public virtual void Stop()
    {
        E_OnStopped();
        ChangeState(TIMER_STATES.STOPPED);
    }
    #endregion
    #endregion
    #region CONSTRUCTORS
    public AbstractTimer(TimeSpan p_baseTime, TimeSpan p_remainingTime)
    {
        SET_BaseTime(p_baseTime);
        SET_RemainingTime(p_remainingTime);
    }
    #endregion
}