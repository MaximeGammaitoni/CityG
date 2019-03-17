using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using System.Threading.Tasks;

public class GazTipsGateway : AbstractLifeCycle {
    #region EVENTS
    public delegate void GazTipsGatewayEvent(string p_index, string p_data);
    #endregion
    #region PROTECTED PROPERTIES
    protected FireBaseClient firebaseClient;
    #endregion
    #region PUBLIC PROPERTIES
    #endregion
    #region PROTECTED METHODS
    #region LIFECYCLE IENUMERATOR METHODS
    protected IEnumerator _Init()
    {
        E_OnStartInitializing("");
        yield return null;
        //Code Here
        yield return null;
        E_OnEndInitializing("");
    }
    protected IEnumerator _Load()
    {
        E_OnStartLoading("");
        yield return null;
        //Code Here
        yield return null;
        E_OnEndLoading("");
    }
    protected IEnumerator _Begin()
    {
        E_OnStartBeginning("");
        yield return null;
        //Code Here
        yield return null;
        E_OnEndBeginning("");
    }
    protected IEnumerator _Pause()
    {
        E_OnStartPausing("");
        yield return null;
        //Code Here
        yield return null;
        E_OnEndPausing("");
    }
    protected IEnumerator _Update()
    {
        E_OnStartUpdating("");
        yield return null;
        //Code Here
        yield return null;
        E_OnEndUpdating("");
    }
    protected IEnumerator _Resume()
    {
        E_OnStartResumeing("");
        yield return null;
        //Code Here
        yield return null;
        E_OnEndResumeing("");
    }
    protected IEnumerator _Stop()
    {
        E_OnStartStoping("");
        yield return null;
        //Code Here
        yield return null;
        E_OnEndStoping("");
    }
    protected IEnumerator _Unload()
    {
        E_OnStartUnloading("");
        yield return null;
        //Code Here...
        yield return null;
        E_OnEndUnloading("");
    }
    protected IEnumerator _Destroy()
    {
        E_OnStartUnloading("");
        yield return null;
        //Code Here...
        yield return null;
        E_OnEndUnloading("");
    }
    #endregion
    #endregion
    #region PUBLIC METHODS
    #region LIFECYCLE CALLS
    public override void Init()
    {
        FireRoutine(_Init());
    }
    public override void Load()
    {
        FireRoutine(_Load());
    }
    public override void Begin()
    {
        FireRoutine(_Begin());
    }
    public override void Pause()
    {
        FireRoutine(_Pause());
    }
    public override void Update()
    {
        FireRoutine(_Update());
    }
    public override void Resume()
    {
        FireRoutine(_Resume());
    }
    public override void Stop()
    {
        FireRoutine(_Stop());
    }
    public override void Unload()
    {
        FireRoutine(_Unload());
    }
    public override void Destroy()
    {
        FireRoutine(_Destroy());
    }
    #endregion
    public Task<DataSnapshot> GET_RandomGazTips()
    {
        Task<DataSnapshot> l_output = firebaseClient.GET_GazTipsList();
        return l_output;/*
        Func<bool> l_wait = () =>
        {
            return (l_output.IsCompleted||l_output.IsCanceled||l_output.IsCompleted) ? true : false;
        };
        yield return new WaitUntil(l_wait);*/

    }
    #endregion
    #region CONSTRUCTOR
    public GazTipsGateway(FireBaseClient p_firebaseClient)
    {
        SubscribeToLifeCycle();
        firebaseClient = p_firebaseClient;
    }
    #endregion

}