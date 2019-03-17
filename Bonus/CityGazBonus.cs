using UnityEngine;
using System;
public class CityGazBonus
{
    #region EVENTS
    #endregion
    #region PROTECTED PROPERTIES
    protected AbstractTimer _timer;
    protected AbstractLock _lock;
    protected AbstractBonus _bonus;
    #endregion
    #region PUBLIC PROPERTIES
    public GameObject prefab;
    #endregion
    #region PROTECTED METHODS
    #endregion
    #region PUBLIC METHODS
    #region SET
    public void SET_Bonus(AbstractBonus p_bonus)
    {
        _bonus = p_bonus;
    }
    public void SET_Lock(AbstractLock p_lock)
    {
        _lock = p_lock;
    }
    public void SET_Timer(AbstractTimer p_timer)
    {
        if(p_timer == null)
        {
            _timer = new CityGazBonusTimer(new TimeSpan(0, 0, 1, 0, 0), new TimeSpan(0, 0, 1, 0, 0));
        }
        _timer = p_timer;
    }
    #endregion
    #region GET
    public AbstractBonus GET_Bonus()
    {
        return _bonus;
    }
    public AbstractLock GET_Lock()
    {
        return _lock;
    }
    public AbstractTimer GET_Timer()
    {
        return _timer;
    }
    #endregion
    #region ACTIONS
    public void Use()
    {
        GET_Bonus().Use();
    }
    public void Select()
    {
        GET_Bonus().Select();
    }
    public void Deselect()
    {
        GET_Bonus().Deselect();
    }
    #endregion
    public static AbstractBonus CreateAbstractBonus(int p_bonusID, int p_amountUsage = -1)
    {
        AbstractBonus l_output = null;
        switch (p_bonusID)
        {
            case 0:
                l_output = new BonusPunto(new BonusEntity(p_bonusID,1,"Petit bonus","Punto"));
                break;
            case 1:
                l_output = new BonusGaspard(new BonusEntity(p_bonusID, 1, "Moyen bonus", "Tubes"));
                break;
            case 2:
                l_output = new BonusTubes(new BonusEntity(p_bonusID, 1, "Grand bonus", "Gaspard"));
                break;
        }
        return l_output;
    }
    #endregion
    #region CONSTRUCTORS
    public CityGazBonus(AbstractBonus p_bonus, AbstractTimer p_timer = null, AbstractLock p_lock = null)
    {
        SET_Bonus(p_bonus);
        SET_Lock(p_lock);
        SET_Timer(p_timer);
    }
    #endregion
}