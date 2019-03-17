using System;
using UnityEngine;
public abstract class AbstractBonus : IBonus
{
    #region EVENTS
    public delegate void AbstractBonusEvent(string[] p_args);
    public event AbstractBonusEvent OnSelected;
    public event AbstractBonusEvent OnDeselected;
    public event AbstractBonusEvent OnUsed;
    public event AbstractBonusEvent OnAmountUsageUpdated;
    public event AbstractBonusEvent OnDescriptionUpdated;
    public event AbstractBonusEvent OnNameUpdated;
    #endregion
    #region PROTECTED PROPERTIES
    protected int _amountUsage;
    protected string _description;
    protected string _name;
    protected int _ID;
    protected string _KEYID;
    protected DateTime _receivedDateTime;
    protected bool _selected;
    #endregion
    #region PUBLIC PROPERTIES
    #endregion
    #region PROTECTED METHODS
    protected void E_OnSelected(string[] p_args = null)
    {
        if (OnSelected != null)
        {
            OnSelected(p_args);
        }
    }
    protected void E_OnDeselected(string[] p_args = null)
    {
        if (OnDeselected != null)
        {
            OnDeselected(p_args);
        }
    }
    protected void E_OnUsed(string[] p_args = null)
    {
        if (OnUsed != null)
        {
            OnUsed(p_args);
        }
    }
    protected void E_OnAmountUsageUpdated(string[] p_args)
    {
        if (OnAmountUsageUpdated != null)
        {
            OnAmountUsageUpdated(p_args);
        }
    }
    protected void E_OnDescriptionUpdated(string[] p_args = null)
    {
        if (OnDescriptionUpdated != null)
        {
            OnDescriptionUpdated(p_args);
        }
    }
    protected void E_OnNameUpdated(string[] p_args = null)
    {
        if (OnNameUpdated != null)
        {
            OnNameUpdated(p_args);
        }
    }
    #endregion
    #region PUBLIC METHODS
    #region GET
    public virtual int GET_AmountUsage()
    {
        return _amountUsage;
    }
    public virtual string GET_Description()
    {
        return _description;
    }
    public virtual string GET_Name()
    {
        return _name;
    }
    public virtual int GET_ID()
    {
        return _ID;
    }
    public virtual string GET_KEYID()
    {
        return _KEYID;
    }
    public virtual bool GET_Selected()
    {
        return _selected;
    }
    public virtual DateTime GET_ReceivedDateTime()
    {
        return _receivedDateTime;
    }
    #endregion
    #region SET
    public virtual void SET_AmountUsage(int p_value)
    {
        _amountUsage = p_value;
    }
    public virtual void SET_Description(string p_value)
    {
        _description = p_value;
    }
    public virtual void SET_Name(string p_value)
    {
        _name = p_value;
    }
    public virtual void SET_ID(int p_value)
    {
        _ID = p_value;
    }
    public virtual void SET_KEYID(string p_value)
    {
        _KEYID = p_value;
    }
    public virtual void SET_ReceivedDateTime(DateTime p_value)
    {
        _receivedDateTime = p_value;
    }
    #endregion
    #region ACTIONS
    public virtual void Deselect()
    {
        _selected = false;
        E_OnDeselected();
    }
    public virtual void Select()
    {
        _selected = true;
        E_OnSelected();
    }
    public virtual void Use()
    {
        if (_amountUsage <= 0)
        {
            return;
        }
        E_OnUsed();
        _amountUsage--;
    }
    #endregion
    #endregion
    #region CONSTRUCTORS
    public AbstractBonus(BonusEntity p_bonusEntity)
    {
        SET_AmountUsage(p_bonusEntity.amountUsage);
        SET_Description(p_bonusEntity.description);
        SET_Name(p_bonusEntity.name);
        SET_ID(p_bonusEntity.id);
    }
    public AbstractBonus(int p_id, int p_amountUsage, string p_description = "", string p_name = "Bonus")
    {
        SET_AmountUsage(p_amountUsage);
        SET_Description(p_description);
        SET_Name(p_name);
        SET_ID(p_id);
    }
    #endregion
}