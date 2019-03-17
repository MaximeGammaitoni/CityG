using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractClient: ILifeCyclable {
    #region EVENTS
    public delegate void ClientDelegate(object p_object, EventArgs p_args);
    public event ClientDelegate OnURLToDatabaseSet;
    public event ClientDelegate OnUserCredentialsSet;
    public event ClientDelegate OnConnectionSet;
    public event ClientDelegate OnConnectionLost;
    public event ClientDelegate OnDisconnection;
    public event ClientDelegate OnRequestSet;
    public event ClientDelegate OnRequestStarted;
    public event ClientDelegate OnRequestFinished;
    public event ClientDelegate OnRequestSucceed;
    public event ClientDelegate OnRequestFailed;
    public event ClientDelegate OnRequestAborted;

    public delegate void ClientLifeDelegate(object p_object, EventArgs p_args);
    public event ClientLifeDelegate OnInitialized;
    public event ClientLifeDelegate OnLoaded;
    public event ClientLifeDelegate OnStarted;
    public event ClientLifeDelegate OnPaused;
    public event ClientLifeDelegate OnResumed;
    public event ClientLifeDelegate OnStoped;
    public event ClientLifeDelegate OnUnloaded;
    public event ClientLifeDelegate OnKilled;
    #endregion
    #region PROTECTED PROPERTIES
    protected List<AbstractRequest> _waitingRequests;
    protected List<AbstractRequest> _completedRequests;
    protected List<AbstractRequest> _failedRequests;
    protected LifeCycleAssistant _lifeCycleAssistant;
    #endregion
    #region PUBLIC PROPERTIES
    public bool isConnected;
    public string urlToDatabase;
    public AbstractAuthentication userCredentials;
    #endregion
    #region PROTECTED METHODS
    #endregion
    #region PUBLIC METHODS
    public abstract void ConnectToDatabase();
    public abstract void DisconnectFromDatabase();

    public abstract AbstractAuthentication SET_UserCredentials(AbstractAuthentication p_abstractAuthentication);
    public abstract AbstractAuthentication GET_UserCredentials();
    public abstract bool IsAccessPermitted(AbstractAuthentication p_credentials, AbstractRequest p_request);

    public abstract void GET_Data(Dictionary<string,string> p_args, AbstractRequest.AbstractRequestDelegate p_delegate);
    public abstract void GET_Data(AbstractRequest p_request);
    public abstract void SET_Data(Dictionary<string,string> p_args, AbstractRequest.AbstractRequestDelegate p_delegate);
    public abstract void SET_Data(AbstractRequest p_request);

    public virtual string SET_URLToDatabase(string p_url)
    {
        string l_output = p_url.IsNullOrEmpty() ? "isNullOrEmpty" : p_url;
        urlToDatabase = p_url != "isNullOrEmpty" ? p_url : urlToDatabase;
        return urlToDatabase;
    }

    public virtual List<AbstractRequest> GET_WaitingRequests()
    {
        return _waitingRequests;
    }
    public virtual List<AbstractRequest> GET_CompletedRequests()
    {
        return _completedRequests;
    }
    public virtual List<AbstractRequest> GET_FailedRequests()
    {
        return _failedRequests;
    }

    public virtual LifeCycleAssistant GET_LifeCycleAssistant()
    {
        return _lifeCycleAssistant;
    }
    public virtual LifeCycleAssistant SET_LifeCycleAssistant()
    {
        _lifeCycleAssistant = new LifeCycleAssistant();
        return _lifeCycleAssistant;
    }

    #region LifeAssistant
    public abstract void Initialize();
    public abstract void Load();
    public abstract void Start();
    public abstract void Pause();
    public abstract void Resume();
    public abstract void Stop();
    public abstract void Unload();
    public abstract void Quit();
    public abstract void Kill();
    #endregion

    #endregion
    #region CONSTRUCTORS
    public AbstractClient(string p_urlToDatabase)
    {
        //SET URL
        SET_URLToDatabase(p_urlToDatabase);
        //SET LIFECYCLE
        SET_LifeCycleAssistant();
    }
    #endregion
}