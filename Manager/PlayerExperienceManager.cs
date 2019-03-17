
using System.Collections.Generic;
using UnityEngine;

public class PlayerExperiencePoints
{
    #region EVENTS
    public delegate void PlayerLevelEvent(string[] p_args);
    public event PlayerLevelEvent OnLevelUpdated;
    public event PlayerLevelEvent OnLevelUp;
    public event PlayerLevelEvent OnExperienceUpdated;
    public event PlayerLevelEvent OnExperienceGained;
    public event PlayerLevelEvent OnExperienceTableUpdated;
    #endregion
    #region PROTECTED PROPERTIES
    protected AbstractExperienceCalculator _experienceCalculator = new BasicExperienceCalculator();
    protected float _currentExperiencePoints;
    protected int _currentLevel;
    protected List<int> _experienceTable;
    #endregion
    #region PUBLIC PROPERTIES
    public enum EXPERIENCECALCULATOR { BASIC, DOUBLE };
    public float currentExperiencePoints
    {
        get
        {
            return _currentExperiencePoints;
        }
        set
        {
            if (value >= 0 && _currentExperiencePoints != value)
            {
                Debug.Log("XP VALUE TO SET:" + value);
                string[] l_args = new string[1];
                l_args[0] = (value - _currentExperiencePoints).ToString();
                _currentExperiencePoints = value;
                E_OnExperienceUpdated(l_args);
            }
        }
    }
    public int currentLevel
    {
        get
        {
            return _currentLevel;
        }
        set
        {
            if (value >= 0 && value != _currentLevel)
            {
                Debug.Log("LVL VALUE TO SET:" + value);
                _currentLevel = value;
                E_OnLevelUpdated();
            }
        }
    }
    public List<int> experienceTable
    {
        get
        {
            return _experienceTable;
        }
        set
        {
            if (value != null && value != _experienceTable && value.Count > 0)
            {
                _experienceTable = value;
                E_OnExperienceTableUpdated();
            }
        }
    }
    #endregion
    #region PROTECTED METHODS
    #region EVENTS FIRE
    protected void E_OnLevelUpdated(string[] p_args = null)
    {
        if (OnLevelUpdated != null)
        {
            OnLevelUpdated(p_args);
        }
    }
    protected void E_OnLevelUp(string[] p_args = null)
    {
        if (OnLevelUp != null)
        {
            OnLevelUp(p_args);
        }
    }
    protected void E_OnExperienceUpdated(string[] p_args = null)
    {
        if (OnExperienceUpdated != null)
        {
            OnExperienceUpdated(p_args);
        }
    }
    protected void E_OnExperienceGained(string[] p_args = null)
    {
        if (OnExperienceGained != null)
        {
            OnExperienceGained(p_args);
        }
    }
    protected void E_OnExperienceTableUpdated(string[] p_args = null)
    {
        if (OnExperienceTableUpdated != null)
        {
            OnExperienceTableUpdated(p_args);
        }
    }
    #endregion
    #region EXPERIENCE MANAGEMENT
    public float GetRemainingPointsBeforeNextLevel(float p_experiencePoints, int p_currentLevel, List<int> p_experienceTable)
    {
        if (p_experienceTable != null && p_experienceTable.Count > p_currentLevel)
            return p_experienceTable[p_currentLevel] - p_experiencePoints;
        else
        {
            Debug.LogError("PlayerLevel -> GetRemainingPointsBeforeNextLevel : Can't GetRemainingPointsBeforeNextLevel, return Remaining = 999");
            return 999.0F;
        }
    }
    protected void SetExperienceCalculator(EXPERIENCECALCULATOR p_type)
    {
        switch (p_type)
        {
            case EXPERIENCECALCULATOR.BASIC:
                _experienceCalculator = new BasicExperienceCalculator();
                break;
            case EXPERIENCECALCULATOR.DOUBLE:
                _experienceCalculator = new DoubleExperienceCalculator();
                break;
        }
    }
    protected void UpdateExperiencePoints(float p_experiencePoints)
    {
        if (p_experiencePoints > 0)
        {
            currentExperiencePoints += _experienceCalculator.AddExperiencePoints(p_experiencePoints);
            E_OnExperienceGained();
            CheckForLevelUp();
        }
        else if (p_experiencePoints < 0)
        {
            currentExperiencePoints += _experienceCalculator.RemoveExperiencePoints(p_experiencePoints);
        }
    }
    protected void UpdateLevel(int p_levelUpAmount)
    {
        currentExperiencePoints -= experienceTable[currentLevel];
        currentLevel += p_levelUpAmount;
        E_OnLevelUp();
    }
    protected void CheckForLevelUp()
    {
        Debug.Log("Check for Level Up");
        while (HasReachedNextLevel())
        {
            UpdateLevel(1);
        }
    }
    protected bool HasReachedNextLevel()
    {
        if (!LevelIndexAvailable(currentLevel))
            return false;
        Debug.Log("XP Remaining to next level :" + GetRemainingPointsBeforeNextLevel(currentExperiencePoints, currentLevel, experienceTable));
        if (GetRemainingPointsBeforeNextLevel(currentExperiencePoints, currentLevel, experienceTable) <= 0)
        {
            return true;
        }

        return false;
    }
    protected bool LevelIndexAvailable(int p_levelIndex)
    {
        if (experienceTable == null || experienceTable.Count <= p_levelIndex)
        {
            return false;
        }

        return true;
    }
    #endregion
    #endregion
    #region PUBLIC METHODS
    public int GetNextLevelTotalExperiencePoints()
    {
        return experienceTable[currentLevel];
    }
    public float GetRemainingPointsBeforeNextLevel()
    {
        if (experienceTable != null && experienceTable.Count > currentLevel)
        {
            return experienceTable[currentLevel] - currentExperiencePoints;
        }
        else
        {
            Debug.LogError("PlayerLevel -> GetRemainingPointsBeforeNextLevel : Can't GetRemainingPointsBeforeNextLevel, return Remaining = 999");
            return 999.0F;
        }
    }
    public void AddExperiencePoints(float p_experiencePoints)
    {
        p_experiencePoints = Mathf.Abs(p_experiencePoints);
        UpdateExperiencePoints(p_experiencePoints);
    }
    public void RemoveExperiencePoints(float p_experiencePoints)
    {
        p_experiencePoints = Mathf.Abs(p_experiencePoints);
        UpdateExperiencePoints(-p_experiencePoints);
    }
    public void Assign(float p_experiencePoints, int p_level, List<int> p_experienceTable = null)
    {
        if (p_experienceTable == null)
            return;

        experienceTable = p_experienceTable;

        if (LevelIndexAvailable(p_level))
        {
            currentLevel = p_level;
        }

        currentExperiencePoints = p_experiencePoints;

    }
    #endregion
    #region CONSTRUCTORS
    public PlayerExperiencePoints(float p_currentExperiencePoints = 0F, int p_currentLevel = 1, List<int> p_experienceTable = null)
    {
        if (p_currentExperiencePoints > 0)
        {
            _currentExperiencePoints = p_currentExperiencePoints;
        }
        else
        {
            _currentExperiencePoints = 0;
        }

        if (p_currentLevel > 0)
        {
            _currentLevel = p_currentLevel;
        }
        else
        {
            _currentLevel = 1;
        }

        if (p_experienceTable != null)
        {
            _experienceTable = p_experienceTable;
        }
        else
        {
            Debug.LogWarning("PlayerExperience -> A valid experience table must be assigned to gain Level.\n Experience Points will be assigned but no Level will be gained !");
        }
        SetExperienceCalculator(EXPERIENCECALCULATOR.BASIC);
    }
    #endregion
}