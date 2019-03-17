using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBonusesManager : AbstractLifeCycle
{
    #region EVENTS
    public delegate void PlayerBonusManagerEvent(string[] p_args);
    public event PlayerBonusManagerEvent OnBonusReceived;
    public event PlayerBonusManagerEvent OnBonusUnlocked;
    public event PlayerBonusManagerEvent OnBonusUsed;

    public event PlayerBonusManagerEvent OnWaitingBonusListAssigned;
    public event PlayerBonusManagerEvent OnWaitingBonusListAdded;
    public event PlayerBonusManagerEvent OnWaitingBonusListRemoved;

    public event PlayerBonusManagerEvent OnUnlockedListAssigned;
    public event PlayerBonusManagerEvent OnUnlockedListAdded;
    public event PlayerBonusManagerEvent OnUnlockedListRemoved;
    #endregion
    #region PROTECTED PROPERTIES
    protected List<CityGazBonus> _waitingList;
    protected List<CityGazBonus> _unlockedList;
    protected PlayerBonusesGateway _gateway;
    #endregion
    #region PUBLIC PROPERTIES
    #endregion
    #region PROTECTED METHODS
    protected void E_OnBonusReceived(string[] p_args = null)
    {
        if (OnBonusReceived != null)
        {
            OnBonusReceived(p_args);
        }
    }
    protected void E_OnBonusUnlocked(string[] p_args = null)
    {
        if (OnBonusUnlocked != null)
        {
            OnBonusUnlocked(p_args);
        }
    }
    protected void E_OnBonusUsed(string[] p_args = null)
    {
        if (OnBonusUsed != null)
        {
            OnBonusUsed(p_args);
        }
    }

    protected void E_OnWaitingBonusListAssigned(string[] p_args = null)
    {
        if (OnWaitingBonusListAssigned != null)
        {
            OnWaitingBonusListAssigned(p_args);
        }
    }
    protected void E_OnWaitingBonusListAdded(string[] p_args = null)
    {
        if (OnWaitingBonusListAdded != null)
        {
            OnWaitingBonusListAdded(p_args);
        }
    }
    protected void E_OnWaitingBonusListRemoved(string[] p_args = null)
    {
        if (OnWaitingBonusListRemoved != null)
        {
            OnWaitingBonusListRemoved(p_args);
        }
    }

    protected void E_OnUnlockedBonusListAssigned(string[] p_args = null)
    {
        if (OnUnlockedListAssigned != null)
        {
            OnUnlockedListAssigned(p_args);
        }
    }
    protected void E_OnUnlockedBonusListAdded(string[] p_args = null)
    {
        if (OnUnlockedListAdded != null)
        {
            OnUnlockedListAdded(p_args);
        }
    }
    protected void E_OnUnlockedBonusListRemoved(string[] p_args = null)
    {
        if (OnUnlockedListRemoved != null)
        {
            OnUnlockedListRemoved(p_args);
        }
    }
    #region LIFECYCLE IENUMERATOR METHODS
    //CALLED BY PUBLIC L_Init()
    protected IEnumerator _Init()
    {
        E_OnStartInitializing("");
        yield return null;
        //Code Here...
        _gateway.Init();
        yield return new WaitUntil(()=> { return _gateway.currentLifeCycleState == LIFECYCLESTATE.INITIALIZED; });
        E_OnEndInitializing("");
    }
    //CALLED BY PUBLIC L_Load()
    protected IEnumerator _Load()
    {
        E_OnStartLoading("");
        yield return null;
        //Code Here...
        yield return null;
        E_OnEndLoading("");
    }
    //CALLED BY PUBLIC L_Begin()
    protected IEnumerator _Begin()
    {
        E_OnStartBeginning("");
        yield return null;
        E_OnEndBeginning("");
    }
    //CALLED BY PUBLIC L_Pause()
    protected IEnumerator _Pause()
    {
        E_OnStartPausing("");
        yield return null;
        E_OnEndPausing("");
    }
    //CALLED BY PUBLIC L_Update()
    protected IEnumerator _Update()
    {
        E_OnStartUpdating("");
        yield return null;
        E_OnEndUpdating("");
    }
    //CALLED BY PUBLIC L_Resume()
    protected IEnumerator _Resume()
    {
        E_OnStartResumeing("");
        yield return null;
        E_OnEndResumeing("");
    }
    //CALLED BY PUBLIC L_Stop()
    protected IEnumerator _Stop()
    {
        E_OnStartStoping("");
        yield return null;
        E_OnEndStoping("");
    }
    //CALLED BY PUBLIC L_Unload()
    protected IEnumerator _Unload()
    {
        E_OnStartUnloading("");
        yield return null;
        E_OnEndUnloading("");
    }
    //CALLED BY PUBLIC L_Destroy()
    protected IEnumerator _Destroy()
    {
        E_OnStartUnloading("");
        yield return null;
        E_OnEndUnloading("");
    }
    #endregion
    #region HANDLERS

    #endregion
    #endregion
    #region PUBLIC METHODS
    //GET
    public List<CityGazBonus> GET_WaitingList()
    {
        if (_waitingList == null)
            _waitingList = new List<CityGazBonus>();
        return _waitingList;
    }
    public List<CityGazBonus> GET_UnlockedList()
    {
        if (_unlockedList == null)
            _unlockedList = new List<CityGazBonus>();
        return _unlockedList;
    }
    //SET
    public void SET_WaitingList(List<CityGazBonus> p_waitingList)
    {
        if (p_waitingList != null)
        {
            _waitingList = p_waitingList;
            E_OnWaitingBonusListAssigned();
        }
        else
        {
            _waitingList = new List<CityGazBonus>();
            E_OnWaitingBonusListAssigned();
        }
    }
    public void SET_UnlockedList(List<CityGazBonus> p_unlockedList)
    {
        if (p_unlockedList != null)
        {
            _unlockedList = p_unlockedList;
            E_OnUnlockedBonusListAssigned();
        }
        else
        {
            _unlockedList = new List<CityGazBonus>();
            E_OnUnlockedBonusListAssigned();
        }
    }
    //ACTIONS
    public void AddToWaitingList(CityGazBonus p_bonus)
    {
        _gateway.TELL_AddBonusToPlayerWaitingList(p_bonus);
        //GET_WaitingList().Add(p_bonus);
        E_OnWaitingBonusListAdded();
    }
    public void AddToUnlockedList(CityGazBonus p_bonus)
    {
        _gateway.TELL_AddBonusToPlayerUnlockedList(p_bonus);
        RemoveFromWaitingList(GET_WaitingList().FindIndex(x=>x.GET_Bonus().GET_KEYID()==p_bonus.GET_Bonus().GET_KEYID()));
        //GET_UnlockedList().Add(p_bonus);
        E_OnUnlockedBonusListAdded(new string[1] { p_bonus.GET_Bonus().GET_ID().ToString()});
    }
    public CityGazBonus RemoveFromWaitingList(int p_ID)
    {
        CityGazBonus l_output = null;
        if (GET_WaitingList()[p_ID] != null)
        {
            l_output = GET_WaitingList()[p_ID];
            GET_WaitingList().RemoveAt(p_ID);
            Debug.Log("PlayerBonusManager-> Removed Bonus From Waiting List (ID was :" + p_ID.ToString() + ")");
            E_OnWaitingBonusListRemoved();
        }
        _gateway.TELL_RemoveBonusFromPlayerWaitingList(l_output.GET_Bonus().GET_KEYID());

        return l_output;
    }
    public CityGazBonus RemoveFromUnlockedList(int p_bonusID)
    {
        CityGazBonus l_output = null;
        if (GET_UnlockedList() != null)
        {
            int l_index = GET_UnlockedList().FindIndex(x => x.GET_Bonus().GET_ID() == p_bonusID);
            l_output = GET_UnlockedList()[l_index];
            GET_UnlockedList().RemoveAt(l_index);
            Debug.Log("PlayerBonusManager-> Removed Bonus From Unlocked List (ID was :" + p_bonusID.ToString() + ")");
            E_OnUnlockedBonusListRemoved();
        }
        _gateway.TELL_RemoveBonusFromPlayerUnlockedList(l_output.GET_Bonus().GET_KEYID());
        return l_output;
    }
    public void StartUnlocking(CityGazBonus p_bonus)
    {
        _gateway.TELL_StartTimerDateTimeForPlayerWaitingList(p_bonus.GET_Bonus().GET_KEYID());
        p_bonus.GET_Lock().SET_StartedUnlockDateTime(System.DateTime.Now);
    }
    public void SaveLocalUnlockedList()
    {
        
    }
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
    #endregion
    #region CONSTRUCTORS
    public PlayerBonusesManager(FireBaseClient p_firebase, List<CityGazBonus> p_waitingList = null, List<CityGazBonus> p_unlockedList = null)
    {
        SubscribeToLifeCycle();
        SET_WaitingList(p_waitingList);
        SET_UnlockedList(p_unlockedList);
        _gateway = new PlayerBonusesGateway(p_firebase);
    }
    #endregion
}