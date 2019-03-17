using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class AbstractTutorial : AbstractLifeCycle, ITutorial
{
    #region EVENTS
    public delegate void AbstractTutorialEvent(string[] p_args);
    public event AbstractTutorialEvent OnTutorialSequenceUpdated;
    public event AbstractTutorialEvent OnTutorialNameUpdated;
    public event AbstractTutorialEvent OnTutorialCurrentObjectiveCompleted;
    #endregion
    #region PROTECTED PROPERTIES
    protected string _tutorialName;
    protected int _sequenceIndex;
    #endregion
    #region PUBLIC PROPERTIES
    public string tutorialName
    {
        get
        {
            return _tutorialName;
        }
        set
        {
            if (!value.IsNullOrEmpty())
            {
                _tutorialName = value;
            }
        }
    }
    public int sequenceIndex
    {
        get
        {
            return _sequenceIndex;
        }
        set
        {
            if(sequences !=null && (value >=0 && value < sequences.Count))
            {
                _sequenceIndex = value;
            }
        }
    }
    public List<AbstractSequence> sequences;
    #endregion
    #region PROTECTED METHODS
    #region EVENTS
    protected virtual void ClearTutorialEvents()
    {
        OnTutorialSequenceUpdated = null;
        OnTutorialNameUpdated = null;
        OnTutorialCurrentObjectiveCompleted = null;
    }
    protected void E_OnTutorialNameUpdated(string[] p_args = null)
    {
        if (OnTutorialNameUpdated != null)
        {
            OnTutorialNameUpdated(p_args);
        }
    }
    protected void E_OnTutorialSequenceUpdated(string[] p_args = null)
    {
        if (OnTutorialSequenceUpdated != null)
        {
            OnTutorialSequenceUpdated(p_args);
        }
    }
    protected void E_OnTutorialCurrentObjectiveCompleted(string[] p_args = null)
    {
        if (OnTutorialCurrentObjectiveCompleted != null)
        {
            OnTutorialCurrentObjectiveCompleted(p_args);
        }
    }
    #endregion
    #endregion
    #region PUBLIC METHODS
    public abstract void StartSequence(int p_index);
    public abstract void StopSequence(int p_index);
    public abstract void GoToSequence(int p_index);
    public abstract void SetSequences(List<AbstractSequence> p_sequences);
    #endregion
    #region CONSTRUCTORS
    public AbstractTutorial(string p_tutorialName = "tutorial_NA")
    {
        tutorialName = p_tutorialName;
    }
    #endregion
}
public abstract class AbstractSequence
{
    #region EVENTS
    public delegate void AbstractSequenceEvent(string[] p_args);
    public event AbstractSequenceEvent OnSequenceStarted;
    public event AbstractSequenceEvent OnSequenceStopped;
    public event AbstractSequenceEvent OnSequenceCompleted;
    #endregion
    #region PROTECTED PROPERTIES
    protected bool isCurrentSequence;
    protected bool isCompleted;
    protected string sequenceName;
    #endregion
    #region PUBLIC PROPERTIES
    #endregion
    #region PROTECTED METHODS
    protected abstract void SequenceCompleteLogic();
    #region EVENTS
    protected void ClearEvents()
    {
        OnSequenceStarted = null;
        OnSequenceStopped = null;
        OnSequenceCompleted = null;
    }
    protected void E_OnSequenceStarted(string[] p_args = null)
    {
        if (OnSequenceStarted != null)
        {
            OnSequenceStarted(p_args);
        }
    }
    protected void E_OnSequenceStopped(string[] p_args = null)
    {
        if (OnSequenceStopped != null)
        {
            OnSequenceStopped(p_args);
        }
    }
    protected void E_OnSequenceCompleted(string[] p_args = null)
    {
        if (OnSequenceCompleted != null)
        {
            OnSequenceCompleted(p_args);
        }
    }
    #endregion
    #endregion
    #region PUBLIC METHODS
    public abstract void Start();
    public abstract void Stop();
    public abstract bool GoNext();
    public abstract void End();
    public bool GetSequenceCurrentlyActive()
    {
        return isCurrentSequence;
    }
    public string GetSequenceName()
    {
        return sequenceName;
    }
    public bool GetSequenceCompletion()
    {
        return isCompleted;
    }
    public void SetCompletion(bool p_value)
    {
        isCompleted = p_value;
        if (isCompleted)
            E_OnSequenceCompleted();
    }
    #endregion
    #region CONSTRUCTORS
    public AbstractSequence()
    {

    }
    #endregion
}
public class SequenceDialogs : AbstractSequence
{
    #region EVENTS
    public delegate void SequenceDialogEvent(string[] p_args);
    public event SequenceDialogEvent OnCurrentDialogIndexUpdated;
    public event SequenceDialogEvent OnGoToEndDialog;
    public event SequenceDialogEvent OnGoToNextDialog;
    public event SequenceDialogEvent OnDialogOpened;
    public event SequenceDialogEvent OnDialogClosed;
    #endregion
    #region PROTECTED PROPERTIES
    protected List<GameObject> dialogs;
    protected List<bool> dialogsShown;
    protected int currentDialogIndex;
    #endregion
    #region PUBLIC PROPERTIES
    #endregion
    #region PROTECTED METHODS
    protected bool dialogsAreAvailables()
    {
        if (dialogs != null && dialogs.Count > 0)
            return true;
        return false;
    }
    protected bool indexHasDialog(int p_index)
    {
        if (dialogsAreAvailables() && dialogs[p_index] != null)
            return true;
        return false;
    }
    protected override void SequenceCompleteLogic()
    {
        //If all dialogs has been seen then
        SetCompletion(true);
        Stop();
    }
    #region EVENTS
    protected void E_OnCurrentDialogIndexUpdated(string[] p_args = null)
    {
        if (OnCurrentDialogIndexUpdated != null)
        {
            OnCurrentDialogIndexUpdated(p_args);
        }
    }
    protected void E_OnDialogOpened(string[] p_args = null)
    {
        if (OnDialogOpened != null)
        {
            OnDialogOpened(p_args);
        }
    }
    protected void E_OnDialogClosed(string[] p_args = null)
    {
        if (OnDialogClosed != null)
        {
            OnDialogClosed(p_args);
        }
    }
    protected void E_OnGoToNextDialog(string[] p_args= null)
    {
        if (OnGoToNextDialog != null)
        {
            OnGoToNextDialog(p_args);
        }
    }
    protected void E_OnGoToEndDialog(string[] p_args = null)
    {
        if (OnGoToEndDialog != null)
        {
            OnGoToEndDialog(p_args);
        }
    }

    protected void HandleOnGoToNextDialog(string[] p_args)
    {
        
    }
    protected void HandleOnGoToEndDialog(string[] p_args)
    {
        
    }
    #endregion
    #endregion
    #region PUBLIC METHODS
    public void ClearDialogsEvents()
    {
        OnCurrentDialogIndexUpdated = null;
        OnGoToNextDialog = null;
        OnGoToEndDialog = null;
        OnDialogOpened = null;
        OnDialogClosed = null;
    }
    public override void Start()
    {
        if (!dialogsAreAvailables() || !indexHasDialog(currentDialogIndex))
            return;
        E_OnSequenceStarted(new string[2] { "Dialogue", "Started" });
        OpenDialog(currentDialogIndex);
    }
    public override void Stop()
    {
        if (!dialogsAreAvailables() || !indexHasDialog(currentDialogIndex))
            return;
        E_OnSequenceStopped(new string[2] { "Dialogue", "Stopped" });
        for(int i = 0; i < dialogs.Count; i++)
        {
            CloseDialog(i);
        }
    }
    public override void End()
    {
        E_OnGoToEndDialog();
        SequenceCompleteLogic();
    }
    public override bool GoNext()
    {
        if (!dialogsAreAvailables() || !indexHasDialog(currentDialogIndex + 1))
            return false;
        if (SetDialogIndex(currentDialogIndex + 1))
        {
            E_OnGoToNextDialog();
            OpenDialog(currentDialogIndex);
            return true;
        }
        return false;
    }
    public virtual void OpenDialog(int p_index)
    {
        if (!dialogsAreAvailables() || !indexHasDialog(p_index))
            return;
        dialogs[p_index].SetActive(true);
        dialogs[p_index].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        E_OnDialogOpened();
    }
    public virtual void CloseDialog(int p_index)
    {
        if (!dialogsAreAvailables() || !indexHasDialog(p_index))
            return;
        dialogs[p_index].SetActive(false);
        dialogs[p_index].GetComponent<RectTransform>().anchoredPosition = new Vector2(-2000, 0);
        E_OnDialogClosed();
    }
    /*
     * Used to set the Dialog's list.
     * What it does :
     * - Check if passed dialog's list is valid
     * - Set passed dialog's list as new dialog's list
     * - Reset dialogsShown's list
     * - Get & Set each onClick's method for dialog's button 
     * */
    public virtual bool SetDialogs(List<GameObject> p_dialogs)
    {
        if (p_dialogs == null || p_dialogs.Count == 0)
            return false;
        dialogs = p_dialogs;
        dialogsShown = new List<bool>();
        return true;
    }
    /*
     * Used to change the current Dialog Index.
     * What it does :
     * - Check if index is valid
     * - Stop currently played dialog
     * - Start new index's dialog
     * */
    public bool SetDialogIndex(int p_index)
    {
        if (!dialogsAreAvailables() || !indexHasDialog(p_index))
            return false;
        Debug.Log("SetDialogIndex: "+p_index);
        CloseDialog(currentDialogIndex);
        currentDialogIndex = p_index;
        E_OnCurrentDialogIndexUpdated();
        return true;
    }
    #endregion
    #region CONSTRUCTORS
    /*
     * All parameters are optionals.
     * What it does :
     *  - Set the passed sequenceName
     *  - Set the passed index as currentDialogIndex
     *  - Set the passed dialog's list as new Dialog's list 
     * */
    public SequenceDialogs(string p_sequenceName = "N/A", int p_currentDialogIndex = 0, List<GameObject> p_dialogs = null)
    {
        sequenceName = p_sequenceName;
        SetDialogs(p_dialogs);
        SetDialogIndex(p_currentDialogIndex);
        OnGoToEndDialog += HandleOnGoToEndDialog;
    }
    #endregion
}