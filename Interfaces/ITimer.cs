using System;
public interface ITimeable
{
    #region SET
    void SET_BaseTime(TimeSpan p_value);
    void SET_RemainingTime(TimeSpan p_value);
    #endregion
    #region GET
    TimeSpan GET_BaseTime();
    TimeSpan GET_RemainingTime();
    #endregion
    #region ACTIONS
    void Reset();
    void Start();
    void Pause();
    void Resume();
    void Stop();
    #endregion
}